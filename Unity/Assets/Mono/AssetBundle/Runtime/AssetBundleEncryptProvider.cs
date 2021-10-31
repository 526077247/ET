using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.Serialization;

namespace AddressableExts
{
    #region ================> copy from addressable WebRequestQueue.cs
    internal class WebRequestQueueOperation
    {
        public UnityWebRequestAsyncOperation Result;
        public Action<UnityWebRequestAsyncOperation> OnComplete;

        public bool IsDone
        {
            get { return Result != null; }
        }

        internal UnityWebRequest m_WebRequest;

        public WebRequestQueueOperation(UnityWebRequest request)
        {
            m_WebRequest = request;
        }

        internal void Complete(UnityWebRequestAsyncOperation asyncOp)
        {
            Result = asyncOp;
            OnComplete?.Invoke(Result);
        }
    }


    internal static class WebRequestQueue
    {
        private static int s_MaxRequest = 500;
        internal static Queue<WebRequestQueueOperation> s_QueuedOperations = new Queue<WebRequestQueueOperation>();
        internal static List<UnityWebRequestAsyncOperation> s_ActiveRequests = new List<UnityWebRequestAsyncOperation>();

        public static WebRequestQueueOperation QueueRequest(UnityWebRequest request)
        {
            WebRequestQueueOperation queueOperation = new WebRequestQueueOperation(request);
            if (s_ActiveRequests.Count < s_MaxRequest)
            {
                var webRequestAsyncOp = request.SendWebRequest();
                webRequestAsyncOp.completed += OnWebAsyncOpComplete;
                s_ActiveRequests.Add(webRequestAsyncOp);
                queueOperation.Complete(webRequestAsyncOp);
            }
            else
                s_QueuedOperations.Enqueue(queueOperation);

            return queueOperation;
        }

        private static void OnWebAsyncOpComplete(UnityEngine.AsyncOperation operation)
        {
            s_ActiveRequests.Remove((operation as UnityWebRequestAsyncOperation));

            if (s_QueuedOperations.Count > 0)
            {
                var nextQueuedOperation = s_QueuedOperations.Dequeue();
                var webRequestAsyncOp = nextQueuedOperation.m_WebRequest.SendWebRequest();
                webRequestAsyncOp.completed += OnWebAsyncOpComplete;
                s_ActiveRequests.Add(webRequestAsyncOp);
                nextQueuedOperation.Complete(webRequestAsyncOp);
            }
        }
    }
    #endregion

    [Serializable]
    public class AssetBundleEncryptRequestOptions : AssetBundleRequestOptions
    {

        public override long ComputeSize(IResourceLocation location, ResourceManager resourceManager)
        {
            AssetBundleEncryptRequestOptions m_Options;
            var id = resourceManager == null ? location.InternalId : resourceManager.TransformInternalId(location);
            if (!ResourceManagerConfig.IsPathRemote(id))
            {
                return 0;
            }
            string bundleName = Path.GetFileName(id);
            m_Options = location.Data as AssetBundleEncryptRequestOptions;
            var path = AssetBundleMgr.GetInstance().TransformAssetBundleLocation(id, bundleName, m_Options.Hash);
            if (File.Exists(path) || !ResourceManagerConfig.IsPathRemote(path))
            {
                //如果文件存在  情况 1、Editor模式下文件存在  2、persistent下的文件存在  3、streaming asset目录下
                return 0;
            }
            if (AssetBundleMgr.GetInstance().IsCached(bundleName, m_Options.Hash)){
                return 0;
            }
            return BundleSize;
        }
    }

    class AssetBundleEncryptResource : IAssetBundleResource
    {
        AssetBundle m_AssetBundle;
        DownloadHandler m_downloadHandler;
        UnityEngine.AsyncOperation m_RequestOperation;
        WebRequestQueueOperation m_WebRequestQueueOperation;
        ProvideHandle m_ProvideHandle;
        AssetBundleEncryptRequestOptions m_Options;
        int m_Retries;
        private int m_bundleOffset = 0;

        private static int HEAD_LEN = 47;

        UnityWebRequest CreateWebRequest(string url)
        {
            var webRequest = UnityWebRequest.Get(url);
            Debug.Log("url = " + url);
            if (m_Options.Timeout > 0)
                webRequest.timeout = m_Options.Timeout;
            if (m_Options.RedirectLimit > 0)
                webRequest.redirectLimit = m_Options.RedirectLimit;
#if !UNITY_2019_3_OR_NEWER
            webRequest.chunkedTransfer = m_Options.ChunkedTransfer;
#endif
            if (m_ProvideHandle.ResourceManager.CertificateHandlerInstance != null)
            {
                webRequest.certificateHandler = m_ProvideHandle.ResourceManager.CertificateHandlerInstance;
                webRequest.disposeCertificateHandlerOnDispose = false;
            }
            return webRequest;
        }

        float PercentComplete() { return m_RequestOperation != null ? m_RequestOperation.progress : 0.0f; }

        public AssetBundle GetAssetBundle()
        {
            if (m_AssetBundle == null && m_downloadHandler != null)
            {
                //m_AssetBundle = m_downloadHandler.assetBundle;
                byte[] newBuffer = new byte[m_downloadHandler.data.Length - m_bundleOffset];
                System.Buffer.BlockCopy(m_downloadHandler.data, m_bundleOffset, newBuffer, 0, m_downloadHandler.data.Length - m_bundleOffset);
                m_AssetBundle = AssetBundle.LoadFromMemory(newBuffer);
                //newBuffer = null;
                m_downloadHandler.Dispose();
                m_downloadHandler = null;
            }
            return m_AssetBundle;
        }

        internal void Start(ProvideHandle provideHandle)
        {
            m_Retries = 0;
            m_AssetBundle = null;
            m_downloadHandler = null;
            m_ProvideHandle = provideHandle;
            m_Options = m_ProvideHandle.Location.Data as AssetBundleEncryptRequestOptions;
            m_RequestOperation = null;
            provideHandle.SetProgressCallback(PercentComplete);
            BeginOperation();
        }

        private int computeBundleOffset(string bundleName)
        {
            int hashCode = bundleName.ToLower().GetHashCode();
            int offset = AssetBundleEncryptResource.HEAD_LEN + Math.Abs(hashCode) % 256;
            return offset;
        }

        private void BeginOperation()
        {
            string path = m_ProvideHandle.ResourceManager.TransformInternalId(m_ProvideHandle.Location);
            string bundleName = Path.GetFileName(path);
            m_bundleOffset = computeBundleOffset(bundleName);
            path = AssetBundleMgr.GetInstance().TransformAssetBundleLocation(path, bundleName, m_Options.Hash);
            if (File.Exists(path) || !ResourceManagerConfig.IsPathRemote(path))
            {
                //如果文件存在  情况 1、Editor模式下文件存在  2、persistent下的文件存在  3、streaming asset目录下
                m_RequestOperation = AssetBundle.LoadFromFileAsync(path, m_Options == null ? 0 : m_Options.Crc, (ulong)m_bundleOffset);
                m_RequestOperation.completed += LocalRequestOperationCompleted;
            }
            else if (AssetBundleMgr.GetInstance().IsCached(bundleName, m_Options.Hash))
            {
                //persistent目录下ab是否存在，是否有缓存
                string cachePath = AssetBundleMgr.GetInstance().getCachedAssetBundlePath(bundleName);
                m_RequestOperation = AssetBundle.LoadFromFileAsync(cachePath, m_Options == null ? 0 : m_Options.Crc, (ulong)m_bundleOffset);
                m_RequestOperation.completed += LocalRequestOperationCompleted;
            }
            else if (ResourceManagerConfig.ShouldPathUseWebRequest(path))
            {
                // 文件真的需要从网络上下载 
                var req = CreateWebRequest(path);
                req.disposeDownloadHandlerOnDispose = false;
                m_WebRequestQueueOperation = WebRequestQueue.QueueRequest(req);
                if (m_WebRequestQueueOperation.IsDone)
                {
                    m_RequestOperation = m_WebRequestQueueOperation.Result;
                    m_RequestOperation.completed += WebRequestOperationCompleted;
                }
                else
                {
                    m_WebRequestQueueOperation.OnComplete += asyncOp =>
                    {
                        m_RequestOperation = asyncOp;
                        m_RequestOperation.completed += WebRequestOperationCompleted;
                    };
                }
            }
            else
            {
                m_RequestOperation = null;
                m_ProvideHandle.Complete<AssetBundleEncryptResource>(null, false, new Exception(string.Format("Invalid path in AssetBundleEncryptResource: '{0}'.", path)));
            }
        }

        private void LocalRequestOperationCompleted(UnityEngine.AsyncOperation op)
        {
            m_AssetBundle = (op as AssetBundleCreateRequest).assetBundle;
            m_ProvideHandle.Complete(this, m_AssetBundle != null, null);
        }

        private void WebRequestOperationCompleted(UnityEngine.AsyncOperation op)
        {
            UnityWebRequestAsyncOperation remoteReq = op as UnityWebRequestAsyncOperation;
            var webReq = remoteReq.webRequest;
            if (string.IsNullOrEmpty(webReq.error))
            {
                //web 返回成功 需要区分是http还是android streaming asset目录
                m_downloadHandler = webReq.downloadHandler;
                string path = m_ProvideHandle.ResourceManager.TransformInternalId(m_ProvideHandle.Location);
                string bundleName = Path.GetFileName(path);

                AssetBundleMgr.GetInstance().CacheAssetBundle(bundleName, m_Options.Hash, m_downloadHandler.data);
                m_downloadHandler.Dispose();
                m_downloadHandler = null;

                string cachePath = AssetBundleMgr.GetInstance().getCachedAssetBundlePath(bundleName);
                //这里在启动的时候已经加载过了 
                if(bundleName == "luascript_bytes_content_assets_all.bundle")
                {
                    AssetBundles.AddressablesManager.Instance.ReleaseLuas();
                }
                m_RequestOperation = AssetBundle.LoadFromFileAsync(cachePath, m_Options == null ? 0 : m_Options.Crc, (ulong)m_bundleOffset);
                m_RequestOperation.completed += LocalRequestOperationCompleted;
                //m_ProvideHandle.Complete(this, true, null);
            }
            else
            {
                m_downloadHandler = webReq.downloadHandler;
                m_downloadHandler.Dispose();
                m_downloadHandler = null;
                if (m_Retries++ < m_Options.RetryCount)
                {
                    Debug.LogFormat("Web request {0} failed with error '{1}', retrying ({2}/{3})...", webReq.url, webReq.error, m_Retries, m_Options.RetryCount);
                    BeginOperation();
                }
                else
                {
                    var exception = new Exception(string.Format("RemoteAssetBundleProvider unable to load from url {0}, result='{1}'.", webReq.url, webReq.error));
                    m_ProvideHandle.Complete<AssetBundleEncryptResource>(null, false, exception);
                }
            }
            webReq.Dispose();
        }
        /// <summary>
        /// Unloads all resources associated with this asset bundle.
        /// </summary>
        public void Unload()
        {
            if (m_AssetBundle != null)
            {
                m_AssetBundle.Unload(true);
                m_AssetBundle = null;
            }
            if (m_downloadHandler != null)
            {
                m_downloadHandler.Dispose();
                m_downloadHandler = null;
            }
            m_RequestOperation = null;
        }
    }


    [DisplayName("AssetBundle Encrypt Provider")]
    public class AssetBundleEncryptProvider : ResourceProviderBase
    {
        public override void Provide(ProvideHandle providerInterface)
        {
            new AssetBundleEncryptResource().Start(providerInterface);
        }
        /// <inheritdoc/>
        public override Type GetDefaultType(IResourceLocation location)
        {
            return typeof(IAssetBundleResource);
        }

        /// <summary>
        /// Releases the asset bundle via AssetBundle.Unload(true).
        /// </summary>
        /// <param name="location">The location of the asset to release</param>
        /// <param name="asset">The asset in question</param>
        /// <returns></returns>
        public override void Release(IResourceLocation location, object asset)
        {
            if (location == null)
                throw new ArgumentNullException("location");
            if (asset == null)
            {
                Debug.LogWarningFormat("Releasing null asset bundle from location {0}.  This is an indication that the bundle failed to load.", location);
                return;
            }
            var bundle = asset as AssetBundleEncryptResource;
            if (bundle != null)
            {
                bundle.Unload();
                return;
            }
            return;
        }
    }
}
