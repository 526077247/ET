using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace ET
{
    public class UIPointerClick : UIBaseComponent
    {

        public UnityAction __onclick;
        PointerClick __unity_pointerclick;

        public PointerClick unity_pointerclick
        {
            get
            {
                if (__unity_pointerclick == null)
                {
                    __unity_pointerclick = this.gameObject.GetComponent<PointerClick>();
                    if (__unity_pointerclick == null)
                    {
                        __unity_pointerclick = this.gameObject.AddComponent<PointerClick>();
                        Log.Info($"添加UI侧组件UIPointerClick时，物体{this.gameObject.name}上没有找到PointerClick组件");
                    }
                }
                return __unity_pointerclick;
            }
        }
        
       
        public override void Dispose()
        {
            base.Dispose();
            __unity_pointerclick = null;
        }

    }
}