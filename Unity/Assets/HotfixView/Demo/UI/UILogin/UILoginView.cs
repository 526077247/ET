using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	public class UILoginView: UIBaseView
	{
		public UILoginView()
        {
		
		}
		public UIButton loginBtn;
		public UIInput password;
		public UIInput account;
		public UIInputTextmesh ipaddr;
		public UIButton registerBtn;
		public List<UIButton> btns;
		Scene scene;

        public override string PrefabPath => "UI/UILogin/Prefabs/UILoginView.prefab";

        public override void OnCreate()
        {
			base.OnCreate();
			loginBtn = this.AddComponent<UIButton>("Panel/LoginBtn");
			registerBtn = this.AddComponent<UIButton>("Panel/RegisterBtn");
			loginBtn.SetOnClick(OnLogin);
			account = AddComponent<UIInput>("Panel/Account");
			password = AddComponent<UIInput>("Panel/Password");
			ipaddr = AddComponent<UIInputTextmesh>("Panel/GM/InputField");
			
			var settings = AddComponent<UIBaseComponent>("Panel/GM/Setting");
			btns = new List<UIButton>();
            for (int i = 0; i < settings.transform.childCount; i++)
            {
				btns.Add(settings.AddComponent<UIButton>(i));
				btns[i].SetOnClick(()=> {
					OnBtnClick(btns[i].transform.name);
				});
			}
		}

		public override void OnEnable<T>(T scene)
		{
			base.OnEnable();
			this.scene = scene as Scene;
			ipaddr.SetText(ServerConfigManagerComponent.Instance.GetCurConfig().iplist[0]);
			account.SetText(PlayerPrefs.GetString(CacheKeys.Account, ""));
			password.SetText(PlayerPrefs.GetString(CacheKeys.Password,"" ));
		}
		public async void OnLogin()
		{
			loginBtn.SetInteractable(false);
			GlobalComponent.Instance.Account = account.GetText();
			PlayerPrefs.SetString(CacheKeys.Account, account.GetText());
			PlayerPrefs.SetString(CacheKeys.Password, password.GetText());
			await LoginHelper.Login(scene, ipaddr.GetText(), account.GetText(), password.GetText());
			loginBtn.SetInteractable(true);
		}
		public void OnBtnClick(string name)
        {
			ipaddr.SetText(ServerConfigManagerComponent.Instance.ChangeEnv(name.ToLower()).iplist[0]);
		}
	}
}
