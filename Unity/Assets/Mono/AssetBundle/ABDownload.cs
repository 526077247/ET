using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ET
{
    public class ABDownload : Singleton<ABDownload>
    {
        const int DEFAULT_TIMEOUT = 180;

        public override void Dispose()
        {

        }

        public DownloadAssetBundleAsyncOperation DownloadAssetBundle(string url, string hash, Dictionary<string, string> headers = null, int timeout = DEFAULT_TIMEOUT)
        {
            DownloadAssetBundleAsyncOperation operate = new DownloadAssetBundleAsyncOperation();
            var request = UnityWebRequest.Get(url);
            if (timeout > 0)
            {
                request.timeout = timeout;
            }
            if (headers != null)
                foreach (var item in headers)
                {
                    request.SetRequestHeader(item.Key, item.Value);
                }
            operate.InitOperation(request, url, hash);
            return operate;
        }

    }
}
