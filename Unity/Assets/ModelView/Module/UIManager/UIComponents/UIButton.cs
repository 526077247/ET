using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ET
{
    public class UIButton : UIBaseComponent
    {
        public UnityAction __onclick;
        Button __unity_uibutton;
        public bool gray_state;
        Image __unity_uiimage;
        public string sprite_path;

        public Button unity_uibutton
        {
            get
            {
                if (__unity_uibutton == null)
                {
                    __unity_uibutton = this.gameObject.GetComponent<Button>();
                    if (__unity_uibutton == null)
                    {
                        __unity_uibutton = this.gameObject.AddComponent<Button>();
                        Log.Info($"添加UI侧组件UIButton时，物体{this.gameObject.name}上没有找到Button组件");
                    }
                }
                return __unity_uibutton;
            }
        }
        public Image unity_uiimage
        {
            get
            {
                if (__unity_uiimage == null)
                {
                    __unity_uiimage = this.gameObject.GetComponent<Image>();
                }
                return __unity_uiimage;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            __unity_uiimage = null;
            __unity_uibutton = null;
        }
    }
}