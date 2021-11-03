using System;
using System.Collections.Generic;
using System.Net;
using TMPro;
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
		public InputField password;
		public InputField account;
		public TMP_InputField ipaddr;
		public UIButton registerBtn;
		public List<Button> btns;
		Scene scene;

        public override string PrefabPath => "UI/UILogin/Prefabs/UILoginView.prefab";

        public override void OnCreate()
        {
			base.OnCreate();
			loginBtn = this.AddComponent<UIButton>("Panel/LoginBtn");
			registerBtn = this.AddComponent<UIButton>("Panel/RegisterBtn");
			loginBtn.SetOnClick(OnLogin);
			account = transform.Find("Panel/Account").GetComponent<InputField>();
			password = transform.Find("Panel/Password").GetComponent<InputField>();
			ipaddr = transform.Find("Panel/GM/InputField").GetComponent<TMP_InputField>();
			
			Transform settings = transform.Find("Panel/GM/Setting");
			btns = new List<Button>();
            for (int i = 0; i < settings.childCount; i++)
            {
				var trans = settings.GetChild(i);
				btns.Add(trans.GetComponent<Button>());
				btns[i].onClick.AddListener(()=> {
					OnBtnClick(trans.name);
				});
			}
		}

		public override void OnEnable<T>(T scene)
		{
			base.OnEnable();
			this.scene = scene as Scene;
			ipaddr.text = ServerConfigManagerComponent.Instance.GetCurConfig().iplist[0];
			account.text = PlayerPrefs.GetString(CacheKeys.Account, "");
			password.text = PlayerPrefs.GetString(CacheKeys.Password,"" );
		}
		public async void OnLogin()
		{
			loginBtn.SetInteractable(false);
			GlobalComponent.Instance.Account = account.text;
			PlayerPrefs.SetString(CacheKeys.Account, account.text);
			PlayerPrefs.SetString(CacheKeys.Password, password.text);
			await LoginHelper.Login(scene, ipaddr.text, account.text, password.text);
			loginBtn.SetInteractable(true);
		}
		public void OnBtnClick(string name)
        {
			ipaddr.text = ServerConfigManagerComponent.Instance.ChangeEnv(name.ToLower()).iplist[0];
		}
	}
}
