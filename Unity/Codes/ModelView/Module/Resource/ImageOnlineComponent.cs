using AssetBundles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ET
{
    public class ImageOnlineInfo
    {
        public int ref_count;
        public Sprite sprite;
    }

    [ObjectSystem]
    public class ImageOnlineComponentAwakeSystem : AwakeSystem<ImageOnlineComponent>
    {
        public override void Awake(ImageOnlineComponent self)
        {
            self.Awake();
        }
    }
    public class ImageOnlineComponent:Entity
    {
        public static ImageOnlineComponent Instance { get; set; }
        Dictionary<string, ImageOnlineInfo> m_cacheOnlineSprite;
        Dictionary<string,Queue<Action<Sprite>>> callback_queue;

        public void Awake()
        {
            Instance = this;
            m_cacheOnlineSprite = new Dictionary<string, ImageOnlineInfo>();
            callback_queue = new Dictionary<string, Queue<Action<Sprite>>>();
        }

        /// <summary>
        ///  获取线上图片精灵
        /// </summary>
        /// <param name="image_path">图片地址</param>
        /// <param name="callback">完成回调</param>
        /// <param name="reload">是否重新下载</param>
        public async ETTask<Sprite> GetOnlineImageSprite(string image_path,bool reload = false, Action<Sprite> callback=null)
        {
            if(!reload&& m_cacheOnlineSprite.TryGetValue(image_path,out var value))
            {
                value.ref_count++;
                callback?.Invoke(value.sprite);
            }
            else if(callback_queue.TryGetValue(image_path,out var queue)&& queue!=null)
            {
                queue.Enqueue(callback);
            }
            else
            {
                callback_queue[image_path] = new Queue<Action<Sprite>>();
                callback_queue[image_path].Enqueue(callback);
                return await LoadImageOnline(image_path, 3, !reload);//没有找到就去下载
            }
            return null;
        }
        /// <summary>
        /// 加载图片失败递归重试
        /// </summary>
        /// <param name="image_path"></param>
        /// <param name="retryCount"></param>
        /// <param name="islocal"></param>
        /// <returns></returns>
        async ETTask<Sprite> LoadImageOnline(string image_path,int retryCount = 3, bool islocal = true)
        {
            if (retryCount <= 0) return null;
            retryCount--;
            Sprite res;
            if (islocal)//先从本地取
            {
                res = await HttpGetImage(image_path, null, null, true);
                if (res != null)
                {
                    m_cacheOnlineSprite[image_path] = new ImageOnlineInfo
                    {
                        sprite = res,
                        ref_count = 1
                    };
                    CallBackAll(image_path, res);
                    return res;
                }
                Log.Debug("online_image_info path: " + image_path + " || msg:get img from local fail ");
            }
            // 从网上取
            res = await HttpGetImage(image_path);
            if (res != null)
            {
                m_cacheOnlineSprite[image_path] = new ImageOnlineInfo
                {
                    sprite = res,
                    ref_count = 1
                };
                CallBackAll(image_path,res);
                return res;
            }
            else
            {
                return await LoadImageOnline(image_path, retryCount, false);// 失败重试
            }
        }

        void CallBackAll(string image_path, Sprite res)
        {
            if (callback_queue.TryGetValue(image_path, out var queue))
            {
                while (queue.Count > 0)
                {
                    queue.Dequeue()?.Invoke(res);
                }
                callback_queue.Remove(image_path);
            }
        }
        //释放
        public void ReleaseOnlineSprite(string image_path)
        {
            if (string.IsNullOrEmpty(image_path)) return;
            if(m_cacheOnlineSprite.TryGetValue(image_path, out var value))
            {
                value.ref_count--;
                if (value.ref_count <= 0)
                {
                    m_cacheOnlineSprite.Remove(image_path);
                }
            }
        }

        public async ETTask<Sprite> HttpGetImage(string url, Dictionary<string, string> headers = null, Dictionary<string, string> extparams = null, bool islocal = false)
        {
            if (headers == null) headers = new Dictionary<string, string>();
            var asyncOp = HttpManager.Instance.HttpGetImageOnline(url, islocal, headers);
            while (!asyncOp.isDone)
            {
                await TimerComponent.Instance.WaitAsync(1);
            }
            Sprite res = null;
            if (asyncOp.result == UnityWebRequest.Result.Success)
            {
                var texture = DownloadHandlerTexture.GetContent(asyncOp);
                res = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                if (!islocal)
                {
                    SaveImageToLocal(url, texture);
                }
                if (texture != null)
                    GameObject.Destroy(texture);
            }
            asyncOp.Dispose();
            return res;
        }

        public void SaveImageToLocal(string url,Texture2D texture)
        {
            GameUtility.SafeWriteAllBytes(HttpManager.Instance.LocalImage(url), texture.EncodeToPNG());
        }
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            Instance = null;

        }
    }
}
