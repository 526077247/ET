using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace AssetBundles
{
    public class Config
    {
        public string remote_cdn_url;
        public string EngineVer;
        public string ResVer;
    }
    public class AssetBundleConfig
    {
        public static AssetBundleConfig Instance { get; private set; } = new AssetBundleConfig();
        public static string localSvrAppPath = "Editor/AssetBundle/LocalServer/AssetBundleServer.exe";
        public static string AssetBundlesFolderName = "AssetBundles";
        public static string AssetBundleSuffix = ".assetbundle";
        public static string AssetsFolderName = "AssetsPackage";
        public static string ChannelFolderName = "Channel";
        public static string AssetsPathMapFileName = "AssetsMap.bytes";
        public static string BuildInABHashFileName = "BuildInABHashFile.bytes";
        public static string VariantMapParttren = "Variant";
        public static string CommonMapPattren = ",";
        public static string CatalogFolderName = "com.unity.addressables";

        public static string GlobalAssetBundleName = "global_assets_all.bundle";
        private AssetBundle globalAssetBundle;
        public string remote_cdn_url { set; get; } = "";

        public string EngineVer { set; get; } = "";
        public string ResVer { set; get; } = "";

        //@brief 同步加载global asset bundle, 不通过addressable直接进行加载
        public void SyncLoadGlobalAssetBundle()
        {
#if !UNITY_EDITOR
            globalAssetBundle = AddressablesManager.Instance.SyncLoadAssetBundle(GlobalAssetBundleName);

            //读取config.json
            TextAsset[] assets = globalAssetBundle.LoadAllAssets<TextAsset>();
            for (int i = 0; i < assets.Length; i++)
            {
                string name = assets[i].name;
                string text = assets[i].text;
                if (name == "config")
                {
                    ReadConfigInfo(text);
                }
            }
            globalAssetBundle.Unload(true);
#else
	        var configAsset = AssetDatabase.LoadAssetAtPath("Assets/AssetsPackage/config.bytes", typeof(TextAsset)) as TextAsset;
            ReadConfigInfo(configAsset.text);
#endif
        }

        /*
         * 读取config.bytes文件 json格式
         * 保存了远程地址
         */
        private void ReadConfigInfo(string text)
        {
            var config = JsonUtility.FromJson<Config>(text);
            if (!string.IsNullOrEmpty(config.remote_cdn_url))
            {
                this.remote_cdn_url = config.remote_cdn_url;
            }

            if (!string.IsNullOrEmpty(config.EngineVer))
            {
                this.EngineVer = config.EngineVer;
            }

            if (!string.IsNullOrEmpty(config.ResVer))
            {
                this.ResVer = config.ResVer;
            }
            string str_platform = "pc";
            if(Application.platform == RuntimePlatform.IPhonePlayer)
            {
                str_platform = "ios";
            }
            else if(Application.platform == RuntimePlatform.Android)
            {
                str_platform = "android";
            }
            this.remote_cdn_url = string.Format("{0}/{1}_{2}", this.remote_cdn_url, this.ResVer, str_platform);
            Debug.Log(string.Format("ReadConfigInfo EngineVer:{0} ResVer:{1} remote_cdn_url:{2}", this.EngineVer, this.ResVer, this.remote_cdn_url));
        }

    }
}
