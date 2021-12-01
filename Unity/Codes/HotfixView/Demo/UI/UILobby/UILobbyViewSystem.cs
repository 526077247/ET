using System;
using UnityEngine;

namespace ET
{
    [UISystem]
    public class UILobbyViewOnCreateSystem : OnCreateSystem<UILobbyView>
	{
		public override void OnCreate(UILobbyView self)
		{
            self.EnterBtn = self.AddUIComponent<UIButton>("Panel/EnterMap");
            self.EnterBtn.SetOnClick(()=> { self.OnEnterBtnClick(); });
        }
	}
    [UISystem]
    public class UILobbyViewOnEnableSystem : OnEnableSystem<UILobbyView, Scene>
	{
		public override void OnEnable(UILobbyView self, Scene scene)
		{
            self.zoneScene = scene;
        }
	}
	public static class UILobbyViewSystem 
    {
        
        public static void OnEnterBtnClick(this UILobbyView self)
        {
            MapHelper.EnterMapAsync(self.zoneScene).Coroutine();
        }
    }
}