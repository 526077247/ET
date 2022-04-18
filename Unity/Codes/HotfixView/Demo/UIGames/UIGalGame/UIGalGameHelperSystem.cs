using System;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[UISystem]
	public class UIGalGameHelperOnCreateSystem: OnCreateSystem<UIGalGameHelper>
	{
		public override void OnCreate(UIGalGameHelper self)
		{
			self.inputer = self.GetTransform().GetComponent<KeyListener>();
		}
	}
	[UISystem]
	public class UIGalGameHelperOnEnableSystem: OnEnableSystem<UIGalGameHelper>
	{
		public override void OnEnable(UIGalGameHelper self)
		{
			self.inputer.OnKeyUp += UIGalGameHelperSystem.OnKeyHandler;
		}
	}
	[UISystem]
	public class UIGalGameHelperOnDisableSystem: OnDisableSystem<UIGalGameHelper>
	{
		public override void OnDisable(UIGalGameHelper self)
		{
			self.inputer.OnKeyUp -= UIGalGameHelperSystem.OnKeyHandler;
		}
	}
	public static class UIGalGameHelperSystem
	{

		public static void OnKeyHandler(KeyCode code)
		{
			GalGameEngineComponent.Instance.WaitInput.SetResult(code);
			GalGameEngineComponent.Instance.WaitInput = ETTask<KeyCode>.Create();
		}
	}
}
