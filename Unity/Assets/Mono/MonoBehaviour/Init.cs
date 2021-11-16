using AssetBundles;
using System;
using System.IO;
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
            //测试下载时先把Addressables的缓存清掉，AppData\LocalLow下的缓存都删掉
            //PlayerPrefs.DeleteAll();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			//初始化App版本，解决覆盖安装问题
			sw.Start();
			InitAppVersion();
			sw.Stop();
			Debug.Log(string.Format("InitAppVersion use {0}ms", sw.ElapsedMilliseconds));

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

		void InitAppVersion()
		{

			string outputPath = Path.Combine(Application.persistentDataPath, "version.txt");
			GameUtility.CheckFileAndCreateDirWhenNeeded(outputPath);
			var persistentAppVersion = GameUtility.SafeReadAllText(outputPath);
			if (persistentAppVersion == null)
			{
				GameUtility.SafeWriteAllText(outputPath, Application.version);
				return;
			}
			Debug.Log(string.Format("app_ver = {0}, persistentAppVersion = {1}", Application.version, persistentAppVersion));

			// 如果persistent目录版本app版本低，说明是大版本覆盖安装，清理过时的缓存
			if (!string.IsNullOrEmpty(persistentAppVersion) && VersionCompare.Compare(persistentAppVersion, Application.version) < 0)
			{
				var path = AssetBundleUtility.GetPersistentDataPath();
				GameUtility.SafeDeleteDir(path);
				var path1 = AssetBundleUtility.GetCatalogDataPath();
				GameUtility.SafeDeleteDir(path1);
			}
			GameUtility.SafeWriteAllText(outputPath, Application.version);
		}
	}
}