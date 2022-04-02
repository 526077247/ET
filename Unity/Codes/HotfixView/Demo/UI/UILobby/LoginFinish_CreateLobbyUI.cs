

namespace ET
{
	public class LoginFinish_CreateLobbyUI: AEvent<EventType.LoginFinish>
	{
		protected override void Run(EventType.LoginFinish args)
		{
			await UIManagerComponent.Instance.OpenWindow<UILobbyView,Scene>(UILobbyView.PrefabPath,args.ZoneScene);
		}
	}
}
