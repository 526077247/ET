using AssetBundles;
using System;
namespace ET
{
    [ObjectSystem]
    public class ResourcesComponentAwakeSystem : AwakeSystem<ResourcesComponent>
    {
        public override void Awake(ResourcesComponent self)
        {
            ResourcesComponent.Instance = self;
            self.AddressablesManager = AddressablesManager.Instance;
        }
    }
    [ObjectSystem]
    public class  ResourcesComponentDestroySystem : DestroySystem<ResourcesComponent>
    {
        public override void Destroy(ResourcesComponent self)
        {
            ResourcesComponent.Instance = null;
        }
    }
    [FriendClass(typeof(ResourcesComponent))]
    public static class ResourceComponentSystem
    {
        //是否有加载任务正在进行
        public static bool IsProsessRunning(this ResourcesComponent self)
        {
            return self.AddressablesManager.IsProsessRunning;
        }

        //异步加载Asset：协程形式
        public static async ETTask<T> LoadAsync<T>(this ResourcesComponent self,string path, Action<T> callback = null) where T: UnityEngine.Object
        {
            if (string.IsNullOrEmpty(path))
            {
                Log.Error("path err : " + path);
                callback?.Invoke(null);
                return null;
            }
            var asset = await self.AddressablesManager.LoadAssetAsync<T>(path);

            if (asset == null)
                Log.Error("Asset load err : " + path);
            callback?.Invoke(asset);
            return asset;

        }


        public static async ETTask LoadSceneAsync(this ResourcesComponent self,string path, bool isAdditive)
        {
            if (string.IsNullOrEmpty(path))
            {
                Log.Error("path err : " + path);
                return;
            }
            await self.AddressablesManager.LoadSceneAsync(path, isAdditive);
        }


        //清理资源：切换场景时调用
        public static void ClearAssetsCache(this ResourcesComponent self,UnityEngine.Object[] excludeClearAssets = null)
        {
            self.AddressablesManager.ClearAssetsCache(excludeClearAssets);
        }

        public static void ReleaseAsset(this ResourcesComponent self,UnityEngine.Object pooledGo)
        {
            self.AddressablesManager.ReleaseAsset(pooledGo);
        }
            
    }
}