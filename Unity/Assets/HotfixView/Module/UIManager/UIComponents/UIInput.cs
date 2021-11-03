﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace ET
{
    public class UIInput: UIBaseComponent
    {
        InputField __unity_uiinput;
        InputField unity_uiinput
        {
            get
            {
                if (__unity_uiinput == null)
                {
                    __unity_uiinput = this.gameObject.GetComponent<InputField>();
                    if (__unity_uiinput == null)
                    {
                        __unity_uiinput = this.gameObject.AddComponent<InputField>();
                        Log.Info($"添加UI侧组件UIInput时，物体{this.gameObject.name}上没有找到InputField组件");
                    }
                }
                return __unity_uiinput;
            }
        }

        public string GetText()
        {
            return unity_uiinput.text;
        }

        public void SetText(string text)
        {
            unity_uiinput.text = text;
        }

        public override void Dispose()
        {
            base.Dispose();
            __unity_uiinput = null;
        }
    }
}
