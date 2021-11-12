using ET;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using static UnityEngine.AddressableAssets.Addressables;

namespace AssetBundles
{
    public class AddressableUpdateAsyncOperation : ResourceAsyncOperation
    {
        protected bool isOver = false;
        private AsyncOperationHandle downloadHandle;
        public IList<IResourceLocation> downlocations { get; set; }

        public long downloadSize { get; set; } = 0;
        public bool isSuccess { get; set; } = false;
        public string catalog { get; set; }

        public JsonData needLoadInfo = new JsonData();

        //检查catalog的更新
        public ETTask CoCheckForCatalogUpdates()
        {
            ETTask result = ETTask.Create();
            ResetValue();
            var handle = Addressables.CheckForCatalogUpdates(false);
            handle.Completed += (res) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    isOver = true;
                    if (handle.Result != null && handle.Result.Count > 0)
                    {
                        isSuccess = true;
                        catalog = handle.Result[0];
                    }
                    else
                    {
                        isSuccess = false;
                    }
                }
                else
                {
                    isOver = true;
                    isSuccess = false;
                }
                Addressables.Release(handle);
                result.SetResult();
            };
            return result.GetAwaiter();
        }

        //根据key来获取下载大小
        public ETTask CoGetDownloadSizeAsync(string[] keys)
        {
            ETTask result = ETTask.Create();
            ResetValue();
            var handle = Addressables.GetDownloadSizeAsync(keys);
            handle.Completed += (res) =>
            {
                downloadSize = handle.Result;
                isOver = true;
                isSuccess = true;
                Addressables.Release(handle);
                result.SetResult();
            };
            return result.GetAwaiter();
        }

        //下载catalogs
        public ETTask CoUpdateCatalogs(string catalog)
        {
            ETTask result = ETTask.Create();
            ResetValue();
            var handle = Addressables.UpdateCatalogs(new string[] { catalog }, false);
            handle.Completed += (res) =>
            {
                isOver = true;
                isSuccess = handle.Status == AsyncOperationStatus.Succeeded;
                Addressables.Release(handle);
                result.SetResult();
            };
            return result.GetAwaiter();
        }

        //下载资源
        public ETTask CoDownloadDependenciesAsync(List<string> keys, MergeMode mergeMode)
        {
            ETTask result = ETTask.Create();
            ResetValue();
            downloadHandle = Addressables.DownloadDependenciesAsync(keys.ConvertAll(s => (object)s), mergeMode, false);
            downloadHandle.Completed += (res) =>
            {
                isOver = true;
                isSuccess = downloadHandle.Status == AsyncOperationStatus.Succeeded;
                Addressables.Release(downloadHandle);
                result.SetResult();
            };
            return result.GetAwaiter();
        }
        //下载更新资源
        public ETTask CoCheckUpdateContent(List<string> keys, MergeMode mergeMode)
        {
            ETTask result = ETTask.Create();
            ResetValue();
            var handle = Addressables.LoadResourceLocationsAsync(keys.ConvertAll(s => (object)s), mergeMode);
            handle.Completed += (res) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    isOver = true;
                    if (handle.Result != null && handle.Result.Count > 0)
                    {
                        isSuccess = true;
                        downlocations = handle.Result;

                        string bundleName3;
                        string path;
                        AssetBundleRequestOptions data;
                        if (downlocations != null && downlocations.Count > 0)
                        {
                            foreach (var item in downlocations)
                            {
                                if (item.HasDependencies)
                                {
                                    foreach (var dep in item.Dependencies)
                                    {
                                        bundleName3 = Path.GetFileName(dep.InternalId);
                                        if (dep.Data != null)
                                        {
                                            data = dep.Data as AssetBundleRequestOptions;
                                            path = AssetBundleMgr.GetInstance().TransformAssetBundleLocation(dep.InternalId, bundleName3, data.Hash);
                                            if (UnityEngine.ResourceManagement.Util.ResourceManagerConfig.ShouldPathUseWebRequest(path))
                                            {
                                                needLoadInfo[bundleName3] = data.Hash;
                                            }
                                        }
                                    }
                                }
                            }
                        }


                    }
                    else
                    {
                        isSuccess = false;
                    }
                }
                else
                {
                    isOver = true;
                    isSuccess = false;
                }
                Addressables.Release(handle);
                result.SetResult();
            };
            return result.GetAwaiter();
        }

        //下载更新资源
        public ETTask CoDownloadUpdateContent(List<string> keys, MergeMode mergeMode)
        {
            ETTask result = ETTask.Create();
            ResetValue();
            var locHash = new HashSet<IResourceLocation>();
            string bundleName3;
            string path;
            AssetBundleRequestOptions data;
            if (downlocations != null && downlocations.Count > 0)
            {
                foreach (var item in downlocations)
                {
                    if (item.HasDependencies)
                    {
                        foreach (var dep in item.Dependencies)
                        {
                            bundleName3 = Path.GetFileName(dep.InternalId);
                            if (dep.Data != null)
                            {
                                data = dep.Data as AssetBundleRequestOptions;
                                path = AssetBundleMgr.GetInstance().TransformAssetBundleLocation(dep.InternalId, bundleName3, data.Hash);
                                //需要从网上下载这个是在资源更新的时候用到的
                                if (File.Exists(path) || !UnityEngine.ResourceManagement.Util.ResourceManagerConfig.IsPathRemote(path))
                                {
                                }
                                else if (AssetBundleMgr.GetInstance().IsCached(bundleName3, data.Hash))
                                {
                                    //persistent目录下ab是否存在，是否有缓存
                                }
                                else if (UnityEngine.ResourceManagement.Util.ResourceManagerConfig.ShouldPathUseWebRequest(path))
                                {
                                    locHash.Add(item);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            downloadHandle = Addressables.DownloadDependenciesAsync(new List<IResourceLocation>(locHash), false);
            downloadHandle.Completed += (res) =>
            {
                isOver = true;
                isSuccess = downloadHandle.Status == AsyncOperationStatus.Succeeded;

                Addressables.Release(downloadHandle);
                result.SetResult();
            };
            return result.GetAwaiter();
        }

        //重置数据
        public void ResetValue()
        {
            isOver = false;
            isSuccess = false;
            catalog = "";
            downloadSize = 0;
            needLoadInfo.Clear();
        }

        public string GetNeedDownloadinfo()
        {
            return JsonMapper.ToJson(needLoadInfo);
        }

        public override bool IsDone()
        {
            return isOver;
        }

        public override float Progress()
        {
            if (isDone)
            {
                return 1.0f;
            }
            else
            {
                return 0.0f;
            }
        }

        public float DownloadProgress()
        {
            if (isDone)
            {
                return 1.0f;
            }
            else
            {
                return downloadHandle.PercentComplete;
            }
        }

        public override void Update()
        {
        }
    }
}
