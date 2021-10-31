using AssetBundles;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadAssetBundleAsyncOperation : ResourceAsyncOperation
{
    private UnityWebRequest request;
    private UnityWebRequestAsyncOperation requestAsyncOperation;
    private string url;
    private string hash;

    public void InitOperation(UnityWebRequest request, string url, string hash)
    {
        if(url == null || hash == null)
        {
            return;
        } 
        this.url = url;
        this.hash = hash;
        this.request = request;
        this.requestAsyncOperation = request.SendWebRequest();
        this.requestAsyncOperation.completed += WebRequestOperationCompleted;
    }
    private void WebRequestOperationCompleted(UnityEngine.AsyncOperation op)
    {
        UnityWebRequestAsyncOperation remoteReq = op as UnityWebRequestAsyncOperation;
        var webReq = remoteReq.webRequest;
        if (this.request != null && string.IsNullOrEmpty(this.request.error))
        {
            var downloadHandler = this.request.downloadHandler;
            string bundleName = Path.GetFileName(this.url);
            AssetBundleMgr.GetInstance().CacheAssetBundle(bundleName, this.hash, downloadHandler.data);
        }
        else
        {
           
        }
    }

    public string errorMsg
    {
        get { return request.error; }
    }

    public bool isSuccess
    {
        get { return !(request.isNetworkError || request.isHttpError || !string.IsNullOrEmpty(request.error)); }
    }

    #region ========> override function
    public override void Update()
    {
    }

    public override bool IsDone()
    {
        if(request.isNetworkError || request.isHttpError || !string.IsNullOrEmpty(request.error)){
            return true;
        } 
        return request.isDone;
    }

    public override float Progress()
    {
        if (isDone)
        {
            return 1.0f;
        }
        else
        {
            return request.downloadProgress;
        }
    }

    public override void Dispose()
    {
        if (request != null)
        {
            request.Dispose();
        }
    }
    #endregion
}
