using System;
namespace ET
{
    public class SceneLoadComponent:Entity,IAwake,IDestroy
    {
        //场景配置
        public ListComponent<ETTask> PreLoadTask;
        public int Total;
        public int FinishCount;
        public Action<float> ProgressCallback;

        public ListComponent<string> Paths;
        public ListComponent<int> Types;
        public DictionaryComponent<string, int> ObjCount;
        public static class LoadType
        {
            public const int GameObject = 0;
            public const int Image = 1;
            public const int Material = 2;
        }
    }
}