﻿using System;
using UnityEngine;

namespace ET
{
	public class UILobbyView : UIBaseView
    {
        public override string PrefabPath => "UI/UILobby/Prefabs/UILobbyView.prefab";
        public Scene zoneScene;
        public UIButton EnterBtn;
    }
}