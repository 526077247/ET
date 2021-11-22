namespace ET
{
	public class EnterMapFinish_RemoveLobbyUI: AEvent<EventType.EnterMapFinish>
	{
		protected override async ETTask Run(EventType.EnterMapFinish args)
		{
            Scene zoneScene = args.ZoneScene;
            await SceneManagerComponent.Instance.SwitchScene<MapScene>(SceneNames.Map);
            zoneScene.AddComponent<OperaComponent>();
            //container.AddComponent<CameraComponent>();
            await UIManagerComponent.Instance.DestroyWindow<UILoadingView>();

        }
	}
}
