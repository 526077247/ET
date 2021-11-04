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
			UIBaseView view = target.GetComponent(target.ViewType) as UIBaseView;
			var obj = view?.gameObject;
			if (obj)
			{
				if (GameObjectPoolComponent.Instance == null)
					GameObject.Destroy(obj);
				else
					GameObjectPoolComponent.Instance.RecycleGameObject(obj);
			}
			view?.OnDestroy();
		}
	}
}
