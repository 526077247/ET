#define ILRuntime

using AssetBundles;
using System;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Linq;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace ET
{
	public class CodeLoader
	{
        static CodeLoader __Instance;
        public static CodeLoader Instance
        {
            get
            {
                if (__Instance == null)
                    __Instance = new CodeLoader();
                return __Instance;
            }
        }

        public Action Update;
        public Action LateUpdate;
        public Action OnApplicationQuit;

		private Assembly assembly;
		
		private Type[] allTypes;

		private CodeLoader()
		{
		}
		
		public void Start()
		{
			switch (Define.CodeMode)
			{
				case Define.CodeMode_Mono:
				{
					Dictionary<string, UnityEngine.Object> dictionary = AssetsBundleHelper.LoadBundle("code.unity3d");
					byte[] assBytes = ((TextAsset)dictionary["Code.dll"]).bytes;
					byte[] pdbBytes = ((TextAsset)dictionary["Code.pdb"]).bytes;
					
					assembly = Assembly.Load(assBytes, pdbBytes);
					this.allTypes = assembly.GetTypes();
					IStaticMethod start = new MonoStaticMethod(assembly, "ET.Entry", "Start");
					start.Run();
					break;
				}
				case Define.CodeMode_ILRuntime:
				{
#if !UNITY_EDITOR
            var ab = AddressablesManager.Instance.SyncLoadAssetBundle("code_assets_all.bundle");
            byte[] assBytes = ((TextAsset)ab.LoadAsset("Assets/AssetsPackage/Code/Code.dll.bytes", typeof(TextAsset))).bytes;
            byte[] pdbBytes = ((TextAsset)ab.LoadAsset("Assets/AssetsPackage/Code/Code.pdb.bytes", typeof(TextAsset))).bytes;
#else
            byte[] assBytes = (AssetDatabase.LoadAssetAtPath("Assets/AssetsPackage/Code/Code.dll.bytes", typeof(TextAsset)) as TextAsset).bytes;
            byte[] pdbBytes = (AssetDatabase.LoadAssetAtPath("Assets/AssetsPackage/Code/Code.pdb.bytes", typeof(TextAsset)) as TextAsset).bytes;
#endif
				
					AppDomain appDomain = new AppDomain();
					MemoryStream assStream = new MemoryStream(assBytes);
					MemoryStream pdbStream = new MemoryStream(pdbBytes);
					appDomain.LoadAssembly(assStream, pdbStream, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());

					ILHelper.InitILRuntime(appDomain);

					this.allTypes = appDomain.LoadedTypes.Values.Select(x => x.ReflectionType).ToArray();
					IStaticMethod start = new ILStaticMethod(appDomain, "ET.Entry", "Start", 0);
					start.Run();
					break;
				}
				case Define.CodeMode_Reload:
				{
					byte[] assBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Data.dll"));
					byte[] pdbBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Data.pdb"));
					
					assembly = Assembly.Load(assBytes, pdbBytes);
					LoadHotfix();
					IStaticMethod start = new MonoStaticMethod(assembly, "ET.Entry", "Start");
					start.Run();
					break;
				}
			}
		}

		// 热重载调用下面三个方法
		// CodeLoader.Instance.LoadHotfix();
		// Game.EventSystem.Add(CodeLoader.Instance.GetTypes());
		// Game.EventSystem.Load();
		public void LoadHotfix()
		{
			byte[] assBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Logic.dll"));
			byte[] pdbBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Logic.pdb"));
#if !UNITY_EDITOR
            ab.Unload(true);
#endif
			
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
	}
}