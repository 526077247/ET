using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public class HotFixComponent:Entity
    {
        public static HotFixComponent Instance;
        public string[] Assemblys = new string[]
        {
            "Assembly-CSharp",
            "Unity.Hotfix",
            "Unity.HotfixView",
            "Unity.Model",
            "Unity.ModelView",
        };
    }
}
