using System.IO;
using UnityEngine;

namespace ET
{
    public class AppStart_Init : AEvent<EventType.AppStart>
    {
        protected override async ETTask Run(EventType.AppStart args)
        {
            Game.Scene.AddComponent<TimerComponent>();
            Game.Scene.AddComponent<CoroutineLockComponent>();
            Game.Scene.AddComponent<ServerConfigManagerComponent>();
            Game.Scene.AddComponent<ResourcesComponent>();
            Game.Scene.AddComponent<MaterialComponent>();
            Game.Scene.AddComponent<ImageLoaderComponent>();
            Game.Scene.AddComponent<ImageOnlineComponent>();
            Game.Scene.AddComponent<GameObjectPoolComponent>();
            Game.Scene.AddComponent<UIManagerComponent>();
            Game.Scene.AddComponent<CameraManagerComponent>();
            Game.Scene.AddComponent<SceneManagerComponent>();
            Game.Scene.AddComponent<ToastComponent>();
            
            // 加载配置
            Game.Scene.AddComponent<ConfigComponent>();
            ConfigComponent.Instance.Load();
            
            Game.Scene.AddComponent<I18NComponent>();
            Game.Scene.AddComponent<OpcodeTypeComponent>();
            Game.Scene.AddComponent<MessageDispatcherComponent>();
            
            Game.Scene.AddComponent<NetThreadComponent>();
            Game.Scene.AddComponent<SessionStreamDispatcher>();
            Game.Scene.AddComponent<ZoneSceneManagerComponent>();
            
            Game.Scene.AddComponent<GlobalComponent>();
            Game.Scene.AddComponent<AIDispatcherComponent>();
            
            await UIManagerComponent.Instance.OpenWindow<UIUpdateView>();//下载热更资源
            await ETTask.CompletedTask;
        }
    }
}
