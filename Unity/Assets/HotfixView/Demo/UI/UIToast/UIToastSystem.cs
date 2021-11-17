﻿using System;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    public class UIToastOnCreateSystem : OnCreateSystem<UIToast>
    {
        public override void OnCreate(UIToast self)
        {
            self.Text = self.AddComponent<UITextmesh>("Content");
        }
    }

    public class UIToastOnEnableSystem : OnEnableSystem<UIToast, string>
    {
        public override void OnEnable(UIToast self, string param1)
        {
            self.Text.SetText(param1);
        }
    }
}