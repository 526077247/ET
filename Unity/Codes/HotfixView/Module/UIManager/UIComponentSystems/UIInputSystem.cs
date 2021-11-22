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

        public static string GetText(this UIInput self)
        {
            return self.unity_uiinput.text;
        }

        public static void SetText(this UIInput self,string text)
        {
            self.unity_uiinput.text = text;
        }

    }
}
