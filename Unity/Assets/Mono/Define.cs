namespace ET
{
	public static class Define
	{
#if UNITY_EDITOR
		public static readonly bool Debug = true;
#else
        public static readonly bool Debug = false;
#endif
		public static readonly int DesignScreen_Width = 1366;
		public static readonly int DesignScreen_Height = 768;

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