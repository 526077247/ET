using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	[UISystem]
	[FriendClass(typeof(UIGuidanceView))]
	public class UIGuidanceViewOnCreateSystem : OnCreateSystem<UIGuidanceView>
	{

		public override void OnCreate(UIGuidanceView self)
		{
			self.CircleMask = self.AddUIComponent<UICircleMaskControl>("CircleMask");
			self.RectMask = self.AddUIComponent<UIRectMaskControl>("RectMask");
			self.Mask = self.GetTransform().Find("Mask").GetComponent<PointerMask>();
		}

	}
	[ObjectSystem]
	[FriendClass(typeof(UIGuidanceView))]
	public class UIGuidanceViewLoadSystem : LoadSystem<UIGuidanceView>
	{

		public override void Load(UIGuidanceView self)
		{

		}

	}
	[UISystem]
	[FriendClass(typeof(UIGuidanceView))]
	public class UIGuidanceViewOnEnableSystem : OnEnableSystem<UIGuidanceView,GameObject,int>
	{

		public override void OnEnable(UIGuidanceView self,GameObject obj,int type)
		{
			self.CurCanvas = self.GetGameObject().GetComponentInParent<Canvas>();

			self.Mask.Target = obj;
			OnEnableAsync(self,obj,type).Coroutine();
		}

		static async ETTask OnEnableAsync(UIGuidanceView self,GameObject obj,int type)
		{
			self.CircleMask.SetActive(false);
			self.RectMask.SetActive(false);
			await TimerComponent.Instance.WaitAsync(1);
			if (type == GuidanceGameObejctType.Rect)
			{
				self.RectMask.SetActive(true,obj.GetComponent<RectTransform>(),self.CurCanvas);
			}
			else if (type == GuidanceGameObejctType.Circle)
			{
				self.CircleMask.SetActive(true, obj.GetComponent<RectTransform>(), self.CurCanvas);
			}
			else
			{
				Log.Error("未处理的类型 Guidance Type ="+type);
			}
		}

	}
	[FriendClass(typeof(UIGuidanceView))]
	public static class UIGuidanceViewSystem
	{

	}

}
