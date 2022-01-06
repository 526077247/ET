using System;
namespace ET
{
    [ObjectSystem]
    public class SceneLoadComponentAwakeSystem: AwakeSystem<SceneLoadComponent>
    {
        public override void Awake(SceneLoadComponent self)
        {
            self.PreLoadTask = ListComponent<ETTask>.Create();
            self.Total = 0;
            self.FinishCount = 0;
        }
    }
    [ObjectSystem]
    public class SceneLoadComponentDestroySystem: DestroySystem<SceneLoadComponent>
    {
        public override void Destroy(SceneLoadComponent self)
        {
            self.PreLoadTask.Dispose();
        }
    }
    public static class SceneLoadComponentSystem
    {
        //预加载资源
        public static ETTask AddPreloadResources<T>(this SceneLoadComponent self, string path) where T: UnityEngine.Object
        {
            ETTask task = ETTask.Create();
            ResourcesComponent.Instance.LoadAsync<T>(path, (go) =>
            {
                self.FinishCount++;
                self.ProgressCallback?.Invoke((float) self.FinishCount / self.Total);
                task.SetResult();
            }).Coroutine();
            self.Total++;
            return task;
        }
        //预加载prefab
        public static ETTask AddPreloadGameObject(this SceneLoadComponent self,string path,int count)
        {
            ETTask task = ETTask.Create();
            GameObjectPoolComponent.Instance.PreLoadGameObjectAsync(path,count, () =>
            {
                self.FinishCount++;
                self.ProgressCallback?.Invoke((float) self.FinishCount / self.Total);
                task.SetResult();
            }).Coroutine();
            self.Total++;
            return task;
        }
        //预加载图集
        public static ETTask AddPreloadImage(this SceneLoadComponent self,string path)
        {
            ETTask task = ETTask.Create();
            ImageLoaderComponent.Instance.LoadImageAsync(path, (go) =>
            {
                self.FinishCount++;
                self.ProgressCallback?.Invoke((float) self.FinishCount / self.Total);
                task.SetResult();
            }).Coroutine();
            self.Total++;
            return task;
        }
        //预加载材质
        public static ETTask AddPreloadMaterial(this SceneLoadComponent self,string path)
        {
            ETTask task = ETTask.Create();
            MaterialComponent.Instance.LoadMaterialAsync(path, (go) =>
            {
                self.FinishCount++;
                self.ProgressCallback?.Invoke((float) self.FinishCount / self.Total);
                task.SetResult();
            }).Coroutine();
            self.Total++;
            return task;
        }

        //场景加载结束：后续资源准备（预加载等）
        //注意：这里使用协程，子类别重写了，需要加载的资源添加到列表就可以了
        public static async ETTask OnPrepare(this SceneLoadComponent self,Action<float> progress_callback)
        {
            self.ProgressCallback = progress_callback;
            if (self.Total <= 0) return;
            await ETTaskHelper.WaitAll(self.PreLoadTask);
        }
    }
}