using System;
using UnityEngine;

namespace ET
{
    [ComponentOf(typeof(Unit))]
    public class GameObjectComponent: Entity, IAwake,IAwake<GameObject>,IAwake<GameObject,Action>, IDestroy
    {
        public GameObject GameObject { get; set; }
        public Action OnDestroyAction;
        public bool IsDebug;

        public ReferenceCollector Collector;
    }
}