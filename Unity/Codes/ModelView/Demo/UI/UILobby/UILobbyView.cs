using System;
using UnityEngine;

namespace ET
{
	public class UILobbyView : Entity,IAwake
    {
        public static string PrefabPath => "UI/UILobby/Prefabs/UILobbyView.prefab";
        public Scene zoneScene;
        public UIButton EnterBtn;
    }
}