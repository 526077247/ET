using AssetBundles;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace ET
{
	public interface IEntry
	{
		void Start();
		void Update();
		void LateUpdate();
		void OnApplicationQuit();
	}

	public class Init : MonoBehaviour
	{
		private IEntry entry;

		private void Awake()
		{
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			//先初始化AssetBundleMgr, 必须在Addressable系统初始化之前
			sw.Start();
			AssetBundleMgr.GetInstance().InitBuildInAssetBundleHashInfo();
			sw.Stop();
			Debug.Log(string.Format("InitBuildInAssetBundleHashInfo use {0}ms", sw.ElapsedMilliseconds));
			sw.Reset();

			sw.Start();
			AssetBundleConfig.Instance.SyncLoadGlobalAssetBundle();
			sw.Stop();
			Debug.Log(string.Format("SyncLoadGlobalAssetBundle use {0}ms", sw.ElapsedMilliseconds));
			sw.Reset();

			//先设置remote_cdn_url 
			sw.Start();
			AssetBundleMgr.GetInstance().SetAddressableRemoteResCdnUrl(AssetBundleConfig.Instance.remote_cdn_url);
			sw.Stop();
			Debug.Log(string.Format("SetAddressableRemoteResCdnUrl use {0}ms", sw.ElapsedMilliseconds));
			sw.Reset();

			InitUnitySetting();
			DontDestroyOnLoad(gameObject);

			Assembly modelAssembly = null;
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				string assemblyName = assembly.FullName;
				if (!assemblyName.Contains("Unity.ModelView"))
				{
					continue;
				}
				modelAssembly = assembly;
				break;
			}

			Type initType = modelAssembly.GetType("ET.Entry");
			this.entry = Activator.CreateInstance(initType) as IEntry;
		}

		private void Start()
		{
			this.entry.Start();
		}

		private void Update()
		{
			this.entry.Update();
		}

		private void LateUpdate()
		{
			this.entry.LateUpdate();
		}

		private void OnApplicationQuit()
		{
			this.entry.OnApplicationQuit();
		}

		// 一些unity的设置项目
		void InitUnitySetting()
		{
			Input.multiTouchEnabled = false;
			//设置帧率
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 60;
		}
	}
}