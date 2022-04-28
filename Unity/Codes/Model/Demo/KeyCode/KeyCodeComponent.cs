using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    public class KeyCodeComponent:Entity,IAwake,IDestroy
    {
        public static KeyCodeComponent Instance;
        public int[] Skills { get; set; }
    }
}
