using System;
using System.Threading;
using System.Threading.Tasks;
using AssetBundles;
using UnityEngine;

namespace ET
{
    [ObjectSystem]
    public class ResourcesComponentAwakeSystem : AwakeSystem<ResourcesComponent>
    {
        public override void Awake(ResourcesComponent self)
        {
            self.Awake();
        }
    }


    //--[[
    //-- 资源管理系统：提供资源加载管理
    //-- 注意：
    //-- 1、只提供异步接口，即使内部使用的是同步操作，对外来说只有异步
    //-- 2、对于串行执行一连串的异步操作，建议使用协程（用同步形式的代码写异步逻辑），回调方式会使代码难读
    //-- 3、理论上做到逻辑层脚本对AB名字是完全透明的，所有资源只有packagePath的概念，这里对路径进行处理
    //--]]
    public class ResourcesComponent: Entity
    {
        public static ResourcesComponent Instance { get; set; }
        AddressablesManager AddressablesManager;
        public void Awake()
        {
            Instance = this;
            AddressablesManager = AddressablesManager.Instance;
        }

        //是否有加载任务正在进行
        public bool IsProsessRunning()
        {
            return AddressablesManager.IsProsessRunning;
        }

        //异步加载Asset：协程形式
        public async ETTask<TextAsset> LoadTextAsync(string path, Action<float> progress_callback = null, Action<TextAsset> callback = null,bool ignoreError = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("path err : " + path);
                callback?.Invoke(null);
                return null;
            }
            var loader = AddressablesManager.LoadAssetAsync(path, typeof(TextAsset));

            while (!loader.isDone)
            {
                await TimerComponent.Instance.WaitAsync(1);
                progress_callback?.Invoke(loader.progress);
            }
            var asset = (TextAsset)loader.asset;
            loader.Dispose();
            if (asset == null &&!ignoreError)
                Debug.LogError("Asset load err : " + path);
            callback?.Invoke(asset);
            return asset;

        }
        //异步加载Asset：协程形式
        public async ETTask<T> LoadAsync<T>(string path, Action<T> callback = null) where T: UnityEngine.Object
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("path err : " + path);
                callback?.Invoke(null);
                return null;
            }
            var asset = await AddressablesManager.LoadAssetAsync<T>(path);

            if (asset == null)
                Debug.LogError("Asset load err : " + path);
            callback?.Invoke(asset);
            return asset;

        }
        //异步加载Asset：协程形式
        public async ETTask<UnityEngine.Object> LoadAsync(string path, Type res_type,  Action<float> progress_callback=null, Action<UnityEngine.Object> callback = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("path err : " + path);
                callback?.Invoke(null);
                return null;
            }
            var loader = AddressablesManager.LoadAssetAsync(path, res_type);

            while (!loader.isDone)
            {
                await TimerComponent.Instance.WaitAsync(1);
                progress_callback?.Invoke(loader.progress);
            }
            var asset = loader.asset;
            loader.Dispose();
            if (asset == null)
                Debug.LogError("Asset load err : " + path);
            callback?.Invoke(asset);
            return asset;

        }

        public async ETTask LoadSceneAsync(string path, bool isAdditive, Action<float> progress_callback = null, Action callback=null)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("path err : " + path);
                callback?.Invoke();
                return;
            }
            var loader = AddressablesManager.LoadSceneAsync(path, isAdditive);
            while (!loader.isDone)
            {
                await TimerComponent.Instance.WaitAsync(1);
                progress_callback?.Invoke(loader.progress);
            }
            loader.Dispose();
            callback?.Invoke();
        }


        //清理资源：切换场景时调用
        public void ClearAssetsCache(UnityEngine.Object[] excludeClearAssets = null)
        {
            AddressablesManager.ClearAssetsCache(excludeClearAssets);
            //AssetBundleManager: UnloadAllUnusedResidentAssetBundles()

            // TODO：Lua脚本要重新加载，暂时吧，后面缓缓策略
            //local luaAssetbundleName = CS.XLuaManager.Instance.AssetbundleName
            //AssetBundleManager: AddAssetbundleAssetsCache(luaAssetbundleName)
        }

        public void ReleaseAsset(UnityEngine.Object pooledGo)
        {
            AddressablesManager.ReleaseAsset(pooledGo);
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
