using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using SuperScrollView;
namespace ET
{
	[UISystem]
	public class UILoginViewOnCreateSystem : OnCreateSystem<UILoginView>
	{
		public override void OnCreate(UILoginView self)
		{
			self.loginBtn = self.AddUIComponent<UIButton>("Panel/LoginBtn");
			self.registerBtn = self.AddUIComponent<UIButton>("Panel/RegisterBtn");
			self.loginBtn.SetOnClick(() => { self.OnLogin(); });
			self.registerBtn.SetOnClick(() => { self.OnRegister(); });
			self.account = self.AddUIComponent<UIInput>("Panel/Account");
			self.password = self.AddUIComponent<UIInput>("Panel/Password");
			self.ipaddr = self.AddUIComponent<UIInputTextmesh>("Panel/GM/InputField");
			self.loginBtn.AddUIComponent<UIRedDotComponent, string>("","Test");
			self.settingView = self.AddUIComponent<UILoopListView2>("Panel/GM/Setting");
			self.settingView.InitListView(ServerConfigCategory.Instance.GetAll().Count, (a, b) => { return self.GetItemByIndex(a, b); });
		}
	}
	[UISystem]
	public class UILoginViewOnEnableSystem : OnEnableSystem<UILoginView, Scene>
	{
		public override void OnEnable(UILoginView self, Scene scene)
		{
			self.scene = scene;
			self.ipaddr.SetText(ServerConfigManagerComponent.Instance.GetCurConfig().RealmIp);
			self.account.SetText(PlayerPrefs.GetString(CacheKeys.Account, ""));
			self.password.SetText(PlayerPrefs.GetString(CacheKeys.Password, ""));
		}
	}
	public static class UILoginViewSystem
	{
		
		public static void OnLogin(this UILoginView self)
		{
			self.loginBtn.SetInteractable(false);
			GlobalComponent.Instance.Account = self.account.GetText();
			PlayerPrefs.SetString(CacheKeys.Account, self.account.GetText());
			PlayerPrefs.SetString(CacheKeys.Password, self.password.GetText());
			LoginHelper.Login(self.scene, self.ipaddr.GetText(), self.account.GetText(), self.password.GetText(), () =>
			{
				self.loginBtn.SetInteractable(true);
			}).Coroutine();
		}
		public static void OnBtnClick(this UILoginView self,int id)
        {
			self.ipaddr.SetText(ServerConfigManagerComponent.Instance.ChangeEnv(id).RealmIp);
		}

		public static void OnRegister(this UILoginView self)
		{
			Game.EventSystem.PublishAsync(new UIEventType.ShowToast() { Text = "测试OnRegister" }).Coroutine();
			RedDotComponent.Instance.RefreshRedDotViewCount("Test1", 1);
		}

		public static LoopListViewItem2 GetItemByIndex(this UILoginView self,LoopListView2 listView, int index)
		{
			if (index < 0 || index >= ServerConfigCategory.Instance.GetAll().Count)
				return null;
			var data = ServerConfigCategory.Instance.Get(index+1);//配置表从1开始的
			var item = listView.NewListViewItem("SettingItem");
			if (!item.IsInitHandlerCalled)
			{
				item.IsInitHandlerCalled = true;
				self.settingView.AddItemViewComponent<UISettingItem>(item);
			}
			var uiitemview = self.settingView.GetUIItemView<UISettingItem>(item);
			uiitemview.SetData(data,(id)=>
			{
				self.OnBtnClick(id);
			});
			return item;
		}
	}
}
