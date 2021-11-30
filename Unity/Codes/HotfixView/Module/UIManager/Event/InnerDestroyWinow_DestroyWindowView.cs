using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    public class InnerDestroyWinow_DestroyWindowView : AEvent<UIEventType.InnerDestroyWindow>
	{
		protected override async ETTask Run(UIEventType.InnerDestroyWindow args)
		{
			var target = args.target;
			UIBaseContainer view = target.GetComponent(target.ViewType) as UIBaseContainer;
			if (view != null)
			{
				var obj = view.GetGameObject();
				if (obj)
				{
					if (GameObjectPoolComponent.Instance == null)
						GameObject.Destroy(obj);
					else
						GameObjectPoolComponent.Instance.RecycleGameObject(obj);
				}
				UIEventSystem.Instance.OnDestroy(view);
			}
			await ETTask.CompletedTask;
		}
	}
}
