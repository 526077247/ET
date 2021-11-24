﻿using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace ET
{
	public class UILoginView: UIBaseView
	{
		public UIButton loginBtn;
		public UIInput password;
		public UIInput account;
		public UIInputTextmesh ipaddr;
		public UIButton registerBtn;
		public List<UIButton> btns;
		public Scene scene;

        public override string PrefabPath => "UI/UILogin/Prefabs/UILoginView.prefab";

	}
}