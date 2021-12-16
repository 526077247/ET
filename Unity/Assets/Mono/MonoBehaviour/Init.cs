using AssetBundles;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;
namespace ET
{
	// 1 mono模式 2 ILRuntime模式 3 mono热重载模式
	public enum CodeMode
	{
		Mono = 1,
		ILRuntime = 2,
		Reload = 3,
	}
	
	public class Init: MonoBehaviour
	{
		public CodeMode CodeMode = CodeMode.Mono;
		
		private void Awake()
		{
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

			//开始热修复
			sw.Start();
			AddressablesManager.Instance.StartInjectFix();
			sw.Stop();
			Debug.Log(string.Format("StartInjectFix use {0}ms", sw.ElapsedMilliseconds));
			sw.Reset();

			InitUnitySetting();
			

#if ENABLE_IL2CPP
			this.CodeMode = CodeMode.ILRuntime;
//#elif UNITY_EDITOR
//			this.CodeMode = CodeMode.Reload;
#endif

			System.AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
			{
				Log.Error(e.ExceptionObject.ToString());
			};
			
			SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
			
			DontDestroyOnLoad(gameObject);

			ETTask.ExceptionHandler += Log.Error;

			Log.ILog = new UnityLogger();

			Options.Instance = new Options();

			CodeLoader.Instance.CodeMode = this.CodeMode;
		}

		private void Start()
		{
			CodeLoader.Instance.Start();
		}

		private void Update()
		{
			CodeLoader.Instance.Update?.Invoke();
			if (CodeLoader.Instance.isReStart)
			{
				Log.Debug("ReStart");
				CodeLoader.Instance.OnApplicationQuit();
				CodeLoader.Instance.Start();
				CodeLoader.Instance.isReStart = false;
			}
		}

		private void LateUpdate()
		{
			CodeLoader.Instance.LateUpdate?.Invoke();
		}

		private void OnApplicationQuit()
		{
			CodeLoader.Instance.OnApplicationQuit();
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