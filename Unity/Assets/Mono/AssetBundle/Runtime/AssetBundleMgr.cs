using AssetBundles;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.ResourceLocations;

/// <summary>
/// 该文件是对addressable系统中assetbundle存放地方以及缓存的管理类
/// 由于所有的ab目前都是Remote NoStatic模式，内置在包里面的也是Remote NoStatic模式
/// 所以提供一个机制来识别ab是在缓存系统中还是内置在包里面的
/// </summary>
public class AssetBundleMgr
{
    public static string PersistentAssetBundleFolder = Path.Combine(Application.persistentDataPath, "AssetBundles"); //assetbundle cache存放目录

    public static string RemoteResCdnUrlStreamingPath = Path.Combine(Application.streamingAssetsPath, "RemoteResCdnUrl.bytes");//在streaming asset path res目录存放了远程资源的地址
    public static string RemoteResCdnUrlPersistentPath = Path.Combine(Application.persistentDataPath, "RemoteResCdnUrl.bytes");//在persistent path res目录存放了远程资源的地址

    private HashSet<string> package_ab_hashset = new HashSet<string>(); //记录内置在包里面的ab的hash值
    private Dictionary<string, string> dict_cache_ab_hash = new Dictionary<string, string>(); //记录缓存在本地的ab的hash值， 这个是为了防止重复IO读取
    private string remote_res_cdn_url = "";

    private static AssetBundleMgr Instance = null;
    public static AssetBundleMgr GetInstance()
    {
        if (Instance == null)
        {
            Instance = new AssetBundleMgr();
        }
        return Instance;
    }

    public AssetBundleMgr()
    {
        Debug.Log("PersistentAssetBundleFolder   = " + PersistentAssetBundleFolder);
        if (!Directory.Exists(PersistentAssetBundleFolder))
        {
            Directory.CreateDirectory(PersistentAssetBundleFolder);
        }
    }

    #region ============> dict_ab_location 读写
    //初始化内置的assetbundle的hash信息, 这里只读取streamingAsset中的数据， 不读取cache中的数据
    public void InitBuildInAssetBundleHashInfo()
    {
        Debug.Log("InitBuildInAssetBundleHashInfo start");
        string text;
#if UNITY_ANDROID
        string fileName = "aa/" + AssetBundleConfig.BuildInABHashFileName;
        text = AndroidAssetUtil.readAssetText(fileName);
#else
        string streamingAssetFilePath = Addressables.RuntimePath + "/" + AssetBundleConfig.BuildInABHashFileName;
        text = GameUtility.SafeReadStreamAllText(streamingAssetFilePath);
#endif

        if (string.IsNullOrEmpty(text))
        {
            Debug.Log("InitBuildInAssetBundleHashInfo error text is empty");
            return;
        }
        //Debug.Log("InitBuildInAssetBundleHashInfo text is " + text);
        string[] lines = GameUtility.StringToArrary(text);
        for (var i = 0; i < lines.Length; i++)
        {
            package_ab_hashset.Add(lines[i]);
        }
    }
    #endregion

    public void SetAddressableRemoteResCdnUrl(string cdnUrl)
    {
        Debug.Log("SetAddressableRemoteResCdnUrl cdnUrl = " + cdnUrl);
        if (string.IsNullOrEmpty(cdnUrl))
        {
            return;
        }

        this.remote_res_cdn_url = cdnUrl;

        //设置catalog的请求路径
        string newLocation = cdnUrl + "/" + "catalog_1.hash";
        Addressables.SetRemoteCatalogLocation(newLocation);

        //设置location的transfrom func
        Addressables.InternalIdTransformFunc = (IResourceLocation location) =>
        {
            string internalId = location.InternalId;
            if (internalId != null && internalId.StartsWith("http"))
            {
                var fileName = Path.GetFileName(internalId);
                string newInternalId = cdnUrl + "/" + fileName;
                // Debug.Log("InternalIdTransformFunc  " + newInternalId);
                return newInternalId;
            }
            else
            {
                return location.InternalId;
            }
        };
    }

    public string GetAddressableRemoteResCdnUrl()
    {
        return this.remote_res_cdn_url;
    }


    public string TransformAssetBundleLocation(string srcLocation, string bundleName, string hash)
    {
        if (IsAssetBundleInPackage(hash))
        {
            //如果该ab是存在于包里面的，则需要将location改成streaming asset的路径
            string location = GetAssetBundlePathAtStreamingAsset(bundleName);
            return location;
        }
        else
        {
            return srcLocation;
        }
    }

    public string GetAssetBundlePathAtStreamingAsset(string assetBundleName)
    {
        string location = Addressables.RuntimePath + "/" + PlatformMappingService.GetPlatform() + "/" + assetBundleName;
        return location;
    }

    #region ===========> asset bundle cache at persistent folder
    //判断assetbundle是否是内置在包里面的
    public bool IsAssetBundleInPackage(string hash)
    {
        if (package_ab_hashset.Contains(hash))
        {
            return true;
        }
        return false;
    }

    //获取assetbundle在本地的存储路径，该接口只会检查本地数据
    public string GetAssetBundleLocalPath(string assetBundleName, string hash, bool ignoreHash = false)
    {
        if (IsCached(assetBundleName, hash, ignoreHash))
        {
            return Path.Combine(PersistentAssetBundleFolder, assetBundleName);
        }
        else
        {
            return GetAssetBundlePathAtStreamingAsset(assetBundleName);
        }
    }

    //判断assetbundle是否是缓存在本地的
    public bool IsCached(string assetBundleName, string hash, bool ignoreHash = false)
    {
        var path = Path.Combine(PersistentAssetBundleFolder, assetBundleName);
        if (dict_cache_ab_hash.TryGetValue(assetBundleName, out string hashStr))
        {
            //如果在dict_cache_ab_hash中找到该asset bundle，则直接使用里面的数据
            if (hashStr == hash || ignoreHash)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //如果从缓存中没有找到，则进行io检查
        var hashPath = Path.Combine(PersistentAssetBundleFolder, assetBundleName + ".hash");
        if (File.Exists(path) && File.Exists(hashPath))
        {
            if (ignoreHash)
            {
                return true;
            }
            else
            {
                var hashString = File.ReadAllText(hashPath);
                dict_cache_ab_hash.Add(assetBundleName, hashString);
                return hash == hashString;
            }
        }
        return false;
    }

    public string getCachedAssetBundlePath(string assetBundleName)
    {
        return Path.Combine(PersistentAssetBundleFolder, assetBundleName);
    }

    public void CacheAssetBundle(string assetBundleName, string hash, byte[] bytes)
    {
        var path = Path.Combine(PersistentAssetBundleFolder, assetBundleName);
        File.Delete(path);
        File.WriteAllBytes(path, bytes);

        var hashPath = Path.Combine(PersistentAssetBundleFolder, assetBundleName + ".hash");
        File.Delete(hashPath);
        File.WriteAllText(hashPath, hash);

        if (dict_cache_ab_hash.TryGetValue(assetBundleName, out string hashStr))
        {
            dict_cache_ab_hash[assetBundleName] = hash;
        }
        else
        {
            dict_cache_ab_hash.Add(assetBundleName, hash);
        }
    }

    public void DelAssetBundleFromCache(string assetBundleName)
    {
        var path = Path.Combine(PersistentAssetBundleFolder, assetBundleName);
        File.Delete(path);

        var hashPath = Path.Combine(PersistentAssetBundleFolder, assetBundleName + ".hash");
        File.Delete(hashPath);

        if (dict_cache_ab_hash.ContainsKey(assetBundleName))
        {
            dict_cache_ab_hash.Remove(assetBundleName);
        }
    }

    public void ClearAllAssetBundleCache()
    {
        Directory.Delete(PersistentAssetBundleFolder, true);
        Directory.CreateDirectory(PersistentAssetBundleFolder);
        dict_cache_ab_hash.Clear();
    }

    public long GetCacheSize()
    {
        var info = new DirectoryInfo(PersistentAssetBundleFolder);
        return GetDirectorySize(info);
    }

    long GetDirectorySize(DirectoryInfo info)
    {
        long size = 0;
        foreach (var f in info.GetFiles())
            size += f.Length;
        foreach (var d in info.GetDirectories())
            size += GetDirectorySize(d);
        return size;
    }
    #endregion
}
