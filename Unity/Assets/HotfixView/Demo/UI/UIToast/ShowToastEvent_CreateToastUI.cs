

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
            ui.AddComponent<UIBaseComponent, GameObject>("",gameObject);
            gameObject.transform.SetParent(ToastComponent.Instance.root);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            ui.OnCreate();
            ui.OnEnable(Content);
            await TimerComponent.Instance.WaitAsync(seconds*1000);
            ui.OnDestroy();
            GameObjectPoolComponent.Instance.RecycleGameObject(gameObject);
        }

    }
}
