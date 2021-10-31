using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;

namespace AssetBundles
{
    public class AddressablesAsyncLoader : BaseAssetAsyncLoader
    {
        static Queue<AddressablesAsyncLoader> pool = new Queue<AddressablesAsyncLoader>();
        static int sequence = 0;
        protected bool isOver = false;

        public static AddressablesAsyncLoader Get()
        {
            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
            else
            {
                return new AddressablesAsyncLoader(++sequence);
            }
        }

        public static void Recycle(AddressablesAsyncLoader creater)
        {
            pool.Enqueue(creater);
        }

        public AddressablesAsyncLoader(int seqence)
        {
            Sequence = seqence;
        }

        public void InitAssetLoader(string addressPath, string label, Type assetType)
        {
            this.asset = null;
            this.result = null;
            isOver = false;
            AssetType = assetType;
            AddressPath = addressPath;

            if (string.IsNullOrEmpty(label))
            {
                this.isSkinLabelAsset = false;
                InternalLoadAssetAsync(addressPath, assetType);
            }
            else
            {
                this.isSkinLabelAsset = true;
                InternalLoadAssetWithLabelAsync(addressPath, label, assetType);
            }
        }

        public void InitSceneLoader(string addressPath, bool isAdditive)
        {
            this.asset = null;
            this.result = null;
            this.isSkinLabelAsset = false;
            isOver = false;
            AssetType = typeof(UnityEngine.SceneManagement.Scene);
            Addressables.LoadSceneAsync(addressPath, isAdditive ? UnityEngine.SceneManagement.LoadSceneMode.Additive : UnityEngine.SceneManagement.LoadSceneMode.Single).Completed += OnLoadSceneDone;
        }

        #region ===============> load asset without label
        private void InternalLoadAssetAsync(string addressPath, Type assetType)
        {
            if (assetType == typeof(Sprite))
            {
                Addressables.LoadAssetAsync<Sprite>(addressPath).Completed += OnLoadSpriteDone;
            }
            else if (assetType == typeof(SpriteAtlas))
            {
                Addressables.LoadAssetAsync<SpriteAtlas>(addressPath).Completed += OnLoadSpriteAtlasDone;
            }
            else if (assetType == typeof(TextAsset))
            {
                Addressables.LoadAssetAsync<TextAsset>(addressPath).Completed += OnLoadTextAssetDone;
            }
            else if (assetType == typeof(Material))
            {
                Addressables.LoadAssetAsync<Material>(addressPath).Completed += OnLoadMaterialAssetDone;
            }
            else if (assetType == typeof(Texture))
            {
                Addressables.LoadAssetAsync<Texture>(addressPath).Completed += OnLoadTextureAssetDone;
            }
            else if (assetType == typeof(Mesh))
            {
                Addressables.LoadAssetAsync<Mesh>(addressPath).Completed += OnLoadMeshAssetDone;
            }
            else if (assetType == typeof(AudioClip)) {
                Addressables.LoadAssetAsync<AudioClip>(addressPath).Completed += OnLoadAudioClipDone;
            }
            else if (assetType == typeof(RenderTexture)) {
	            Addressables.LoadAssetAsync<RenderTexture>(addressPath).Completed += OnLoadRenderTextureDone;
            }
            else
            {
                Addressables.LoadAssetAsync<GameObject>(addressPath).Completed += OnLoadGameObjectDone;
            }
        }

        private void OnLoadSpriteDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<Sprite> obj)
        {
            isOver = true;
            if (obj.Result)
            {
                asset = obj.Result;
            }
            else
            {
                Debug.Log("OnLoadSpriteDone Error " + AddressPath);
            }
            AddressablesManager.Instance.OnAddressablesAsyncLoaderDone(this);
        }

        private void OnLoadSpriteAtlasDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<SpriteAtlas> obj)
        {
            isOver = true;
            if (obj.Result)
            {
                asset = obj.Result;
            }
            else
            {
                Debug.Log("OnLoadSpriteAtlasDone Error " + AddressPath);
            }
            AddressablesManager.Instance.OnAddressablesAsyncLoaderDone(this);
        }

        private void OnLoadTextAssetDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<TextAsset> obj)
        {
            isOver = true;
            if (obj.Result)
            {
                asset = obj.Result;
            }
            else
            {
                Debug.Log("OnLoadTextAssetDone Error " + AddressPath);
            }
            AddressablesManager.Instance.OnAddressablesAsyncLoaderDone(this);
        }

        private void OnLoadGameObjectDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj)
        {
            isOver = true;
            if (obj.Result)
            {
                asset = obj.Result;
            }
            else
            {
                Debug.Log("OnLoadGameObjectDone Error " + AddressPath);
            }
            AddressablesManager.Instance.OnAddressablesAsyncLoaderDone(this);
        }

        private void OnLoadMaterialAssetDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<Material> obj)
        {
            isOver = true;
            if (obj.Result)
            {
                asset = obj.Result;
            }
            else
            {
                Debug.Log("OnLoadMaterialAssetDone Error " + AddressPath);
            }
            AddressablesManager.Instance.OnAddressablesAsyncLoaderDone(this);
        }

        private void OnLoadTextureAssetDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<Texture> obj)
        {
            isOver = true;
            if (obj.Result)
            {
                asset = obj.Result;
            }
            else
            {
                Debug.Log("OnLoadTextureAssetDone Error " + AddressPath);
            }
            AddressablesManager.Instance.OnAddressablesAsyncLoaderDone(this);
        }

        private void OnLoadMeshAssetDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<Mesh> obj)
        {
            isOver = true;
            if (obj.Result)
            {
                asset = obj.Result;
            }
            else
            {
                Debug.Log("OnLoadMeshAssetDone Error " + AddressPath);
            }
            AddressablesManager.Instance.OnAddressablesAsyncLoaderDone(this);
        }

        private void OnLoadAudioClipDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<AudioClip> obj) {
            if (obj.Result) {
                asset = obj.Result;
            }
            else
            {
                Debug.Log("OnLoadAudioClipDone Error " + AddressPath);
            }
            isOver = true;
            AddressablesManager.Instance.OnAddressablesAsyncLoaderDone(this);
        }
        private void OnLoadRenderTextureDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<RenderTexture> obj)
        {
            if (obj.Result)
            {
                asset = obj.Result;
            }
            else
            {
                Debug.Log("OnLoadRenderTextureDone Error " + AddressPath);
            }
            isOver = true;
            AddressablesManager.Instance.OnAddressablesAsyncLoaderDone(this);
        }

        private void OnLoadSceneDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance> obj)
        {
            isOver = true;
            AddressablesManager.Instance.OnAddressablesAsyncLoaderDone(this);
        }
        #endregion

        #region =============> load asset with label, should use this only when load skin
        private void InternalLoadAssetWithLabelAsync(string addressPath, string label, Type assetType)
        {
            if (assetType == typeof(Sprite))
            {
                Addressables.LoadAssetsAsync<Sprite>(new List<string> { addressPath, label }, null, Addressables.MergeMode.Intersection).Completed += OnLoadSpriteWithLabelDone;
            }
            else if (assetType == typeof(SpriteAtlas))
            {
                Addressables.LoadAssetsAsync<SpriteAtlas>(new List<string> { addressPath, label }, null, Addressables.MergeMode.Intersection).Completed += OnLoadSpriteAtlasWithLabelDone;
            }
            else if (assetType == typeof(TextAsset))
            {
                Addressables.LoadAssetsAsync<TextAsset>(new List<string> { addressPath, label }, null, Addressables.MergeMode.Intersection).Completed += OnLoadTextAssetWithLabelDone;
            }
            else if (assetType == typeof(Material))
            {
                Addressables.LoadAssetsAsync<Material>(new List<string> { addressPath, label }, null, Addressables.MergeMode.Intersection).Completed += OnLoadMaterialAssetWithLabelDone;
            }
            else if (assetType == typeof(Texture))
            {
                Addressables.LoadAssetsAsync<Texture>(new List<string> { addressPath, label }, null, Addressables.MergeMode.Intersection).Completed += OnLoadTextureAssetWithLabelDone;
            }
            else if (assetType == typeof(Mesh))
            {
                Addressables.LoadAssetsAsync<Mesh>(new List<string> { addressPath, label }, null, Addressables.MergeMode.Intersection).Completed += OnLoadMeshAssetWithLabelDone;
            }
            else if (assetType == typeof(AudioClip))
            {
                Addressables.LoadAssetsAsync<AudioClip>(new List<string> { addressPath, label }, null, Addressables.MergeMode.Intersection).Completed += OnLoadAudioClipWithLabelDone;
            }
            else
            {
                Addressables.LoadAssetsAsync<GameObject>(new List<string> { addressPath, label }, null, Addressables.MergeMode.Intersection).Completed += OnLoadGameObjectWithLabelDone;
            }
        }

        private void OnLoadSpriteWithLabelDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<IList<Sprite>> obj)
        {
            isOver = true;
            if (obj.Result == null || obj.Result.Count == 0)
            {
                Debug.Log("OnLoadSpriteWithLabelDone Error " + AddressPath);
            }
            else
            {
                result = obj.Result;
                asset = obj.Result[0];
            }
            AddressablesManager.Instance.OnAddressablesAsyncLoaderDone(this);
        }

        private void OnLoadSpriteAtlasWithLabelDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<IList<SpriteAtlas>> obj)
        {
            isOver = true;
            if (obj.Result == null || obj.Result.Count == 0)
            {
                Debug.Log("OnLoadSpriteAtlasWithLabelDone Error " + AddressPath);
            }
            else
            {
                result = obj.Result;
                asset = obj.Result[0];
            }
            AddressablesManager.Instance.OnAddressablesAsyncLoaderDone(this);
        }

        private void OnLoadTextAssetWithLabelDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<IList<TextAsset>> obj)
        {
            isOver = true;
            if (obj.Result == null || obj.Result.Count == 0)
            {
                Debug.Log("OnLoadSpriteAtlasWithLabelDone Error " + AddressPath);
            }
            else
            {
                result = obj.Result;
                asset = obj.Result[0];
            }
            AddressablesManager.Instance.OnAddressablesAsyncLoaderDone(this);
        }

        private void OnLoadMaterialAssetWithLabelDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<IList<Material>> obj)
        {
            isOver = true;
            if (obj.Result == null || obj.Result.Count == 0)
            {
                Debug.Log("OnLoadSpriteAtlasWithLabelDone Error " + AddressPath);
            }
            else
            {
                result = obj.Result;
                asset = obj.Result[0];
            }
            AddressablesManager.Instance.OnAddressablesAsyncLoaderDone(this);
        }

        private void OnLoadTextureAssetWithLabelDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<IList<Texture>> obj)
        {
            isOver = true;
            if (obj.Result == null || obj.Result.Count == 0)
            {
                Debug.Log("OnLoadSpriteAtlasWithLabelDone Error " + AddressPath);
            }
            else
            {
                result = obj.Result;
                asset = obj.Result[0];
            }
            AddressablesManager.Instance.OnAddressablesAsyncLoaderDone(this);
        }

        private void OnLoadMeshAssetWithLabelDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<IList<Mesh>> obj)
        {
            isOver = true;
            if (obj.Result == null || obj.Result.Count == 0)
            {
                Debug.Log("OnLoadSpriteAtlasWithLabelDone Error " + AddressPath);
            }
            else
            {
                result = obj.Result;
                asset = obj.Result[0];
            }
            AddressablesManager.Instance.OnAddressablesAsyncLoaderDone(this);
        }

        private void OnLoadAudioClipWithLabelDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<IList<AudioClip>> obj)
        {
            isOver = true;
            if (obj.Result == null || obj.Result.Count == 0)
            {
                Debug.Log("OnLoadAudioClipWithLabelDone Error " + AddressPath);
            }
            else
            {
                result = obj.Result;
                asset = obj.Result[0];
            }
            AddressablesManager.Instance.OnAddressablesAsyncLoaderDone(this);
        }

        private void OnLoadGameObjectWithLabelDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<IList<GameObject>> obj)
        {
            isOver = true;
            if (obj.Result == null || obj.Result.Count == 0)
            {
                Debug.Log("OnLoadSpriteAtlasWithLabelDone Error " + AddressPath);
            }
            else
            {
                result = obj.Result;
                asset = obj.Result[0];
            }
            AddressablesManager.Instance.OnAddressablesAsyncLoaderDone(this);
        }

        #endregion

        public int Sequence
        {
            get;
            protected set;
        }
        public Type AssetType
        {
            get;
            protected set;
        }
        public string AddressPath
        {
            get;
            protected set;
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

        public override void Dispose()
        {

            Recycle(this);
        }

        public override void Update()
        {
        }
    }
}
