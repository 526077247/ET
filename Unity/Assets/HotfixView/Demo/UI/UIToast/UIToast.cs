using System;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    public class UIToast : UIBaseComponent
	{
		public UITextmesh Text;

		public override void OnCreate()
        {
            base.OnCreate();
            Text = this.AddComponent<UITextmesh>("Content");
		}

        public override void OnEnable<T>(T param1)
        {
            base.OnEnable();
            Text.SetText(param1 as string);
        }
    }
}
