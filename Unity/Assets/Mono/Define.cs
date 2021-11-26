namespace ET
{
	public static class Define
	{
		public const string BuildOutputDir = "./Temp/Bin/Debug";
#if UNITY_EDITOR
		public static readonly bool Debug = true;
#else
        public static readonly bool Debug = false;
#endif
		public static readonly int DesignScreen_Width = 1366;
		public static readonly int DesignScreen_Height = 768;

		// 1 mono模式 2 ILRuntime模式 3 mono热重载模式
#if UNITY_ANDROID
		public static int CodeMode = 1;
#elif UNITY_IOS
		public static int CodeMode = 2;
#elif UNITY_EDITOR
		public static int CodeMode = 3;
#else // PC测试ILRuntime
		public static int CodeMode = 2;
#endif
		public const int CodeMode_Mono = 1;
		public const int CodeMode_ILRuntime = 2;
		public const int CodeMode_Reload = 3;
		
#if UNITY_EDITOR && !ASYNC
		public static bool IsAsync = false;
#else
        public static bool IsAsync = true;
#endif
		
#if UNITY_EDITOR
		public static bool IsEditor = true;
#else
        public static bool IsEditor = false;
#endif
	}
}