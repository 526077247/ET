using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{

    [ObjectSystem]
    public class SceneManagerComponentAwakeSystem : AwakeSystem<SceneManagerComponent>
    {
        public override void Awake(SceneManagerComponent self)
        {
            self.Awake();
        }
    }
    //--[[
    //-- 场景管理系统：调度和控制场景异步加载以及进度管理，展示loading界面和更新进度条数据，GC、卸载未使用资源等
    //-- 注意：
    //-- 1、资源预加载放各个场景类中自行控制
    //-- 2、场景loading的UI窗口这里统一管理，由于这个窗口很简单，更新进度数据时直接写Model层
    //--]]
    public class SceneManagerComponent:Entity
    {
        public static List<string> ScenesChangeIgnoreClean = new List<string>();
        public static List<string> DestroyWindowExceptNames = new List<string>();
        public static SceneManagerComponent Instance { get; set; }
        //当前场景
        BaseScene current_scene;
        //是否忙
        bool busing = false;
        //场景对象
        Dictionary<string, BaseScene> scenes;
        public void Awake()
        {
            Instance = this;
            scenes = new Dictionary<string, BaseScene>();
        }

        //切换场景：内部使用协程
        async ETTask CoInnerSwitchScene<T>(SceneConfig scene_config,bool needclean = false) where T: BaseScene,new()
        {
            float slid_value = 0;
            Log.Info("CoInnerSwitchScene start open uiloading");
            //打开loading界面
            await Game.EventSystem.Publish(new EventType.LoadingBegin());
            Game.EventSystem.Publish(new EventType.LoadingProgress { Progress = slid_value }).Coroutine();

            CameraManagerComponent.Instance.SetCameraStackAtLoadingStart();

            //等待资源管理器加载任务结束，否则很多Unity版本在切场景时会有异常，甚至在真机上crash
            Log.Info("CoInnerSwitchScene ProsessRunning Done ");
            while (ResourcesComponent.Instance.IsProsessRunning())
            {
                await TimerComponent.Instance.WaitAsync(1);
            }
            //清理旧场景
            if (current_scene!=null)
            {
                current_scene.CoOnLeave();
            }
            
            slid_value += 0.01f;
            Game.EventSystem.Publish(new EventType.LoadingProgress { Progress = slid_value }).Coroutine();
            await TimerComponent.Instance.WaitAsync(1);

            //清理UI
            Log.Info("CoInnerSwitchScene Clean UI");
            UIManagerComponent.Instance.DestroyWindowExceptNames(DestroyWindowExceptNames.ToArray());
            
            slid_value += 0.01f;
            Game.EventSystem.Publish(new EventType.LoadingProgress { Progress = slid_value }).Coroutine();
            //清除ImageLoaderManager里的资源缓存 这里考虑到我们是单场景
            Log.Info("CoInnerSwitchScene ImageLoaderManager Cleanup");
            ImageLoaderComponent.Instance.Clear();
            //清除预设以及其创建出来的gameobject, 这里不能清除loading的资源
            Log.Info("CoInnerSwitchScene GameObjectPool Cleanup");
            string[] cleanup_besides_path = ScenesChangeIgnoreClean.ToArray();
            if (needclean)
            {
                GameObjectPoolComponent.Instance.Cleanup(true, cleanup_besides_path);
                slid_value += 0.01f;
                Game.EventSystem.Publish(new EventType.LoadingProgress { Progress = slid_value }).Coroutine();
                //清除除loading外的资源缓存 
                using (ListComponent<UnityEngine.Object> gos = ListComponent<UnityEngine.Object>.Create())
                {
                    foreach (var path in cleanup_besides_path)
                    {
                        var go = GameObjectPoolComponent.Instance.GetCachedGoWithPath(path);
                        if (go != null)
                        {
                            gos.List.Add(go);
                        }
                    }
                    Log.Info("CoInnerSwitchScene ResourcesManager ClearAssetsCache excludeAssetLen = " + gos.List.Count);
                    ResourcesComponent.Instance.ClearAssetsCache(gos.List.ToArray());
                }
                slid_value += 0.01f;
                Game.EventSystem.Publish(new EventType.LoadingProgress { Progress = slid_value }).Coroutine();
            }
            else
            {
                slid_value += 0.02f;
                Game.EventSystem.Publish(new EventType.LoadingProgress { Progress = slid_value }).Coroutine();
            }
            await ResourcesComponent.Instance.LoadSceneAsync(SceneConfig.LoadingScene.SceneAddress, false);
            slid_value += 0.01f;
            Game.EventSystem.Publish(new EventType.LoadingProgress { Progress = slid_value }).Coroutine();
            //GC：交替重复2次，清干净一点
            GC.Collect();
            GC.Collect();
            var res = Resources.UnloadUnusedAssets();
            while (!res.isDone)
            {
                if (res.progress > 1) Log.Error("scene load waht's the fuck!");
                await TimerComponent.Instance.WaitAsync(1);
            }

            slid_value += 0.1f;
            Game.EventSystem.Publish(new EventType.LoadingProgress { Progress = slid_value }).Coroutine();
            //初始化目标场景
            if (!scenes.TryGetValue(scene_config.Name,out var logic_scene))
            {
                logic_scene = new T();
                logic_scene.Init(scene_config);
                scenes[scene_config.Name] = logic_scene;
            }
            logic_scene.OnEnter();

            slid_value += 0.02f;
            Game.EventSystem.Publish(new EventType.LoadingProgress { Progress = slid_value }).Coroutine();
            //异步加载目标场景
            await ResourcesComponent.Instance.LoadSceneAsync(scene_config.SceneAddress, false, (progress) =>
             {
                 Game.EventSystem.Publish(new EventType.LoadingProgress { Progress = slid_value + 0.65f * progress }).Coroutine();
                 if (progress > 1) Log.Error("scene load waht's the fuck!");
             });

            slid_value += 0.65f;
            Game.EventSystem.Publish(new EventType.LoadingProgress { Progress = slid_value }).Coroutine();
            //准备工作：预加载资源等
            await logic_scene.OnPrepare((progress) =>
            {
                Game.EventSystem.Publish(new EventType.LoadingProgress { Progress = slid_value + 0.15f * progress }).Coroutine();
                if (progress > 1) Log.Error("scene load waht's the fuck!");
            });

            slid_value += 0.15f;
            Game.EventSystem.Publish(new EventType.LoadingProgress { Progress = slid_value }).Coroutine();
            CameraManagerComponent.Instance.SetCameraStackAtLoadingDone();
            current_scene = logic_scene;
            logic_scene.CoOnComplete();
            slid_value = 1;
            Game.EventSystem.Publish(new EventType.LoadingProgress { Progress = slid_value }).Coroutine();
            await TimerComponent.Instance.WaitAsync(1);
            //加载完成，关闭loading界面
            await Game.EventSystem.Publish(new EventType.LoadingFinish());
            //释放loading界面引用的资源
            GameObjectPoolComponent.Instance.CleanupWithPathArray(true, cleanup_besides_path);
            busing = false;
            logic_scene.OnSwitchSceneEnd();
        }
        //切换场景
        public async ETTask SwitchScene<T>(SceneConfig scene_config,bool needclean = false) where T : BaseScene, new()
        {
            if (busing) return;
            if (scene_config==null) return;
            if (current_scene != null && current_scene.scene_config.Name == scene_config.Name)
                return;
            busing = true;
            await CoInnerSwitchScene<T>(scene_config,needclean);
        }
        //获取当前场景
        public BaseScene GetCurrentScene()
        {
            return current_scene;
        }

        public string GetCurrentSceneName()
        {
            if (current_scene != null)
            {
                return current_scene.scene_config.Name;
            }
            return "";
        }

        public bool IsInTargetScene(SceneConfig scene_config)
        {
            return current_scene != null && current_scene.scene_config.Name == scene_config.Name;
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            foreach (var scene in scenes)
            {
                scene.Value.Dispose();
            }
            scenes = null;
            base.Dispose();
            
            Instance = null;
        }
    }
}
