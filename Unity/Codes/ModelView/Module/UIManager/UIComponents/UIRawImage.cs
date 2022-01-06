using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
   
    public class UIRawImage: Entity,IAwake
    {
        public string sprite_path;
        public RawImage unity_uiimage;
    }
}
