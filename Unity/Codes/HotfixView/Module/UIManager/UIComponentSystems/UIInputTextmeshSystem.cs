using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public static class UIInputTextmeshSystem
    {
        static void ActivatingComponent(this UIInputTextmesh self)
        {
            if (self.unity_uiinput == null)
            {
                self.unity_uiinput = self.GetGameObject().GetComponent<TMPro.TMP_InputField>();
                if (self.unity_uiinput == null)
                {
                    Log.Info($"添加UI侧组件UIInputTextmesh时，物体{self.GetGameObject().name}上没有找到TMPro.TMP_InputField组件");
                }
            }
        }
        public static string GetText(this UIInputTextmesh self)
        {
            self.ActivatingComponent();
            return self.unity_uiinput.text;
        }

        public static void SetText(this UIInputTextmesh self,string text)
        {
            self.ActivatingComponent();
            self.unity_uiinput.text = text;
        }

    }
}
