using UnityEngine;

namespace ET
{
    public class GlobalComponent: Entity, IAwake
    {
        public static GlobalComponent Instance;
        
        public Transform Global;
        public Transform Unit { get; set; }

        public string Account;
        
        public bool ColliderDebug { get; set; }
    }
}