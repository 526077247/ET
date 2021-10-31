namespace ET
{
	public class EnterMapFinish_RemoveLobbyUI: AEvent<EventType.EnterMapFinish>
	{
		protected override async ETTask Run(EventType.EnterMapFinish args)
		{
            UIManagerComponent.Instance.CloseWindow<UILobbyView>();
            Scene zoneScene = args.ZoneScene;
            await SceneManagerComponent.Instance.SwitchScene<MapScene>(SceneConfig.GetSceneConfigByName("Map"));
            var container = zoneScene.AddComponent<SceneContainer>();
            container.AddComponent<OperaComponent>();
            //container.AddComponent<CameraComponent>();
            UIManagerComponent.Instance.DestroyWindow<UILoadingView>();

        }
	}
}
