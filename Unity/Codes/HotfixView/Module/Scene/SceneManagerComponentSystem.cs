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
        Dictionary<SceneNames,SceneConfig> GetSceneConfig()
        {
            SceneConfig InitScene = new SceneConfig
            {
                SceneAddress = "Scenes/InitScene/Init.unity",
                Name = SceneNames.Init,
            };
            SceneConfig LoadingScene = new SceneConfig
            {
                SceneAddress = "Scenes/LoadingScene/Loading.unity",
                Name = SceneNames.Loading,
            };
            SceneConfig LoginScene = new SceneConfig
            {
                SceneAddress = "Scenes/LoginScene/Login.unity",
                Name = SceneNames.Login,
            };
            SceneConfig MapScene = new SceneConfig
            {
                SceneAddress = "Scenes/MapScene/Map.unity",
                Name = SceneNames.Map,
            };

            var res = new Dictionary<SceneNames, SceneConfig>();
            res.Add(InitScene.Name,InitScene);
            res.Add(LoadingScene.Name, LoadingScene);
            res.Add(MapScene.Name, MapScene);
            res.Add(LoginScene.Name, LoginScene);
            return res;
        }

        
        public override void Awake(SceneManagerComponent self)
        {
            self.ScenesChangeIgnoreClean = new List<string>();
            self.DestroyWindowExceptNames = new List<string>();
            SceneManagerComponent.Instance = self;
            self.scenes = new Dictionary<SceneNames, BaseScene>();
            self.SceneConfigs = GetSceneConfig();

        }
    }


    public class SceneManagerComponentDestroySystem : DestroySystem<SceneManagerComponent>
    {
        public override void Destroy(SceneManagerComponent self)
        {
            self.scenes.Clear();
            self.scenes = null;
            self.ScenesChangeIgnoreClean = null;
            self.DestroyWindowExceptNames = null;
            self.SceneConfigs = null;
            SceneManagerComponent.Instance = null;
        }
    }
    //--[[
    //-- 场景管理系统：调度和控制场景异步加载以及进度管理，展示loading界面和更新进度条数据，GC、卸载未使用资源等
    //-- 注意：
    //-- 1、资源预加载放各个场景类中自行控制
    //-- 2、场景loading的UI窗口这里统一管理，由于这个窗口很简单，更新进度数据时直接写Model层
    //--]]
    public static class SceneManagerComponentSystem
    {
        
        //切换场景
        async static ETTask InnerSwitchScene<T>(this SceneManagerComponent self,SceneConfig scene_config,bool needclean = false) where T: BaseScene,new()
        {
            float slid_value = 0;
            Log.Info("InnerSwitchScene start open uiloading");
            //打开loading界面
            await Game.EventSystem.Publish(new EventType.LoadingBegin());
            Game.EventSystem.Publish(new EventType.LoadingProgress { Progress = slid_value }).Coroutine();

            CameraManagerComponent.Instance.SetCameraStackAtLoadingStart();

            //等待资源管理器加载任务结束，否则很多Unity版本在切场景时会有异常，甚至在真机上crash
            Log.Info("InnerSwitchScene ProsessRunning Done ");
            while (ResourcesComponent.Instance.IsProsessRunning())
            {
                await TimerComponent.Instance.WaitAsync(1);
            }
            //清理旧场景
            if (self.current_scene!=null)
            {
                self.current_scene.CoOnLeave();
            }
            
            slid_value += 0.01f;
            Game.EventSystem.Publish(new EventType.LoadingProgress { Progress = slid_value }).Coroutine();
            await TimerComponent.Instance.WaitAsync(1);

            //清理UI
            Log.Info("InnerSwitchScene Clean UI");
            await UIManagerComponent.Instance.DestroyWindowExceptNames(self.DestroyWindowExceptNames.ToArray());
            
            slid_value += 0.01f;
            Game.EventSystem.Publish(new EventType.LoadingProgress { Progress = slid_value }).Coroutine();
            //清除ImageLoaderManager里的资源缓存 这里考虑到我们是单场景
            Log.Info("InnerSwitchScene ImageLoaderManager Cleanup");
            ImageLoaderComponent.Instance.Clear();
            //清除预设以及其创建出来的gameobject, 这里不能清除loading的资源
            Log.Info("InnerSwitchScene GameObjectPool Cleanup");
            string[] cleanup_besides_path = self.ScenesChangeIgnoreClean.ToArray();
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
                    Log.Info("InnerSwitchScene ResourcesManager ClearAssetsCache excludeAssetLen = " + gos.List.Count);
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
            await ResourcesComponent.Instance.LoadSceneAsync(self.GetSceneConfigByName(SceneNames.Loading).SceneAddress, false);
            slid_value += 0.01f;
            Game.EventSystem.Publish(new EventType.LoadingProgress { Progress = slid_value }).Coroutine();
            //GC：交替重复2次，清干净一点
            GC.Collect();
            GC.Collect();
            ETTask task = ETTask.Create(true);
            var res = Resources.UnloadUnusedAssets();
            res.completed += (op) =>
            {
                task.SetResult();
                if (!op.isDone) Log.Error("scene load waht's the fuck!");
            };
            await task;
            task = null;
            slid_value += 0.1f;
            Game.EventSystem.Publish(new EventType.LoadingProgress { Progress = slid_value }).Coroutine();
            //初始化目标场景
            if (!self.scenes.TryGetValue(scene_config.Name,out var logic_scene))
            {
                logic_scene = self.AddChild<T,SceneConfig>(scene_config);
                self.scenes[scene_config.Name] = logic_scene;
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
            self.current_scene = logic_scene;
            logic_scene.CoOnComplete();
            slid_value = 1;
            Game.EventSystem.Publish(new EventType.LoadingProgress { Progress = slid_value }).Coroutine();
            //等久点，跳的太快
            await TimerComponent.Instance.WaitAsync(500);
            //加载完成，关闭loading界面
            await Game.EventSystem.Publish(new EventType.LoadingFinish());
            //释放loading界面引用的资源
            GameObjectPoolComponent.Instance.CleanupWithPathArray(true, cleanup_besides_path);
            self.busing = false;
            logic_scene.OnSwitchSceneEnd();
        }
        //切换场景
        public static async ETTask SwitchScene<T>(this SceneManagerComponent self, SceneConfig scene_config,bool needclean = false) where T : BaseScene, new()
        {
            if (self.busing) return;
            if (scene_config==null) return;
            if (self.current_scene != null && self.current_scene.scene_config.Name == scene_config.Name)
                return;
            self.busing = true;
            await self.InnerSwitchScene<T>(scene_config,needclean);
        }
        //切换场景
        public static async ETTask SwitchScene<T>(this SceneManagerComponent self, SceneNames scene_name, bool needclean = false) where T : BaseScene, new()
        {
            if (self.busing) return;
            var scene_config = self.GetSceneConfigByName(scene_name);
            if (scene_config == null) return;
            if (self.current_scene != null && self.current_scene.scene_config.Name == scene_config.Name)
                return;
            self.busing = true;
            await self.InnerSwitchScene<T>(scene_config, needclean);
        }
        //切换场景
        public static async ETTask SwitchScene<T>(this SceneManagerComponent self, int scene_id, bool needclean = false) where T : BaseScene, new()
        {
            if (self.busing) return;
            var scene_config = self.GetSceneConfigById(scene_id);
            if (scene_config == null) return;
            if (self.current_scene != null && self.current_scene.scene_config.Name == scene_config.Name)
                return;
            self.busing = true;
            await self.InnerSwitchScene<T>(scene_config, needclean);
        }
        //获取当前场景
        public static BaseScene GetCurrentScene(this SceneManagerComponent self)
        {
            return self.current_scene;
        }

        public static SceneNames GetCurrentSceneName(this SceneManagerComponent self)
        {
            if (self.current_scene != null)
            {
                return self.current_scene.scene_config.Name;
            }
            return SceneNames.None;
        }

        public static bool IsInTargetScene(this SceneManagerComponent self,SceneConfig scene_config)
        {
            return self.current_scene != null && self.current_scene.scene_config.Name == scene_config.Name;
        }

        public static SceneConfig GetSceneConfigByName(this SceneManagerComponent self, SceneNames name)
        {
            if (self.SceneConfigs.TryGetValue(name, out var res))
            {
                return res;
            }
            return null;
        }

        public static SceneConfig GetSceneConfigById(this SceneManagerComponent self, int id)
        {
            SceneNames name = (SceneNames)id;
            if (self.SceneConfigs.TryGetValue(name, out var res))
            {
                return res;
            }
            return null;
        }
    }
}
