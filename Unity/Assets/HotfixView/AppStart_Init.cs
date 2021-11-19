using IFix.Core;
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

#if !UNITY_EDITOR
            // 热修复
            await HotFix();
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
            Game.Scene.AddComponent<I18NComponent>();
            Game.Scene.AddComponent<OpcodeTypeComponent>();
            Game.Scene.AddComponent<MessageDispatcherComponent>();

            Game.Scene.AddComponent<NetThreadComponent>();
            Game.Scene.AddComponent<SessionStreamDispatcher>();

            Game.Scene.AddComponent<ZoneSceneManagerComponent>();

            Game.Scene.AddComponent<GlobalComponent>();
            Game.Scene.AddComponent<AIDispatcherComponent>();

            await UIManagerComponent.Instance.OpenWindow<UIUpdateView>();//下载热更资源
            await UIManagerComponent.Instance.CloseWindow<UILoadingView>();
        }

        public static async ETTask HotFix()
        {
            var asset = await ResourcesComponent.Instance.LoadTextAsync("Hotfix/HotfixInfo.bytes");
            var Assemblys = asset.text.Split(',');
            for (int i = 0; i < Assemblys.Length; i++)
            {
                if (string.IsNullOrEmpty(Assemblys[i])) continue;
                var bytes = await ResourcesComponent.Instance.LoadTextAsync("Hotfix/" + Assemblys[i] + ".patch.bytes", ignoreError: true);
                if (bytes != null)
                {
                    Log.Info("Start Patch " + Assemblys[i]);
                    PatchManager.Load(new MemoryStream(bytes.bytes));
                }
            }
        }
    }
}
