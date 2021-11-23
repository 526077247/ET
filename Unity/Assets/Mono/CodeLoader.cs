#define ILRuntime1

using AssetBundles;
using System;
using System.Collections.Generic;
using UnityEngine;

#if ILRuntime
using System.Linq;
#endif

namespace ET
{
	public class CodeLoader
	{
		public static CodeLoader Instance = new CodeLoader();

		public Action Update;
		public Action LateUpdate;
		public Action OnApplicationQuit;

		private readonly IStaticMethod start;
		
		private readonly Type[] hotfixTypes;

        private IStaticMethod start;

        private Type[] hotfixTypes;

        public async ETTask Init()
        {
            byte[] assBytes = (await AddressablesManager.Instance.LoadAssetAsync<TextAsset>("Code/Code.dll.bytes")).bytes;
            byte[] pdbBytes = (await AddressablesManager.Instance.LoadAssetAsync<TextAsset>("Code/Code.pdb.bytes")).bytes;

#if ILRuntime
            ILRuntime.Runtime.Enviorment.AppDomain appDomain = new ILRuntime.Runtime.Enviorment.AppDomain();
            System.IO.MemoryStream assStream = new System.IO.MemoryStream(assBytes);
			System.IO.MemoryStream pdbStream = new System.IO.MemoryStream(pdbBytes);
            appDomain.LoadAssembly(assStream, pdbStream, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
            ILHelper.InitILRuntime(appDomain);

            this.hotfixTypes = Type.EmptyTypes;
            this.hotfixTypes = appDomain.LoadedTypes.Values.Select(x => x.ReflectionType).ToArray();
			this.start = new ILStaticMethod(appDomain, "ET.Entry", "Start", 0);
#else

			System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(assBytes, pdbBytes);
			hotfixTypes = assembly.GetTypes();
			this.start = new MonoStaticMethod(assembly, "ET.Entry", "Start");
#endif
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