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

    }
}