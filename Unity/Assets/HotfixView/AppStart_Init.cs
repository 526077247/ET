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

#if !UNITY_EDITOR
            // 热修复
            Game.Scene.AddComponent<HotFixComponent>();
            await HotFixComponent.Instance.HotFix();
#endif

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
            ConfigComponent.GetAllConfigBytes = await LoadConfigHelper.LoadAllConfigBytes();
            await ConfigComponent.Instance.LoadAsync();
            Game.Scene.AddComponent<I18nComponent>();
            Game.Scene.AddComponent<OpcodeTypeComponent>();
            Game.Scene.AddComponent<MessageDispatcherComponent>();

            Game.Scene.AddComponent<NetThreadComponent>();
            Game.Scene.AddComponent<SessionStreamDispatcher>();

            Game.Scene.AddComponent<ZoneSceneManagerComponent>();

            Game.Scene.AddComponent<GlobalComponent>();
            Game.Scene.AddComponent<AIDispatcherComponent>();

            await SceneManagerComponent.Instance.SwitchScene<BaseScene>(SceneNames.Loading);
            await UIManagerComponent.Instance.OpenWindow<UIUpdateView>();//下载热更资源
            await UIManagerComponent.Instance.CloseWindow<UILoadingView>();
        }
    }
}
