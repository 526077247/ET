namespace ET
{
	public class EnterMapFinish_RemoveLobbyUI: AEvent<EventType.EnterMapFinish>
	{
		protected override async ETTask Run(EventType.EnterMapFinish args)
		{
            Scene zoneScene = args.ZoneScene;
            await SceneManagerComponent.Instance.SwitchScene<MapScene>(SceneConfig.GetSceneConfigByName(SceneNames.Map));
            var container = zoneScene.AddComponent<SceneContainer>();
            container.AddComponent<OperaComponent>();
            //container.AddComponent<CameraComponent>();
            await UIManagerComponent.Instance.DestroyWindow<UILoadingView>();

        }
	}
}
