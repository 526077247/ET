using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
   
    public class UIRawImage: UIBaseComponent
    {
        public string sprite_path;
        RawImage __unity_uiimage;
        public RawImage unity_uiimage
        {
            get
            {
                if (__unity_uiimage == null)
                {
                    __unity_uiimage = this.gameObject.GetComponent<RawImage>();
                    if (__unity_uiimage == null)
                    {
                        __unity_uiimage = this.gameObject.AddComponent<RawImage>();
                        Log.Info($"添加UI侧组件UIRawImage时，物体{this.gameObject.name}上没有找到RawImage组件");
                    }
                }
                return __unity_uiimage;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            __unity_uiimage = null;
        }
    }
}
