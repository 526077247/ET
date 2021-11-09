using IFix.Editor;
using System.Collections.Generic;
using System.IO;

using UnityEditor;

namespace ET
{
    public static class BuildHelper
    {
        private const string relativeDirPrefix = "../Release";

        public static string BuildFolder = "../Release/{0}/StreamingAssets/";

        static Dictionary<PlatformType, BuildTarget> buildmap = new Dictionary<PlatformType, BuildTarget>(PlatformTypeComparer.Instance)
        {
            { PlatformType.Android , BuildTarget.Android },
            { PlatformType.PC , BuildTarget.StandaloneWindows64 },
            { PlatformType.IOS , BuildTarget.Android },
            { PlatformType.MacOS , BuildTarget.StandaloneOSX },
        };

        static Dictionary<PlatformType, BuildTargetGroup> buildGroupmap = new Dictionary<PlatformType, BuildTargetGroup>(PlatformTypeComparer.Instance)
        {
            { PlatformType.Android , BuildTargetGroup.Android },
            { PlatformType.PC , BuildTargetGroup.Standalone },
            { PlatformType.IOS , BuildTargetGroup.iOS },
            { PlatformType.MacOS , BuildTargetGroup.Standalone },
        };
        private static void KeystoreSetting()
        {
            //PlayerSettings.Android.keystoreName = "ET.keystore";
            //PlayerSettings.Android.keyaliasName = "et";
            //PlayerSettings.keyaliasPass = "123456";
            //PlayerSettings.keystorePass = "123456";
        }
        [MenuItem("Tools/web资源服务器")]
        public static void OpenFileServer()
        {
            ProcessHelper.Run("dotnet", "FileServer.dll", "../FileServer/");
        }

        public static void Build(PlatformType type, BuildOptions buildOptions, bool isBuildExe,bool clearFolder)
        {
            EditorUserSettings.SetConfigValue(AddressableTools.is_packing, "1");
            if (buildmap[type] == EditorUserBuildSettings.activeBuildTarget)
            {
                //pack
                BuildHandle(type, buildOptions, isBuildExe,clearFolder);
            }
            else
            {
                EditorUserBuildSettings.activeBuildTargetChanged = delegate ()
                {
                    if (EditorUserBuildSettings.activeBuildTarget == buildmap[type])
                    {
                        //pack
                        BuildHandle(type, buildOptions, isBuildExe, clearFolder);
                    }
                };
                if(buildGroupmap.TryGetValue(type,out var group))
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(group, buildmap[type]);
                }
                else
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(buildmap[type]);
                }
               
            }
        }
        public static void HandleProject()
        {
            //Inject
            IFixEditor.InjectAssemblys();
            //清除图集
            //UIMVVMGen.ClearAllAtlas();


            AASUtility.CleanPlayerContent();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            //生成图集
            //UIMVVMGen.GeneratingAtlas();

            //UnityEditor.U2D.SpriteAtlasUtility.PackAllAtlases(EditorUserBuildSettings.activeBuildTarget);


            //Marked AssetsPackage Addressable
            AddressableTools.RunCheckAssetBundle();

            //AssetImportMgr.OnDataBuilderComplete();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            //Build Default Build Script
            AddressableTools.BuildPlayerContent();

        }
        static void BuildHandle(PlatformType type, BuildOptions buildOptions, bool isBuildExe,bool clearFolder)
        {
            
            
            BuildTarget buildTarget = BuildTarget.StandaloneWindows;
            string exeName = "ET";
            switch (type)
            {
                case PlatformType.PC:
                    buildTarget = BuildTarget.StandaloneWindows64;
                    exeName += ".exe";
                    IFixEditor.Patch();
                    break;
                case PlatformType.Android:
                    KeystoreSetting();
                    buildTarget = BuildTarget.Android;
                    exeName += ".apk";
                    IFixEditor.CompileToAndroid();
                    break;
                case PlatformType.IOS:
                    buildTarget = BuildTarget.iOS;
                    IFixEditor.CompileToIOS();
                    break;
                case PlatformType.MacOS:
                    buildTarget = BuildTarget.StandaloneOSX;
                    IFixEditor.Patch();
                    break;
            }
            HandleProject();
            //string fold = string.Format(BuildFolder, type);

            if (clearFolder && Directory.Exists(relativeDirPrefix))
            {
                Directory.Delete(relativeDirPrefix, true);
                Directory.CreateDirectory(relativeDirPrefix);
            }
            else
            {
                Directory.CreateDirectory(relativeDirPrefix);
            }

            //Log.Info("开始资源打包");
            //BuildPipeline.BuildAssetBundles(fold, buildAssetBundleOptions, buildTarget);

            //Log.Info("完成资源打包");

            //if (isContainAB)
            //{
            //    FileHelper.CleanDirectory("Assets/StreamingAssets/");
            //    FileHelper.CopyDirectory(fold, "Assets/StreamingAssets/");
            //}

            if (isBuildExe)
            {
                AssetDatabase.Refresh();
                string[] levels = {
                    "Assets/AssetsPackage/Scenes/InitScene/Init.unity",
                };
                Log.Info("开始EXE打包");
                BuildPipeline.BuildPlayer(levels, $"{relativeDirPrefix}/{exeName}", buildTarget, buildOptions);
                Log.Info("完成EXE打包");
            }
        }
    }
}
