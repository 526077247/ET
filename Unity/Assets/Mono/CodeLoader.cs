using AssetBundles;
using System;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Linq;

namespace ET
{
	public class CodeLoader
	{
		public static CodeLoader Instance = new CodeLoader();

		public Action Update;
		public Action LateUpdate;
		public Action OnApplicationQuit;

		private Assembly assembly;
		
		private Type[] allTypes;
		
		private ILRuntime.Runtime.Enviorment.AppDomain appDomain;
		private MemoryStream assStream ;
		private MemoryStream pdbStream ;
		
		public CodeMode CodeMode { get; set; }

		private CodeLoader()
		{
		}
		
		public void Start()
		{
			switch (this.CodeMode)
			{
				case CodeMode.Mono:
				{
#if !UNITY_EDITOR
                    var ab = AddressablesManager.Instance.SyncLoadAssetBundle("code_assets_all.bundle");
                    byte[] assBytes = ((TextAsset)ab.LoadAsset($"Assets/AssetsPackage/Code/Code{AssetBundleConfig.Instance.ResVer}.dll.bytes", typeof(TextAsset))).bytes;
                    byte[] pdbBytes = ((TextAsset)ab.LoadAsset($"Assets/AssetsPackage/Code/Code{AssetBundleConfig.Instance.ResVer}.pdb.bytes", typeof(TextAsset))).bytes;
#else
					byte[] assBytes = (AssetDatabase.LoadAssetAtPath($"Assets/AssetsPackage/Code/Code{AssetBundleConfig.Instance.ResVer}.dll.bytes", typeof(TextAsset)) as TextAsset).bytes;
					byte[] pdbBytes = (AssetDatabase.LoadAssetAtPath($"Assets/AssetsPackage/Code/Code{AssetBundleConfig.Instance.ResVer}.pdb.bytes", typeof(TextAsset)) as TextAsset).bytes;
#endif
					
					assembly = Assembly.Load(assBytes, pdbBytes);
					this.allTypes = assembly.GetTypes();
					IStaticMethod start = new MonoStaticMethod(assembly, "ET.Entry", "Start");
					start.Run();
#if !UNITY_EDITOR
					ab.Unload(true);
#endif
					break;
				}
				case CodeMode.ILRuntime:
				{
#if !UNITY_EDITOR
                    var ab = AddressablesManager.Instance.SyncLoadAssetBundle("code_assets_all.bundle");
                    byte[] assBytes = ((TextAsset)ab.LoadAsset($"Assets/AssetsPackage/Code/Code{AssetBundleConfig.Instance.ResVer}.dll.bytes", typeof(TextAsset))).bytes;
                    byte[] pdbBytes = ((TextAsset)ab.LoadAsset($"Assets/AssetsPackage/Code/Code{AssetBundleConfig.Instance.ResVer}.pdb.bytes", typeof(TextAsset))).bytes;
#else
					byte[] assBytes = (AssetDatabase.LoadAssetAtPath($"Assets/AssetsPackage/Code/Code{AssetBundleConfig.Instance.ResVer}.dll.bytes", typeof(TextAsset)) as TextAsset).bytes;
					byte[] pdbBytes = (AssetDatabase.LoadAssetAtPath($"Assets/AssetsPackage/Code/Code{AssetBundleConfig.Instance.ResVer}.pdb.bytes", typeof(TextAsset)) as TextAsset).bytes;
#endif
					
					appDomain = new ILRuntime.Runtime.Enviorment.AppDomain();
					if(assStream!=null) assStream.Dispose();
					if(pdbStream!=null) pdbStream.Dispose();
					assStream = new MemoryStream(assBytes);
					pdbStream = new MemoryStream(pdbBytes);
					appDomain.LoadAssembly(assStream, pdbStream, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());

					ILHelper.InitILRuntime(appDomain);

					this.allTypes = appDomain.LoadedTypes.Values.Select(x => x.ReflectionType).ToArray();
					IStaticMethod start = new ILStaticMethod(appDomain, "ET.Entry", "Start", 0);
#if !UNITY_EDITOR
					ab.Unload(true);
#endif
					start.Run();
					break;
					
				}
				case CodeMode.Reload:
				{
					byte[] assBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Data.dll"));
					byte[] pdbBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Data.pdb"));
					
					assembly = Assembly.Load(assBytes, pdbBytes);
					this.LoadLogic();
					IStaticMethod start = new MonoStaticMethod(assembly, "ET.Entry", "Start");
					start.Run();
					break;
				}
			}
		}

		// 热重载调用下面三个方法
		// CodeLoader.Instance.LoadLogic();
		// Game.EventSystem.Add(CodeLoader.Instance.GetTypes());
		// Game.EventSystem.Load();
		public void LoadLogic()
		{
			if (this.CodeMode != CodeMode.Reload)
			{
				throw new Exception("CodeMode != Reload!");
			}
			
			// 傻屌Unity在这里搞了个傻逼优化，认为同一个路径的dll，返回的程序集就一样。所以这里每次编译都要随机名字
			string[] logicFiles = Directory.GetFiles(Define.BuildOutputDir, "Logic_*.dll");
			if (logicFiles.Length != 1)
			{
				throw new Exception("Logic dll count != 1");
			}

			string logicName = Path.GetFileNameWithoutExtension(logicFiles[0]);
			byte[] assBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, $"{logicName}.dll"));
			byte[] pdbBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, $"{logicName}.pdb"));

			Assembly hotfixAssembly = Assembly.Load(assBytes, pdbBytes);
			
			List<Type> listType = new List<Type>();
			listType.AddRange(this.assembly.GetTypes());
			listType.AddRange(hotfixAssembly.GetTypes());
			this.allTypes = listType.ToArray();
		}

		public Type[] GetTypes()
		{
			return this.allTypes;
		}

		public bool isReStart = false;
		public void ReStart()
		{
			Log.Debug("ReStart");
			isReStart = true;
		}
	}
}