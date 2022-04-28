using System;
namespace ET
{
    public class ZhuiZhuAimComponent:Entity,IAwake<Unit,Action>
    {
        public Unit Aim;
        public Action OnArrived;
    }
}