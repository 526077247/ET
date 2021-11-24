using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace ET
{
	public class UIMsgBoxWin : UIBaseView
	{
		public override string PrefabPath => "UI/UIUpdate/Prefabs/UIMsgBoxWin.prefab";
		public UIText Text;
		public UIButton btn_cancel;
		public UIText CancelText;
		public UIButton btn_confirm;
		public UIText ConfirmText;
		 
		public class MsgBoxPara
        {
			public string Content;
			public string CancelText;
			public string ConfirmText;
			public Action CancelCallback;
			public Action ConfirmCallback;
		}
	}
}
