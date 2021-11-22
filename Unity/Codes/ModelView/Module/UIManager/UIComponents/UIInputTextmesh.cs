using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace ET
{
    public class UIInputTextmesh:UIBaseComponent
    {
        TMP_InputField __unity_uiinput;
        public TMP_InputField unity_uiinput
        {
            get
            {
                if (__unity_uiinput == null)
                {
                    __unity_uiinput = this.gameObject.GetComponent<TMP_InputField>();
                    if (__unity_uiinput == null)
                    {
                        __unity_uiinput = this.gameObject.AddComponent<TMP_InputField>();
                        Log.Info($"添加UI侧组件UIInputTextmesh时，物体{this.gameObject.name}上没有找到TMP_InputField组件");
                    }
                }
                return __unity_uiinput;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            __unity_uiinput = null;
        }
    }
}
