using ET;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEngine;

/// <summary>
/// 资源导入管理类
/// </summary>
public class AssetImportMgr : AssetPostprocessor
{

    private static string pattern = "[\u4e00-\u9fbb]";

    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        for (int i = 0; i < importedAssets.Length; i++)
        {
            if (importedAssets[i].Contains("Assets"))
            {
                if (importedAssets[i].EndsWith(".dds"))
                {
                    Debug.LogError("纹理不支持.dds,请用其它格式:" + importedAssets[i]);
                    if (EditorUtility.DisplayDialog("提示", "纹理不支持.dds,请转为png、tga等其它格式，或者删除" + importedAssets[i], "忍痛删除"))
                    {
                        File.Delete(importedAssets[i]);
                    }
                }

                if (importedAssets[i].Contains(" "))
                {
                    // Debug.LogError("路径不能包含空格：" + importedAssets[i]);
                }


                if (Regex.IsMatch(importedAssets[i], pattern))
                {
                    Debug.LogError("路径不能包含中文：" + importedAssets[i]);
                }
            }

        }

        HandleAssetsAddressable(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths);
    }

    public void OnPostprocessModel()
    {

        if (!assetPath.Contains("Assets"))
        {
            return;
        }

        ModelImporter mi = (ModelImporter)assetImporter;
        if (mi == null)
            return;
        string path = assetPath.ToLower();
        bool oldReadable = mi.isReadable;
        bool newReadable = false;
        if (path.Contains("ReadEnable"))
        {
            newReadable = true;
        }
        mi.isReadable = newReadable;
        if (oldReadable != newReadable)
        {
            mi.SaveAndReimport();
        }
    }

    public void OnPreprocessTexture()
    {

        if (!assetPath.Contains("Assets/"))
        {
            return;
        }
        TextureImporter ti = (TextureImporter)assetImporter;
        if (ti == null)
            return;

        //Assets/AssetsPackage 除了UI资源外，其余的纹理都要求是2的幂次方 UI资源是因为会打图集，散图的话主要是背景 先忽略吧
        if (!assetPath.Contains("Assets/AssetsPackage/UI") && assetPath.Contains("Assets/AssetsPackage"))
        {
            var width = 0;
            var height = 0;
            ImportUtil.GetTextureRealWidthAndHeight(ti, ref width, ref height);
            if (!ImportUtil.WidthAndHeightIsPowerOfTwo(width, height) || width != height)
            {
                Debug.LogError("检测到纹理尺寸不为宽高相同的2的幂次方  路径 = " + assetPath);
            }
        }

        if (assetPath.Contains("Scene/"))
        {
            //场景里面的资源先不用处理，因为会有各种光照贴图，光照探针，烘焙出来的相关的资源，容易出问题
            return;
        }

        if (ti.textureType == TextureImporterType.NormalMap)
        {
            return;
        }

        bool saveAndReimport = false;
        if (ti.mipmapEnabled)
        {
            ti.mipmapEnabled = false;
            saveAndReimport = true;
        }

        if (ti.isReadable)
        {
            ti.isReadable = false;
            saveAndReimport = true;
        }

        if (assetPath.Contains("Assets/AssetsPackage/UI") && ti.textureType != TextureImporterType.Sprite)
        {
            ti.textureType = TextureImporterType.Sprite;
            saveAndReimport = true;
        }

        if (assetPath.Contains("Assets/AssetsPackage/Effect") && ti.textureCompression != TextureImporterCompression.Compressed)
        {
            ti.textureCompression = TextureImporterCompression.Compressed;
            saveAndReimport = true;
        }

        if (saveAndReimport)
        {
            ti.SaveAndReimport();
        }
    }

    //有资源导入时
    static void HandleAssetsAddressable(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        string is_packing = EditorUserSettings.GetConfigValue(AddressableTools.is_packing);
        if(is_packing == "1")
        {
            return;
        }

        bool runAddressable = false;

        foreach (string str in importedAssets)
        {
            //Debug.Log("Reimported Asset: " + str);
            //   AddAssetToAddressable(str);
            runAddressable |= IsAddressableAsset(str);
            if (runAddressable)
            {
                //EditorMenu.RunCheckAssetBundleWithDiscreteImages();
                //Logger.LogError("======aa========" + str);
                return;
            }
        }

        foreach (string str in deletedAssets)
        {
            //Debug.Log("Deleted Asset: " + str);
        }


        for (int i = 0; i < movedAssets.Length; i++)
        {
            //Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
            runAddressable |= IsAddressableAsset(movedAssets[i]);
            if (runAddressable)
            {
                //EditorMenu.RunCheckAssetBundleWithDiscreteImages();
                //Logger.LogError("========aa1======" + movedAssets[i]);
                return;
            }
        }
        
    }

    static bool IsAddressableAsset(string assetPath)
    {
        if (!assetPath.Contains("Assets/" + AddressableTools.Assets_Package))
        {
            return false;
        }

        //if (assetPath.Contains("Assets/" + AddressableTools.Assets_Package + "/" + XLuaManager.luaAssetbundleAssetName))
        //{
        //    return false;
        //}

        if (Directory.Exists(assetPath))
        {
            return false;
        }
        return true;
    }

    static void AddAssetToAddressable(string assetPath)
    {
        //只将Assets/AssetsPackage目录下的资源挂addressable
        if (!assetPath.Contains("Assets/" + AddressableTools.Assets_Package))
        {
            return;
        }

        if(Directory.Exists(assetPath))
        {
            return;
        }
        
        if (assetPath.Contains("Atlas"))//图集
        {
            string is_atlas_model = EditorUserSettings.GetConfigValue(AddressableTools.is_atlas_model);
            if (is_atlas_model == "0")
            {
                AddressableTools.AddImportAssetToaddressable(assetPath);
            }
        }
        else
        {
            AddressableTools.AddImportAssetToaddressable(assetPath);
        }
        
    }


    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        //Debug.LogError("After Scene is loaded and game is running");
       //EditorMenu.RunCheckAssetBundleWithDiscreteImages();
    }

    [InitializeOnLoadMethod]
    static void Init()
    {
       // EditorApplication.playModeStateChanged += OnEditorPlayModeChanged;
    }

    public static void OnEditorPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            AddressableEditor.RunCheckAssetBundle();

            OnDataBuilderComplete();
        }
    }



    public static void OnDataBuilderComplete()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
            return;
        if (settings.ActivePlayModeDataBuilder == null)
        {
            var err = "Active play mode build script is null.";
            Debug.LogError(err);

            if (BuildScript.buildCompleted != null)
            {
                var result = AddressableAssetBuildResult.CreateResult<AddressableAssetBuildResult>(null, 0, err);
                BuildScript.buildCompleted(result);
            }
            return;
        }

        if (!settings.ActivePlayModeDataBuilder.CanBuildData<AddressablesPlayModeBuildResult>())
        {
            var err = string.Format("Active build script {0} cannot build AddressablesPlayModeBuildResult.", settings.ActivePlayModeDataBuilder);
            Debug.LogError(err);
            if (BuildScript.buildCompleted != null)
            {

                var result = AddressableAssetBuildResult.CreateResult<AddressableAssetBuildResult>(null, 0, err);
                BuildScript.buildCompleted(result);
            }

            return;
        }

        var res = settings.ActivePlayModeDataBuilder.BuildData<AddressablesPlayModeBuildResult>(new AddressablesDataBuilderInput(settings));
        if (!string.IsNullOrEmpty(res.Error))
        {
            Debug.LogError(res.Error);
            EditorApplication.isPlaying = false;
        }
        else
        {
            if (BuildScript.buildCompleted != null)
                BuildScript.buildCompleted(res);
            //settings.DataBuilderCompleted(settings.ActivePlayModeDataBuilder, res);


            if (settings.OnDataBuilderComplete != null)
                settings.OnDataBuilderComplete(settings, settings.ActivePlayModeDataBuilder, res);
        }
    }

}
