using AssetBundles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
namespace ET
{
    public class BaseScene:Entity
    {
        //场景配置
        public SceneConfig scene_config;
        public ListComponent<ETTask> PreLoadTask;
        public int Total;
        public int FinishCount;
        public Action<float> ProgressCallback;
        public virtual void Awake(SceneConfig scene_config)
        {
            this.scene_config = scene_config;
            Total = 0;
            FinishCount = 0;
        }
        //预加载资源
        public ETTask AddPreloadResources<T>(string path) where T: UnityEngine.Object
        {
            ETTask task = ETTask.Create();
            ResourcesComponent.Instance.LoadAsync<T>(path, (go) =>
            {
                FinishCount++;
                ProgressCallback?.Invoke((float) FinishCount / Total);
                task.SetResult();
            }).Coroutine();
            Total++;
            return task;
        }
        //预加载prefab
        public ETTask AddPreloadGameObject(string path,int count)
        {
            ETTask task = ETTask.Create();
            GameObjectPoolComponent.Instance.PreLoadGameObjectAsync(path,count, () =>
            {
                FinishCount++;
                ProgressCallback?.Invoke((float) FinishCount / Total);
                task.SetResult();
            }).Coroutine();
            Total++;
            return task;
        }
        //预加载图集
        public ETTask AddPreloadImage(string path)
        {
            ETTask task = ETTask.Create();
            ImageLoaderComponent.Instance.LoadImageAsync(path, (go) =>
            {
                FinishCount++;
                ProgressCallback?.Invoke((float) FinishCount / Total);
                task.SetResult();
            }).Coroutine();
            Total++;
            return task;
        }
        //预加载材质
        public ETTask AddPreloadMaterial(string path)
        {
            ETTask task = ETTask.Create();
            MaterialComponent.Instance.LoadMaterialAsync(path, (go) =>
            {
                FinishCount++;
                ProgressCallback?.Invoke((float) FinishCount / Total);
                task.SetResult();
            }).Coroutine();
            Total++;
            return task;
        }

        //场景加载结束：后续资源准备（预加载等）
        //注意：这里使用协程，子类别重写了，需要加载的资源添加到列表就可以了
        public async ETTask OnPrepare(Action<float> progress_callback)
        {
            this.ProgressCallback = progress_callback;
            if (Total <= 0) return;
            await ETTaskHelper.WaitAll(PreLoadTask);
        }
        //加载前的初始化
        public virtual void OnEnter()
        {

        }
        //场景加载完毕
        public virtual void CoOnComplete()
        {

        }
        //离开场景：清理场景资源
        public virtual void CoOnLeave()
        {

        }
        //转场景结束
        public virtual void OnSwitchSceneEnd()
        {

        }
    }
}
