

namespace ET
{
	public class LoginFinish_RemoveLoginUI: AEvent<EventType.LoginFinish>
	{
		protected override void Run(EventType.LoginFinish args)
		{
			await UIManagerComponent.Instance.CloseWindow<UILoginView>();
		}
	}
}
