using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public static class UIInputTextmeshSystem
    {
        
        public static string GetText(this UIInputTextmesh self)
        {
            return self.unity_uiinput.text;
        }

        public static void SetText(this UIInputTextmesh self,string text)
        {
            self.unity_uiinput.text = text;
        }

    }
}
