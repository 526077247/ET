using System;
using UnityEngine;

namespace ET
{
	public class UILobbyView : UIBaseComponent
    {
        public static string PrefabPath => "UI/UILobby/Prefabs/UILobbyView.prefab";
        public Scene zoneScene;
        public UIButton EnterBtn;
    }
}