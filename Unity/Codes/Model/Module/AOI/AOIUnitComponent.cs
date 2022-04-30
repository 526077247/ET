using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    /// <summary>1
    /// 使用时注意 平面是x,z；竖直方向是y
    /// </summary>
    [ChildType(typeof(AOITriggerComponent))]
    public class AOIUnitComponent:Entity,IAwake<Vector3,Quaternion,UnitType,int>,IAwake<Vector3,Quaternion,UnitType>,IDestroy
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public AOISceneComponent Scene;
        public UnitType Type { get; set; }
        private AOIGrid grid;
        public AOIGrid Grid
        {
            get =>grid;
            set
            {
                EventSystem.Instance.Publish(new EventType.ChangeGrid()
                {
                    Unit = this,
                    NewGrid = value,
                    OldGrid = grid
                });
                grid = value;
            }
        }

        public int Range;
        public readonly List<AOITriggerComponent> SphereTriggers = new List<AOITriggerComponent>();//自己的触发器
        // public float MaxTriggerRadius=0;
        public AOITriggerComponent Collider
        {
            get
            {
                for (int i = 0; i < SphereTriggers.Count; i++)
                {
                    if (SphereTriggers[i].IsCollider) 
                        return SphereTriggers[i];
                }

                return null;
            }
        }
    }
}
