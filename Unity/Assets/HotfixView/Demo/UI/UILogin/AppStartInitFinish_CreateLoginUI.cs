

namespace ET
{
	public class AppStartInitFinish_CreateLoginUI: AEvent<EventType.AppStartInitFinish>
	{
		protected override async ETTask Run(EventType.AppStartInitFinish args)
		{
			await SceneManagerComponent.Instance.SwitchScene<LoginScene>(SceneConfig.LoadingScene);

			
			if (args.ZoneScene == null)
			{
				foreach (var item in ZoneSceneManagerComponent.Instance.ZoneScenes)
				{
					if (item.Value.GetComponent<UnitComponent>() != null)
					{
						args.ZoneScene = item.Value;
						break;
					}
				}
			}
			args.ZoneScene?.RemoveComponent<SessionComponent>();
			args.ZoneScene?.GetComponent<UnitComponent>().Clear();
			args.ZoneScene?.RemoveComponent<SceneContainer>();

			await SceneManagerComponent.Instance.SwitchScene<BaseScene>(SceneConfig.LoginScene,true);
			UIManagerComponent.Instance.DestroyWindow<UILoadingView>();
			await UIManagerComponent.Instance.OpenWindow<UIUpdateView, Scene>(args.ZoneScene);
		}
	}
}
