using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    public class UIImage:UIBaseComponent
    {
        public string sprite_path;
        Image __unity_uiimage;
        public Image unity_uiimage
        {
            get
            {
                if (__unity_uiimage == null)
                {
                    __unity_uiimage = this.gameObject.GetComponent<Image>();
                    if (__unity_uiimage == null)
                    {
                        __unity_uiimage = this.gameObject.AddComponent<Image>();
                        Log.Info($"添加UI侧组件UIImage时，物体{this.gameObject.name}上没有找到Image组件");
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
