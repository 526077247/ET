using ET;
using IFix.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AssetBundles
{
    public class AddressablesManager
    {
        public static AddressablesManager Instance { get; private set; } = new AddressablesManager();

        private AssetBundle configBundle;

        //存放的是通过LoadAssetAsync加载的资源( 通俗的讲就是不带皮肤label的资源)
        //key为返回的unity asset, value代表这个asset被load了多少次，每一次都会返回一个handle
        Dictionary<UnityEngine.Object, int> dictAssetCaching = new Dictionary<UnityEngine.Object, int>();
        //存放的是通过LoadAssetsAsync加载的资源(通俗的讲就是带皮肤label的资源，其加载后返回的result其实是个list)
        //所以这里需要采用list将cache下
        List<KeyValuePair<UnityEngine.Object, object>> listSkinAssetCaching = new List<KeyValuePair<UnityEngine.Object, object>>();

        private Dictionary<string, Dictionary<string, bool>> assetSkinLabelMap = new Dictionary<string, Dictionary<string, bool>>(); // { address: {skin:true} }
        private string m_curSkinLabel = "skin1"; //当前正在使用的skin的label


        private int processingAddressablesAsyncLoaderCount = 0;

        AddressableUpdateAsyncOperation updateAsyncOpeartion = new AddressableUpdateAsyncOperation();




        public bool IsProsessRunning
        {
            get
            {
                return processingAddressablesAsyncLoaderCount > 0;
            }
        }

        #region ==============> Addressable 相关接口提供

        public async ETTask<AddressableUpdateAsyncOperation> CheckForCatalogUpdates()
        {
            await updateAsyncOpeartion.CoCheckForCatalogUpdates();
            return updateAsyncOpeartion;
        }

        public async ETTask<AddressableUpdateAsyncOperation> GetDownloadSizeAsync(string key)
        {
            await updateAsyncOpeartion.CoGetDownloadSizeAsync(key);
            return updateAsyncOpeartion;
        }

        public async ETTask<AddressableUpdateAsyncOperation> UpdateCatalogs(string catalog)
        {
            await updateAsyncOpeartion.CoUpdateCatalogs(catalog);
            return updateAsyncOpeartion;
        }

        public async ETTask<AddressableUpdateAsyncOperation> DownloadDependenciesAsync(List<string> keys, int iMergeMode)
        {
            Addressables.MergeMode mergeMode = (Addressables.MergeMode)iMergeMode;
            await updateAsyncOpeartion.CoDownloadDependenciesAsync(keys, mergeMode);
            return updateAsyncOpeartion;
        }
        public async ETTask<AddressableUpdateAsyncOperation> CheckUpdateContent(List<string> keys, int iMergeMode)
        {
            Addressables.MergeMode mergeMode = (Addressables.MergeMode)iMergeMode;
            await updateAsyncOpeartion.CoCheckUpdateContent(keys, mergeMode);
            return updateAsyncOpeartion;
        }
        public async ETTask<AddressableUpdateAsyncOperation> DownloadUpdateContent(List<string> keys, int iMergeMode)
        {
            Addressables.MergeMode mergeMode = (Addressables.MergeMode)iMergeMode;
            await updateAsyncOpeartion.CoDownloadUpdateContent(keys, mergeMode);
            return updateAsyncOpeartion;
        }
        
        #endregion

        #region ============== clear asset and cache
        public IEnumerator Cleanup()
        {
            // 等待所有请求完成
            // 要是不等待Unity很多版本都有各种Bug
            yield return new WaitUntil(() =>
            {
                return !IsProsessRunning;
            });
            ClearAssetsCache();

            yield break;
        }

        public void ClearAssetsCache(UnityEngine.Object[] excludeObjects = null)
        {
            Debug.Log("ClearAssetsCache");
            Dictionary<UnityEngine.Object, bool> dict_exclude_object = new Dictionary<UnityEngine.Object, bool>();
            if (excludeObjects != null)
            {
                foreach (var item in excludeObjects)
                {
                    dict_exclude_object.Add(item, true);
                }
            }

            //清除普通的asset
            List<UnityEngine.Object> keys = new List<UnityEngine.Object>();
            foreach (var key in dictAssetCaching.Keys)
            {
                if (!dict_exclude_object.ContainsKey(key))
                {
                    var value = dictAssetCaching[key];
                    for (int i = 0; i < value; i++)
                    {
                        Addressables.Release(key);
                    }
                    keys.Add(key);
                }
            }

            foreach (var key in keys)
            {
                dictAssetCaching.Remove(key);
            }

            //清除带skin label的asset
            for (int i = listSkinAssetCaching.Count - 1; i >= 0; i--)
            {
                var item = listSkinAssetCaching[i];
                if (!dict_exclude_object.ContainsKey(item.Key))
                {
                    Addressables.Release(item.Value);
                    listSkinAssetCaching.RemoveAt(i);
                }
            }
            configBundle.Unload(true);
            configBundle = null;
            Debug.Log("ClearAssetsCache Over");
        }

        public void ReleaseAsset(UnityEngine.Object go)
        {
            if (go==null)
            {
                return;
            }

            bool found = false;
            Debug.Log("ReleaseAsset " + go.name);
            //先从assetCacheing中寻找
            if (dictAssetCaching.TryGetValue(go, out int refCount))
            {
                found = true;
                Addressables.Release(go);
                refCount = refCount - 1;
                if (refCount == 0)
                {
                    dictAssetCaching.Remove(go);
                }
                else
                {
                    dictAssetCaching[go] = refCount;
                }
            }

            if (!found)
            {
                for (var i = 0; i < listSkinAssetCaching.Count; i++)
                {
                    if (listSkinAssetCaching[i].Key == go)
                    {
                        found = true;
                        Addressables.Release(listSkinAssetCaching[i].Value);
                        listSkinAssetCaching.RemoveAt(i);
                        break;
                    }
                }
            }

            if (!found)
            {
                Debug.LogError("ReleaseAsset Error " + go.name);
            }
        }
        #endregion

        public ETTask<T> LoadAssetAsync<T>(string addressPath) where T: UnityEngine.Object
        {
            ETTask<T> tTask = ETTask<T>.Create();
            var label = GetAssetSkinLabel(addressPath);
            processingAddressablesAsyncLoaderCount += 1;
            if (!string.IsNullOrEmpty(label))
            {
                var res = Addressables.LoadAssetsAsync<T>(new List<string> { addressPath, label }, null, Addressables.MergeMode.Intersection);
                res.Completed += (loader) =>
                {
                    var obj = OnAddressablesAsyncLoaderDone(loader);
                    tTask.SetResult(obj);
                };
            }
            else
            {
                var res = Addressables.LoadAssetAsync<T>(addressPath);
                res.Completed += (loader) =>
                {
                    var obj = OnAddressablesAsyncLoaderDone(loader);
                    tTask.SetResult(obj);
                };
            }
            return tTask.GetAwaiter();
        }

#region =============== LoadAssetAsync
        public BaseAssetAsyncLoader LoadAssetAsync(string addressPath, Type assetType)
        {
            var loader = AddressablesAsyncLoader.Get();
            var label = GetAssetSkinLabel(addressPath);
            processingAddressablesAsyncLoaderCount += 1;
            loader.InitAssetLoader(addressPath, label, assetType);
            return loader;
        }

        public BaseAssetAsyncLoader LoadSceneAsync(string addressPath, bool isAdditive)
        {
            var loader = AddressablesAsyncLoader.Get();
            processingAddressablesAsyncLoaderCount += 1;
            loader.InitSceneLoader(addressPath, isAdditive);
            return loader;
        }

        /*
         * loader加载资源完成后主动调用这个接口，来增加asset到cache中
         * note: 原来采用的方式是将loader加入到数组中，update的时候进行遍历，来判断loader是否完成
         * 原来的方式缺点: 1: 无效的检查太多，每帧都会调用update
         *                          2:loader在lua层会引用，其加载资源后会判断loader是否IsDone， 如果完成后会调用Dispose(),将其返还给pool中
         *                          当Lua层先调用dispose后loader返还给pool后，如果在update执行前又加载了新资源导致该loader内部值被修改后，则会出问题
         */
        public void OnAddressablesAsyncLoaderDone(AddressablesAsyncLoader loader)
        {
            processingAddressablesAsyncLoaderCount -= 1;
            if (loader.asset != null)
            {
                if (loader.isSkinLabelAsset)
                {
                    listSkinAssetCaching.Add(new KeyValuePair<UnityEngine.Object, object>(loader.asset, loader.result));
                }
                else
                {
                    int refCount;
                    if (dictAssetCaching.TryGetValue(loader.asset, out refCount))
                    {
                        dictAssetCaching[loader.asset] = refCount + 1;
                    }
                    else
                    {
                        dictAssetCaching.Add(loader.asset, 1);
                    }
                }
            }
        }

        public T OnAddressablesAsyncLoaderDone<T>(AsyncOperationHandle<IList<T>> loader) 
        {
            processingAddressablesAsyncLoaderCount -= 1;
            var res = loader.Result as IList<UnityEngine.Object>;
            if (res != null)
            {
                listSkinAssetCaching.Add(new KeyValuePair<UnityEngine.Object, object>(res[0], res));
                return loader.Result[0];
            }
            return default(T);
        }
        public T OnAddressablesAsyncLoaderDone<T>(AsyncOperationHandle<T> loader)
        {
            processingAddressablesAsyncLoaderCount -= 1;
            var res = loader.Result as UnityEngine.Object;
            if (res != null)
            {
                int refCount;
                if (dictAssetCaching.TryGetValue(res, out refCount))
                {
                    dictAssetCaching[res] = refCount + 1;
                }
                else
                {
                    dictAssetCaching.Add(res, 1);
                }
                return loader.Result;
            }
            return default(T);
        }
#endregion

#region =============> skin change begin
        public void InitAssetSkinLabelText(string text)
        {
            string[] lines = GameUtility.StringToArrary(text);
            Dictionary<string, Dictionary<string, bool>> dict = new Dictionary<string, Dictionary<string, bool>>();
            //为了防止路径里面带有空格，这里区分不采用空格来划分而采用&&
            for (var i = 0; i < lines.Length; i++)
            {
                string[] splits = lines[i].Split(new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
                if (splits.Length > 0)
                {
                    Dictionary<string, bool> labelMap = new Dictionary<string, bool>();
                    var address = splits[0];
                    for (var j = 1; j < splits.Length; j++)
                    {
                        var label = splits[j];
                        labelMap.Add(label, true);
                    }
                    dict.Add(address, labelMap);
                }
            }
            assetSkinLabelMap = dict;
        }

        public void SetCurSkinLabel(string label)
        {
            m_curSkinLabel = label;
        }

        public string GetCurSkinLabel()
        {
            return m_curSkinLabel;
        }

        //获取asset的skin label
        public string GetAssetSkinLabel(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return "";
            }

            Dictionary<string, bool> dict_label;
            if (assetSkinLabelMap.TryGetValue(address, out dict_label))
            {
                bool found;
                if (dict_label.TryGetValue(m_curSkinLabel, out found))
                {
                    //该asset在当前skin下是拥有资源的
                    return m_curSkinLabel;
                }
                else
                {
                    //这里其实是异常了，该asset在当前skin漏添加了资源
                    //TODO 看需不需要直接返回个label
                    return "";
                }
            }
            else
            {
                //证明该asset是不需要皮肤的其label应该是default
                return "";
            }
        }
#endregion

#region sync load asset function 

        /*
         * @brief 之所以是有这些接口，是为了在启动时进行使用，加快启动速度，其他地方严禁调用这里的方法
         */
        public AssetBundle SyncLoadAssetBundle(string assetBundleName)
        {
            var path = AssetBundleMgr.GetInstance().GetAssetBundleLocalPath(assetBundleName, "", true);
            var ab = AssetBundle.LoadFromFile(path, 0, (ulong)computeBundleOffset(assetBundleName));
            return ab;
        }

        private int computeBundleOffset(string bundleName)
        {
            int hashCode = bundleName.ToLower().GetHashCode();
            int offset = 47 + Math.Abs(hashCode) % 256;
            return offset;
        }


        public void StartInjectFix()
        {
#if !UNITY_EDITOR
            string assetBundleName = "hotfix_assets_all.bundle";
            if (AssetBundleMgr.GetInstance().IsCached(assetBundleName, "", true))
            {
                var path = Path.Combine(AssetBundleMgr.PersistentAssetBundleFolder, assetBundleName);
                var addressPath = "Assets/AssetsPackage/Hotfix/HotfixInfo.bytes";
                var ab = AssetBundle.LoadFromFile(path, 0, (ulong)computeBundleOffset(assetBundleName));
                TextAsset asset = (TextAsset)ab.LoadAsset(addressPath, typeof(TextAsset));
                var Assemblys = asset.text.Split(',');
                for (int i = 0; i < Assemblys.Length; i++)
                {
                    if (string.IsNullOrEmpty(Assemblys[i])) continue;
                    var bytes = (TextAsset)ab.LoadAsset("Assets/AssetsPackage/Hotfix/" + Assemblys[i] + ".patch.bytes", typeof(TextAsset));
                    if (bytes != null)
                    {
                        Debug.Log("Start Patch " + Assemblys[i]);
                        PatchManager.Load(new MemoryStream(bytes.bytes));
                    }
                }
                ab.Unload(true);
            }
#endif
        }

        public TextAsset LoadTextAsset(string addressPath)
        {
            addressPath = "Assets/AssetsPackage/" + addressPath;
#if !UNITY_EDITOR
            if (configBundle == null)
            {
                configBundle = SyncLoadAssetBundle("config_assets_all.bundle");
            }
            TextAsset asset = (TextAsset)configBundle.LoadAsset(addressPath, typeof(TextAsset));
            if (asset == null)
            {
                Debug.LogError("LoadTextAsset fail, path: "+ addressPath);
            }
            return asset;
#else
            TextAsset asset = (AssetDatabase.LoadAssetAtPath(addressPath, typeof(TextAsset)) as TextAsset);
            if (asset == null)
            {
                Debug.LogError("LoadTextAsset fail, path: " + addressPath);
            }
            return asset;
#endif
        }
        public Dictionary<string, TextAsset> LoadAllTextAsset()
        {
            Dictionary<string, TextAsset> res = new Dictionary<string, TextAsset>();
#if !UNITY_EDITOR
            
            if (configBundle == null)
            {
                configBundle = SyncLoadAssetBundle("config_assets_all.bundle");
            }
            var assets = configBundle.LoadAllAssets<TextAsset>();
            foreach (TextAsset asset in assets)
            {
                res.Add(asset.name, asset);
            }
#else
            var fullPath = "Assets/AssetsPackage/Config/";
            if (Directory.Exists(fullPath)){
                DirectoryInfo direction = new DirectoryInfo(fullPath);
                FileInfo[] files = direction.GetFiles("*",SearchOption.AllDirectories);
                for(int i=0;i<files.Length;i++){
                    if (files[i].Name.EndsWith(".meta")){
                        continue;
                    }
                    var asset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/AssetsPackage/Config/" + files[i].Name);
                    res.Add(asset.name, asset);
                }  

            }  

#endif
            return res;
        }
        #endregion
    }
}
