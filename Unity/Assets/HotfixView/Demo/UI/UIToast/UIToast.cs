using System;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    public class UIToast : UIBaseContainer
	{
		public TextMeshProUGUI Text;

		public override void OnCreate()
        {
            base.OnCreate();
            Text = transform.Find("Content").GetComponent<TextMeshProUGUI>();
		}

        public override void OnEnable<T>(T param1)
        {
            base.OnEnable();
            Text.text = param1 as string;
        }
    }
}
