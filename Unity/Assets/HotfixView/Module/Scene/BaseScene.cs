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
        //预加载资源：资源路径、资源类型
        Dictionary<string, Type> preload_resources;
        //预加载GameObject：资源路径、实例化个数
        Dictionary<string, int> preload_prefab;
        //预加载图集，只有在图集模式下才会生效
        Dictionary<string, Type> preload_atlas;
        //预加载材质
        Dictionary<string, Type> preload_material;
        //预加载的bank音频资源
        Dictionary<string, bool> preload_fmodbanks;

        public virtual void Init(SceneConfig scene_config)
        {
            this.scene_config = scene_config;
            preload_resources = new Dictionary<string, Type>();
            preload_prefab = new Dictionary<string, int>();
            preload_atlas = new Dictionary<string, Type>();
            preload_material = new Dictionary<string, Type>();
            preload_fmodbanks = new Dictionary<string, bool>();
        }

        //添加预加载资源
        //注意：只有prefab类型才需要填inst_count，用于指定初始实例化个数
        public void AddPreloadResource(string path, Type res_type,int inst_count)
        {
            if(res_type == typeof(GameObject))
            {
                preload_prefab[path] = inst_count;
                //GameObjectPoolComponent.Instance.AddPersistentPrefabPath(path);
            }
            else if(res_type== typeof(SpriteAtlas))
            {
                preload_atlas[path] = res_type;
            }
            else if (res_type == typeof(Material))
            {
                preload_material[path] = res_type;
            }
            else
            {
                preload_resources[path] = res_type;
            }
        }

        /// <summary>
        /// 添加预加载的音频资源
        /// </summary>
        /// <param name="bank_name">bank的名字，不需要带FmodBanks</param>
        public void AddPreloadFmodBank(string bank_name)
        {
            preload_fmodbanks[bank_name] = true;
        }
        //场景加载结束：后续资源准备（预加载等）
        //注意：这里使用协程，子类别重写了，需要加载的资源添加到列表就可以了
        public async ETTask OnPrepare(Action<float> progress_callback)
        {
            int res_count = preload_resources.Count;
            int prefab_count = preload_prefab.Count;
            int bank_count = preload_fmodbanks.Count;
            int atlas_count = preload_atlas.Count;
            int material_count = preload_material.Count;
            int total_count = res_count + prefab_count + bank_count + atlas_count + material_count;
            if (total_count <= 0) return;
            //进度条切片，已加载数目
            float progress_slice = 1.0f / total_count;
            int finish_count = 0;

            //预加载资源
            foreach (var item in preload_resources)
            {
                ResourcesComponent.Instance.LoadAsync(item.Key, item.Value,callback:(go) =>
                {
                    finish_count++;
                    progress_callback(finish_count * progress_slice);
                }).Coroutine();
            }
            //预加载prefab
            foreach (var item in preload_prefab)
            {
                GameObjectPoolComponent.Instance.PreLoadGameObjectAsync(item.Key, item.Value, callback:() =>
                {
                    finish_count++;
                    progress_callback(finish_count * progress_slice);
                }).Coroutine();
            }
            Type sprite_type = typeof(Sprite);
            Type sprite_atlas_type = typeof(SpriteAtlas);
            //预加载图集
            foreach (var item in preload_atlas)
            {
                if(item.Value == sprite_atlas_type)
                    ImageLoaderComponent.Instance.LoadAtlasImageAsync(item.Key, callback:(sp) =>
                    {
                        finish_count++;
                        progress_callback(finish_count * progress_slice);
                    }).Coroutine();
                else
                    ImageLoaderComponent.Instance.LoadSingleImageAsync(item.Key, callback:(sp) =>
                    {
                        finish_count++;
                        progress_callback(finish_count * progress_slice);
                    }).Coroutine();
            }
            //预加载材质
            foreach (var item in preload_material)
            {
                MaterialComponent.Instance.LoadMaterialAsync(item.Key, (go) =>
                {
                    finish_count++;
                    progress_callback(finish_count * progress_slice);
                }).Coroutine();
            }
            //预加载的bank音频资源, TODO：当皮肤切换之间音频内容是相同的话，这里音频文件不需要再加载次了
            //List<UnityEngine.Object> bank_assets_list = new List<UnityEngine.Object>();
            //foreach (var item in preload_fmodbanks)
            //{
            //    SoundManager.Instance.PreLoadGameObjectAsync(item.Key, item.Value, () =>
            //    {
            //        finish_count++;
            //        progress_callback(finish_count * progress_slice);
            //    });
            //}
            //延迟帧 立即释放addressable 引用的bank text asset，因为Fmod底层load时候其实会copy份内存了
            //await TimerComponent.Instance.WaitAsync(1);
            //foreach (var asset in bank_assets_list)
            //{
            //    AddressablesManager.Instance.ReleaseAsset(asset);
            //}
            while (finish_count!= total_count)
            {
                await TimerComponent.Instance.WaitAsync(1);
            }
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
