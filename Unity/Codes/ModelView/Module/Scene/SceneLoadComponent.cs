﻿using System;
namespace ET
{
    public class SceneLoadComponent:Entity
    {
        //场景配置
        public ListComponent<ETTask> PreLoadTask;
        public int Total;
        public int FinishCount;
        public Action<float> ProgressCallback;

    }
}