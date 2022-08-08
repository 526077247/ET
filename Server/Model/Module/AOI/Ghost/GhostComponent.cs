using System;
using System.Collections.Generic;

namespace ET
{
    [ComponentOf(typeof(AOIUnitComponent))]
    public class GhostComponent :Entity,IAwake
    {
        public Dictionary<int,int> AreaIds { get; set; }
        public bool IsGoast { get; set; }

        /// <summary>
        /// 需要转发到其他Area的协议
        /// </summary>
        public static readonly Dictionary<Type, Type> MsgMap = new Dictionary<Type, Type>()
        {
            {typeof(M2C_PathfindingResult),typeof(M2M_PathfindingResult)},
            {typeof(M2C_Stop),typeof(M2M_Stop)},
            
        };
    }
}