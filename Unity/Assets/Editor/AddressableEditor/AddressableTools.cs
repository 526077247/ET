using AddressableExts;
using AssetBundles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

[InitializeOnLoad]
public class AddressableTools
{
    public const string is_atlas_model = "is_atlas_model";
    public const string is_packing = "is_packing";
    public const string Assets_Package = "AssetsPackage";
    public static string assetspackageConfigPath = Path.Combine(Application.dataPath, "Editor", "Addressable", "Database", Assets_Package); //config 目录
    public static string assetsPath = Path.Combine(Application.dataPath, Assets_Package); //资源目录

    public static void RunCheckAssetBundle(bool _is_atlas_model = true)
    {
        EditorUserSettings.SetConfigValue(is_atlas_model, _is_atlas_model ? "1" : "0");

        //先把之前的config删除
        clear(assetspackageConfigPath);

        //bool addressableIsError = false;//addressable是否坏了
        AddressableAssetSettings settings = AASUtility.GetSettings();
        foreach (var group in settings.groups)
        {
            if (group == null)
            {
                Logger.LogError("addressable坏了");
                //addressableIsError = true;
            }
        }

        //清除 addressables groups
        //if (_is_atlas_model || addressableIsError)
        //{
        // clearAddressablesGroups();
        //}

        //动态增加 addressables groups
        addAddressablesGroups();

        //动态增加 config
        refreshConfig();

        CheckAssetBundles.Run();

        string assetFolder = Path.Combine(Application.dataPath, AssetBundleConfig.AssetsFolderName);
        //writeAddressKeyToFile(XLuaManager.luaAssetbundleAssetName.ToLower(), AssetBundleConfig.AssetsPathMapFileName);
        //writeAddressKeyToFile(IFix.Editor.IFixEditor.patch_Name.ToLower(), BuildUtils.IfixMapFileName);

        //特殊文件
        string app_version = Path.Combine(assetFolder, BuildUtils.AppVersionFileName);
        string assets_map = Path.Combine(assetFolder, AssetBundleConfig.AssetsPathMapFileName);
        string channel_name = Path.Combine(assetFolder, BuildUtils.ChannelNameFileName);
        string notice_version = Path.Combine(assetFolder, BuildUtils.NoticeVersionFileName);
        string res_version = Path.Combine(assetFolder, BuildUtils.ResVersionFileName);
        string ifix_map = Path.Combine(assetFolder, BuildUtils.IfixMapFileName);
        string config = Path.Combine(assetFolder, BuildUtils.ConfigFileName);

        SingleFileAddress("global", app_version);
        SingleFileAddress("global", assets_map);
        SingleFileAddress("global", channel_name);
        SingleFileAddress("global", notice_version);
        SingleFileAddress("global", res_version);
        SingleFileAddress("global", ifix_map);
        SingleFileAddress("global", config);

        //设置AssetBundle Provider
        SetAllGroupsToAssetBundleEncryptProvider();
        //设置Asset Address
        SetAssetAddressAndLabel();
        //将资源全部打成远程模式
        SetAllGroupsToRemoteNoStatic();
    }

    private static void writeAddressKeyToFile(string groupsName, string fileName)
    {
        //更新assetmap
        List<AddressableAssetEntry> assets = new List<AddressableAssetEntry>();
        AASUtility.GetSettings().GetAllAssets(assets, false,
            (g) =>
            {
                return g.name.Equals(groupsName);
            });


        //把lua 的  address的key 写到文件里
        string[] address = assets.Select(e => e.address).ToArray();
        string assetFolder = Path.Combine(Application.dataPath, AssetBundleConfig.AssetsFolderName);
        var assetPathMap = Path.Combine(assetFolder, fileName);
        GameUtility.SafeWriteAllLines(assetPathMap, address);
        AssetDatabase.Refresh();
    }



    private static void clear(string path)
    {
        if (Directory.Exists(path))
        {
            string[] paths = Directory.GetFileSystemEntries(path);
            for (int i = 0; i < paths.Length; i++)
            {
                if (File.Exists(paths[i]))
                {
                    FileInfo fi = new FileInfo(paths[i]);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                    {
                        fi.Attributes = FileAttributes.Normal;
                    }
                    //Logger.LogError("=====1======" + paths[i]);
                    File.Delete(paths[i]);
                }
                else
                {
                    if (Directory.Exists(paths[i]))
                    {
                        Directory.Delete(paths[i], true);
                        var AssetString = "Assets" + Path.DirectorySeparatorChar;
                        // Logger.LogError("====2=======" + paths[i]);
                        AssetDatabase.DeleteAsset(path.Substring(paths[i].IndexOf(AssetString)));
                    }
                }
            }

            AssetDatabase.Refresh();
        }
    }

    /// <summary>
    /// 动态创建 config
    /// </summary>
    private static void refreshConfig()
    {
        Dictionary<string, bool> dic = new Dictionary<string, bool>();
        //再根据资源动态创建config
        string[] paths = Directory.GetFileSystemEntries(assetsPath);
        for (int i = 0; i < paths.Length; i++)
        {
            if (Directory.Exists(paths[i]))
            {
                DirectoryInfo di = new DirectoryInfo(paths[i]);

                if (di.Name == "Tmp")
                {
                    continue;
                }

                //if (di.Name == XLuaManager.luaAssetbundleAssetName)//Lua目录默认是root模式,所有lua脚本打包到一个group
                //{
                //    dic.Add(di.Name, false);
                //}
                //else//其它目录，如果本目录下有子目录，type是children模型，否就是root模式
                //{
                    bool hasSecondPath = false;
                    string[] secondPaths = Directory.GetFileSystemEntries(paths[i]);
                    for (int j = 0; j < secondPaths.Length; j++)
                    {
                        if (Directory.Exists(secondPaths[j]))
                        {
                            hasSecondPath = true;
                            break;
                        }
                    }

                    if (hasSecondPath)
                    {
                        //创建目录
                        createConfigDir(di.Name);
                        for (int j = 0; j < secondPaths.Length; j++)
                        {
                            if (Directory.Exists(secondPaths[j]))
                            {
                                DirectoryInfo secondDi = new DirectoryInfo(secondPaths[j]);
                                dic.Add(Path.Combine(di.Name, secondDi.Name), false);
                            }
                        }
                    }
                    else
                    {
                        dic.Add(di.Name, false);
                    }
                //}
            }
        }

        addAndDeleteBD(assetspackageConfigPath, dic);
    }

    private static void addAndDeleteBD(string path, Dictionary<string, bool> dic)
    {
        if (Directory.Exists(path))
        {
            string[] files = Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories);
            Dictionary<string, bool> tempDic = new Dictionary<string, bool>();
            for (int i = 0; i < files.Length; i++)
            {
                if (File.Exists(files[i]))
                {
                    string[] path_arr = files[i].Split(new string[] { Assets_Package + Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                    bool needFile = false;
                    foreach (var key in dic.Keys)
                    {
                        if (key + ".asset" == path_arr[1])
                        {
                            needFile = true;
                            if (!tempDic.ContainsKey(key))
                            {
                                tempDic.Add(key, false);
                            }
                        }
                    }
                    if (!needFile)
                    {
                        //删除dic中没有的config文件
                        File.Delete(files[i]);
                    }
                }
            }

            foreach (var key in dic.Keys)
            {
                if (!tempDic.ContainsKey(key))
                {
                    //创建新的config文件
                    CreateAssetBundleDispatcher(key, false);
                }
            }
            AssetDatabase.Refresh();
        }
    }


    //在Assets/Editor/Addressable/Database/AssetsPackage下创建存放子目录的config
    private static void createConfigDir(string dirName)
    {
        string databaseAssetPath = Path.Combine("Assets", "Editor", "Addressable", "Database", Assets_Package, dirName);
        if (!Directory.Exists(databaseAssetPath))
        {
            Directory.CreateDirectory(databaseAssetPath);
        }
    }

    private static void CreateAssetBundleDispatcher(string assetName, bool childrendType)
    {
        //Logger.LogError("====================" + assetName);
        string databaseAssetPath = Path.Combine("Assets", "Editor", "Addressable", "Database", Assets_Package, assetName + ".asset");
        var dir = Path.GetDirectoryName(databaseAssetPath);
        GameUtility.CheckDirAndCreateWhenNeeded(dir);

        var instance = ScriptableObject.CreateInstance<AddressableDispatcherConfig>();
        AssetDatabase.CreateAsset(instance, databaseAssetPath);
        AssetDatabase.Refresh();


        instance.PackagePath = assetName;

        if (childrendType)
        {
            instance.Type = AddressableDispatcherFilterType.Children;
        }
        else
        {
            instance.Type = AddressableDispatcherFilterType.Root;
        }

        instance.Apply();
        EditorUtility.SetDirty(instance);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void clearAddressablesGroups()
    {
        AASUtility.RemoveAllGroups();
    }

    /// <summary>
    /// 在 Addressables Groups 动态创建groups
    /// </summary>
    private static void addAddressablesGroups()
    {
        //先创建一个global

        AASUtility.CreateGroup("global", false);
        return;
        //再根据资源动态创建groups
        string[] paths = Directory.GetFileSystemEntries(assetsPath);
        for (int i = 0; i < paths.Length; i++)
        {
            if (Directory.Exists(paths[i]))
            {
                DirectoryInfo di = new DirectoryInfo(paths[i]);

                if (di.Name == "Tmp")
                {
                    continue;
                }

                if (di.Name == "Lua")//
                {
                    AASUtility.CreateGroup(di.Name.ToLower());
                }
                else
                {
                    AASUtility.CreateGroup(di.Name.ToLower());
                    string[] secondPaths = Directory.GetFileSystemEntries(paths[i]);
                    for (int j = 0; j < secondPaths.Length; j++)
                    {
                        if (Directory.Exists(secondPaths[j]))
                        {
                            DirectoryInfo secondDi = new DirectoryInfo(secondPaths[j]);
                            AASUtility.CreateGroup(di.Name.ToLower() + "_" + secondDi.Name.ToLower());
                        }
                    }

                }

            }
        }
    }

    public static void SingleFileAddress(string group, string path)
    {
        string relativePath = path.Substring(path.IndexOf("Assets" + Path.DirectorySeparatorChar));

        AASUtility.AddAssetToGroup(AssetDatabase.AssetPathToGUID(relativePath), group);
    }


    public static void AddImportAssetToaddressable(string path)
    {
        string dir = Path.GetDirectoryName(path);
        int index = dir.IndexOf(Assets_Package + Path.DirectorySeparatorChar);
        if (index < 0)
        {
            return;
        }
        dir = dir.Substring(index + (Assets_Package + Path.DirectorySeparatorChar).Length);
        var isLuaFile = false;
        if (dir.ToLower().Contains("LuaScript_Bytes_Content".ToLower()))
        {
            isLuaFile = true;
        }
        index = dir.IndexOf(Path.DirectorySeparatorChar);

        if (index > 0)
        {
            if (isLuaFile)
            {
                dir = dir.Substring(0, index);
            }
            else
            {
                index = dir.IndexOf(Path.DirectorySeparatorChar, index + 1);
                if (index > 0)
                {
                    dir = dir.Substring(0, index);
                }
            }
        }

        string groupName = dir.ToLower().Replace(Path.DirectorySeparatorChar, '_');
        string guid = AssetDatabase.AssetPathToGUID(path);
        AASUtility.AddAssetToGroup(guid, groupName);
    }


    public static void BuildPlayerContent()
    {
        AASUtility.BuildPlayerContent();
    }



    #region ===========================>设置Groups配置
    /// <summary>
    /// 将所以的groups 设置成Local_Static模式
    /// </summary>
    public static void SetAllGroupsToLocalStatic()
    {
        AddressableAssetSettings settings = AASUtility.GetSettings();
        foreach (var group in settings.groups)
        {
            SetGroupsSetting(group, false, false, BundledAssetGroupSchema.BundleNamingStyle.NoHash,
                AddressableAssetSettings.kLocalBuildPath, AddressableAssetSettings.kLocalLoadPath, true);
        }
    }

    /// <summary>
    /// 将所以的groups 设置成Remote_Static模式
    /// </summary>
    public static void SetAllGroupsToRemoteStatic()
    {
        AddressableAssetSettings settings = AASUtility.GetSettings();
        foreach (var group in settings.groups)
        {
            SetGroupsSetting(group, true, true, BundledAssetGroupSchema.BundleNamingStyle.AppendHash,
                AddressableAssetSettings.kRemoteBuildPath, AddressableAssetSettings.kRemoteLoadPath, true);
        }
    }

    /// <summary>
    /// 将所以的groups 设置成Remote_NoStatic模式
    /// </summary>
    public static void SetAllGroupsToRemoteNoStatic()
    {
        AddressableAssetSettings settings = AASUtility.GetSettings();
        settings.OverridePlayerVersion = "1";
        foreach (var group in settings.groups)
        {
            SetGroupsSetting(group, true, true, BundledAssetGroupSchema.BundleNamingStyle.NoHash,
                AddressableAssetSettings.kRemoteBuildPath, AddressableAssetSettings.kRemoteLoadPath, false);
        }
    }

    private static void SetGroupsSetting(AddressableAssetGroup group,
        bool UseAssetBundleCache,
        bool UseAssetBundleCrc,
        BundledAssetGroupSchema.BundleNamingStyle BundleNaming,
        string BuildPath,
        string LoadPath,
        bool StaticContent)
    {
        BundledAssetGroupSchema bundledAssetGroupSchema = group.GetSchema<BundledAssetGroupSchema>();
        if (bundledAssetGroupSchema == null)
        {
            bundledAssetGroupSchema = group.AddSchema<BundledAssetGroupSchema>();
        }
        //bundledAssetGroupSchema.IncludeInBuild = true;
        bundledAssetGroupSchema.UseAssetBundleCache = UseAssetBundleCache;
        bundledAssetGroupSchema.UseAssetBundleCrc = UseAssetBundleCrc;
        bundledAssetGroupSchema.BundleNaming = BundleNaming;
        bundledAssetGroupSchema.BuildPath.SetVariableByName(group.Settings, BuildPath);
        bundledAssetGroupSchema.LoadPath.SetVariableByName(group.Settings, LoadPath);
        bundledAssetGroupSchema.SetAssetBundleProviderType(typeof(AssetBundleEncryptProvider));
        EditorUtility.SetDirty(bundledAssetGroupSchema);


        ContentUpdateGroupSchema contentUpdateGroupSchema = group.GetSchema<ContentUpdateGroupSchema>();
        if (contentUpdateGroupSchema == null)
        {
            contentUpdateGroupSchema = group.AddSchema<ContentUpdateGroupSchema>();
        }
        contentUpdateGroupSchema.StaticContent = StaticContent;
        EditorUtility.SetDirty(contentUpdateGroupSchema);
    }

    private static void SetAllGroupsToAssetBundleEncryptProvider()
    {

        AddressableAssetSettings settings = AASUtility.GetSettings();
        foreach (var group in settings.groups)
        {
            BundledAssetGroupSchema bundledAssetGroupSchema = group.GetSchema<BundledAssetGroupSchema>();
            if (bundledAssetGroupSchema == null)
            {
                bundledAssetGroupSchema = group.AddSchema<BundledAssetGroupSchema>();
            }
            bundledAssetGroupSchema.SetAssetBundleProviderType(typeof(AssetBundleEncryptProvider));
            EditorUtility.SetDirty(bundledAssetGroupSchema);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 自动设置asset的label，如果是皮肤下面的asset则会同时修改其address， 同时会将带皮肤的asset写入到文件中
    /// /// </summary>
    public static void SetAssetAddressAndLabel()
    {
        Debug.Log("SetAllGroupsLabel Begin");
        var setting = AASUtility.GetSettings();
        foreach (var group in setting.groups)
        {
            string label = "default";
            List<AddressableAssetEntry> assetEntry = new List<AddressableAssetEntry>();
            group.GatherAllAssets(assetEntry, true, true, false);
            for (var i = 0; i < assetEntry.Count; i++)
            {
                //先清除掉asset身上的labels
                var labels = assetEntry[i].labels.ToArray<string>();
                if (labels.Length == 1)
                {
                    if (labels[0] != label)
                    {
                        for (var j = 0; j < labels.Length; j++)
                        {
                            assetEntry[i].SetLabel(labels[j], false);
                        }
                        assetEntry[i].SetLabel(label, true, true);
                    }
                }
                else
                {
                    for (var j = 0; j < labels.Length; j++)
                    {
                        assetEntry[i].SetLabel(labels[j], false);
                    }
                    assetEntry[i].SetLabel(label, true, true);
                }
            }
        }
        AssetDatabase.Refresh();
        Debug.Log("SetAllGroupsLabel Finished");
    }
    #endregion


    /// <summary>
    /// 将选择的目录和文件 mark addressable
    /// </summary>
    public static void MarkedAddressable()
    {
        EditorSettings.serializationMode = SerializationMode.ForceText;
        string[] strs = Selection.assetGUIDs;
        string path = AssetDatabase.GUIDToAssetPath(strs[0]);
        string addressable_path = "Assets/AssetsPackage";
        if (path.Contains(addressable_path))
        {
            if (path == addressable_path)
            {
                //EditorMenu.RunPre();
            }
            else
            {
                if (Directory.Exists(path))
                {
                    //Logger.LogError("目录");
                    MarkedSingleDirectory(path);
                }
                else
                {
                    //Logger.LogError("文件");
                    MarkedSingleFile(path);
                }
            }
        }
        else
        {
            EditorUtility.DisplayDialog("marked addressable", "请选择Assets/AssetsPackage下的文件或目录", "确定");
        }
    }

    /// <summary>
    /// mark 单个文件
    /// </summary>
    /// <param name="path"></param>
    static void MarkedSingleFile(string path)
    {
        //Logger.LogError("file path:" + path);
        string temp_path = path.Replace("Assets/AssetsPackage/", "");
        //Logger.LogError("temp_path:" + temp_path);
        string[] path_list = temp_path.Split('/');
        string groupName = "tempgroup";
        if (path_list.Length > 1)
        {
            string validation_path = "Assets/AssetsPackage/" + path_list[0] + "/" + path_list[1];
            if (path_list.Length > 2)//是文件，没有二级目录
            {
                //Logger.LogError("二级目录:" + path);
                groupName = path_list[0].ToLower() + "_" + path_list[1].ToLower();
            }
            else
            {
                //Logger.LogError("一级目录 :" + path);
                groupName = path_list[0].ToLower();
            }

            var guid = UnityEditor.AssetDatabase.AssetPathToGUID(path);
            AASUtility.AddAssetToGroup(guid, groupName);
        }
        else
        {
            Logger.LogError("file path error:" + path);
        }
    }

    /// <summary>
    /// mark 单个目录
    /// </summary>
    /// <param name="path"></param>
    static void MarkedSingleDirectory(string path)
    {
        string[] paths = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        for (int i = 0; i < paths.Length; i++)
        {
            if (File.Exists(paths[i]))
            {
                FileInfo fi = new FileInfo(paths[i]);
                if (!fi.Extension.Equals(".meta"))
                {
                    //Logger.LogError("file path:" + paths[i]);
                    MarkedSingleFile(paths[i].Replace("\\", "/"));
                }
            }
        }
    }
}
