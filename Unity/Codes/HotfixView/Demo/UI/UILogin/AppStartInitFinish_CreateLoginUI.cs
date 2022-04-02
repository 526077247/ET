

namespace ET
{
	public class AppStartInitFinish_CreateLoginUI: AEvent<EventType.AppStartInitFinish>
	{
		protected override void Run(EventType.AppStartInitFinish args)
		{
			await SceneManagerComponent.Instance.SwitchScene(SceneNames.Login,true);
			await UIManagerComponent.Instance.OpenWindow<UILoginView,Scene>(UILoginView.PrefabPath,args.ZoneScene);
			await UIManagerComponent.Instance.CloseWindow<UILoadingView>();
		}
	}
}
