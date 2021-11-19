using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	[UISystem]
	public class UIMsgBoxWinOnCreateSystem : OnCreateSystem<UIMsgBoxWin>
	{

		public override void OnCreate(UIMsgBoxWin self)
		{
			self.Text = self.AddComponent<UIText>("Text");
			self.btn_cancel = self.AddComponent<UIButton>("btn_cancel");
			self.CancelText = self.AddComponent<UIText>("btn_cancel/Text");
			self.btn_confirm = self.AddComponent<UIButton>("btn_confirm");
			self.ConfirmText = self.AddComponent<UIText>("btn_confirm/Text");
		}

	}
	[UISystem]
	public class UIMsgBoxWinOnEnableSystem : OnEnableSystem<UIMsgBoxWin, UIMsgBoxWin.MsgBoxPara>
    {
        public override void OnEnable(UIMsgBoxWin self, UIMsgBoxWin.MsgBoxPara a)
        {
			self.Text.SetText(a.Content);
			self.btn_cancel.SetOnClick(a.CancelCallback);
			self.btn_confirm.SetOnClick(a.ConfirmCallback);
			self.ConfirmText.SetText(a.ConfirmText);
			self.CancelText.SetText(a.CancelText);
		}
    }
	[UISystem]
	public class UIMsgBoxWinOnDisableSystem : OnDisableSystem<UIMsgBoxWin>
	{
		public override void OnDisable(UIMsgBoxWin self)
		{
			self.btn_cancel.RemoveOnClick();
			self.btn_confirm.RemoveOnClick();
		}
	}

}
