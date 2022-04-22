using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [ChildType(typeof(AOIGrid))]
    public class AOISceneComponent:Entity,IAwake<int>,IDestroy,IUpdate
    {
        public int gridLen;
        public float halfDiagonal;
    }
    
}