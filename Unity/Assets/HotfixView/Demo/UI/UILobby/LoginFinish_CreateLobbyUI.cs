

namespace ET
{
	public class LoginFinish_CreateLobbyUI: AEvent<EventType.LoginFinish>
	{
		protected override async ETTask Run(EventType.LoginFinish args)
		{
			UIManagerComponent.Instance.OpenWindow<UILobbyView,Scene>(args.ZoneScene).Coroutine();
		}
	}
}
