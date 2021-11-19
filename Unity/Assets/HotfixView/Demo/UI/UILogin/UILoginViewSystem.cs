using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace ET
{
	[UISystem]
	public class UILoginViewOnCreateSystem : OnCreateSystem<UILoginView>
	{
		public override void OnCreate(UILoginView self)
		{
			self.loginBtn = self.AddComponent<UIButton>("Panel/LoginBtn");
			self.registerBtn = self.AddComponent<UIButton>("Panel/RegisterBtn");
			self.loginBtn.SetOnClick(self.OnLogin);
			self.account = self.AddComponent<UIInput>("Panel/Account");
			self.password = self.AddComponent<UIInput>("Panel/Password");
			self.ipaddr = self.AddComponent<UIInputTextmesh>("Panel/GM/InputField");

			var settings = self.AddComponent<UIBaseContainer>("Panel/GM/Setting");
			self.btns = new List<UIButton>();
			for (int i = 0; i < 2; i++)
			{
				string name = "Setting" + (i + 1);
				var btn = settings.AddComponent<UIButton>(name);
				btn.SetOnClick(() =>
				{
					self.OnBtnClick(name);
				});
				self.btns.Add(btn);
			}
		}
	}
	[UISystem]
	public class UILoginViewOnEnableSystem : OnEnableSystem<UILoginView, Scene>
	{
		public override void OnEnable(UILoginView self, Scene scene)
		{
			self.scene = scene;
			self.ipaddr.SetText(ServerConfigManagerComponent.Instance.GetCurConfig().iplist[0]);
			self.account.SetText(PlayerPrefs.GetString(CacheKeys.Account, ""));
			self.password.SetText(PlayerPrefs.GetString(CacheKeys.Password, ""));
		}
	}
	public static class UILoginViewSystem
	{
		
		public static async void OnLogin(this UILoginView self)
		{
			self.loginBtn.SetInteractable(false);
			GlobalComponent.Instance.Account = self.account.GetText();
			PlayerPrefs.SetString(CacheKeys.Account, self.account.GetText());
			PlayerPrefs.SetString(CacheKeys.Password, self.password.GetText());
			await LoginHelper.Login(self.scene, self.ipaddr.GetText(), self.account.GetText(), self.password.GetText());
			self.loginBtn.SetInteractable(true);
		}
		public static void OnBtnClick(this UILoginView self,string name)
        {
			self.ipaddr.SetText(ServerConfigManagerComponent.Instance.ChangeEnv(name.ToLower()).iplist[0]);
		}
	}
}
