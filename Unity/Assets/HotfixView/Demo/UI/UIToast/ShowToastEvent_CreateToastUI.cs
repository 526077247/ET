

using UnityEngine;

namespace ET
{
	public class ShowToastEvent_CreateToastUI : AEvent<EventType.ShowToast>
	{
		protected override async ETTask Run(EventType.ShowToast args)
		{
			Show(args.Text).Coroutine();
		}

        async ETTask Show(string Content, int seconds = 3)
        {
            GameObject gameObject = await GameObjectPoolComponent.Instance.GetGameObjectAsync("UI/UIToast/Prefabs/UIToast.prefab");
            UIToast ui = ToastComponent.Instance.AddChild<UIToast, GameObject>(gameObject);
            ui.transform.SetParent(ToastComponent.Instance.root);
            ui.transform.localPosition = Vector3.zero;
            ui.transform.localRotation = Quaternion.identity;
            ui.transform.localScale = new Vector3(1, 1, 1);
            UIEventSystem.Instance.OnCreate(ui);
            UIEventSystem.Instance.OnEnable(ui,Content);
            await TimerComponent.Instance.WaitAsync(seconds*1000);
            UIEventSystem.Instance.OnDestroy(ui);
            GameObjectPoolComponent.Instance.RecycleGameObject(gameObject);
        }

    }
}
