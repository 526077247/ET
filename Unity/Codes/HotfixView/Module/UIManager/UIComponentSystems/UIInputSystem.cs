using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace ET
{
    public static class UIInputSystem
    {
        static void ActivatingComponent(this UIInput self)
        {
            if (self.unity_uiinput == null)
            {
                self.unity_uiinput = self.GetGameObject().GetComponent<InputField>();
                if (self.unity_uiinput == null)
                {
                    Log.Error($"添加UI侧组件UIInput时，物体{self.GetGameObject().name}上没有找到InputField组件");
                }
            }
        }
        public static string GetText(this UIInput self)
        {
            self.ActivatingComponent();
            return self.unity_uiinput.text;
        }

        public static void SetText(this UIInput self,string text)
        {
            self.ActivatingComponent();
            self.unity_uiinput.text = text;
        }

    }
}
