using System;
using System.Linq;

namespace ET
{
    [ObjectSystem]
    public class SceneLoadComponentAwakeSystem: AwakeSystem<SceneLoadComponent>
    {
        public override void Awake(SceneLoadComponent self)
        {
            self.PreLoadTask = ListComponent<ETTask>.Create();
            self.Paths = ListComponent<string>.Create();
            self.Types = ListComponent<int>.Create();
            self.ObjCount = DictionaryComponent<string, int>.Create();
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
            self.Paths.Dispose();
            self.Types.Dispose();
            self.ObjCount.Dispose();
        }
    }
    [FriendClass(typeof(SceneLoadComponent))]
    public static class SceneLoadComponentSystem
    {
        //预加载prefab
        public static void AddPreloadGameObject(this SceneLoadComponent self, string path, int count)
        {
            if (self.ObjCount.ContainsKey(path))
            {
                self.ObjCount[path]++;
                return;
            }
            self.ObjCount.Add(path,count);
            self.Paths.Add(path);
            self.Types.Add(SceneLoadComponent.LoadType.GameObject);
            self.Total++;
        }
        //预加载图集
        public static void AddPreloadImage(this SceneLoadComponent self, string path)
        {
            if (self.Paths.Contains(path)) return;
            self.Paths.Add(path);
            self.Types.Add(SceneLoadComponent.LoadType.Image);
            self.Total++;
        }
        //预加载材质
        public static void AddPreloadMaterial(this SceneLoadComponent self, string path)
        {
            if (self.Paths.Contains(path)) return;
            self.Paths.Add(path);
            self.Types.Add(SceneLoadComponent.LoadType.Material);
            self.Total++;
        }

        //预加载prefab
        private static ETTask StartPreloadGameObject(this SceneLoadComponent self,string path,int count)
        {
            ETTask task = ETTask.Create();
            GameObjectPoolComponent.Instance.PreLoadGameObjectAsync(path,count, () =>
            {
                self.FinishCount++;
                self.ProgressCallback?.Invoke((float) self.FinishCount / self.Total);
                task.SetResult();
            }).Coroutine();
            return task;
        }
        //预加载图集
        private static ETTask StartPreloadImage(this SceneLoadComponent self,string path)
        {
            ETTask task = ETTask.Create();
            ImageLoaderComponent.Instance.LoadImageAsync(path, (go) =>
            {
                self.FinishCount++;
                self.ProgressCallback?.Invoke((float) self.FinishCount / self.Total);
                task.SetResult();
            }).Coroutine();
            return task;
        }
        //预加载材质
        private static ETTask StartPreloadMaterial(this SceneLoadComponent self,string path)
        {
            ETTask task = ETTask.Create();
            MaterialComponent.Instance.LoadMaterialAsync(path, (go) =>
            {
                self.FinishCount++;
                self.ProgressCallback?.Invoke((float) self.FinishCount / self.Total);
                task.SetResult();
            }).Coroutine();
            return task;
        }

        //场景加载结束：后续资源准备（预加载等）
        //注意：这里使用协程，子类别重写了，需要加载的资源添加到列表就可以了
        public static async ETTask OnPrepare(this SceneLoadComponent self,Action<float> progress_callback)
        {
            for (int i = 0; i < self.Total; i++)
            {
                switch (self.Types[i])
                {
                    case SceneLoadComponent.LoadType.Image:
                        self.PreLoadTask.Add(self.StartPreloadImage(self.Paths[i]));
                        break;
                    case SceneLoadComponent.LoadType.Material:
                        self.PreLoadTask.Add(self.StartPreloadMaterial(self.Paths[i]));
                        break;
                    case SceneLoadComponent.LoadType.GameObject:
                        self.PreLoadTask.Add(self.StartPreloadGameObject(self.Paths[i],self.ObjCount[self.Paths[i]]));
                        break;
                    default:
                        break;
                }
            }
            self.ProgressCallback = progress_callback;
            if (self.Total <= 0) return;
            await ETTaskHelper.WaitAll(self.PreLoadTask);
        }
    }
}