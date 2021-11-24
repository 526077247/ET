#define ILRuntime

using AssetBundles;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if ILRuntime
using System.Linq;
#endif

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
		
		private Type[] hotfixTypes;

		private CodeLoader()
		{
		}
		
		public void Start()
		{
#if !UNITY_EDITOR
            var ab = AddressablesManager.Instance.SyncLoadAssetBundle("code_assets_all.bundle");
            byte[] assBytes = ((TextAsset)ab.LoadAsset("Assets/AssetsPackage/Code/Code.dll.bytes", typeof(TextAsset))).bytes;
            byte[] pdbBytes = ((TextAsset)ab.LoadAsset("Assets/AssetsPackage/Code/Code.pdb.bytes", typeof(TextAsset))).bytes;
#else
            byte[] assBytes = (AssetDatabase.LoadAssetAtPath("Assets/AssetsPackage/Code/Code.dll.bytes", typeof(TextAsset)) as TextAsset).bytes;
            byte[] pdbBytes = (AssetDatabase.LoadAssetAtPath("Assets/AssetsPackage/Code/Code.pdb.bytes", typeof(TextAsset)) as TextAsset).bytes;
#endif
#if ILRuntime
			ILRuntime.Runtime.Enviorment.AppDomain appDomain = new ILRuntime.Runtime.Enviorment.AppDomain();
			System.IO.MemoryStream assStream = new System.IO.MemoryStream(assBytes);
			System.IO.MemoryStream pdbStream = new System.IO.MemoryStream(pdbBytes);
			appDomain.LoadAssembly(assStream, pdbStream, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
			
			ILHelper.InitILRuntime(appDomain);
			
			this.hotfixTypes = Type.EmptyTypes;
			this.hotfixTypes = appDomain.LoadedTypes.Values.Select(x => x.ReflectionType).ToArray();
			IStaticMethod start = new ILStaticMethod(appDomain, "ET.Entry", "Start", 0);
#else

			System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(assBytes, pdbBytes);
			hotfixTypes = assembly.GetTypes();
			IStaticMethod start = new MonoStaticMethod(assembly, "ET.Entry", "Start");
#endif
#if !UNITY_EDITOR
            ab.Unload(true);
#endif
			
			start.Run();
		}

        public void Start()
        {
            this.start.Run();
        }

        public Type[] GetHotfixTypes()
        {
            return this.hotfixTypes;
        }
    }
}