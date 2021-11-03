using System;
using UnityEngine;

namespace ET
{

    public class UILobbyView : UIBaseView
    {
        public override string PrefabPath => "UI/UILobby/Prefabs/UILobbyView.prefab";
        Scene zoneScene;
        UIButton EnterBtn;
        public override void OnCreate()
        {
            base.OnCreate();
            EnterBtn = this.AddComponent<UIButton>("Panel/EnterMap");
            EnterBtn.SetOnClick(OnEnterBtnClick);
        }
        public override void OnEnable<T>(T t)
        {
            base.OnCreate();
            zoneScene = t as Scene;
        }
        void OnEnterBtnClick()
        {
            MapHelper.EnterMapAsync(zoneScene).Coroutine();
        }
    }
}