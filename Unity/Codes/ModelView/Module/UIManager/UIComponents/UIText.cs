using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    public class UIText : UIBaseComponent
    {
        Text __unity_uitext;
        public Text unity_uitext
        {
            get
            {
                if (__unity_uitext == null)
                {
                    __unity_uitext = this.gameObject.GetComponent<Text>();
                    if (__unity_uitext == null)
                    {
                        __unity_uitext = this.gameObject.AddComponent<Text>();
                        Log.Info($"添加UI侧组件UITextmesh时，物体{this.gameObject.name}上没有找到Text组件");
                    }
                    unity_i18ncomp_touched = this.gameObject.GetComponent<I18NText>();
                }
                return __unity_uitext;
            }
        }
        public I18NText unity_i18ncomp_touched;
        public string __text_key;
        public object[] keyParams;
        public override void Dispose()
        {
            __unity_uitext = null;
            base.Dispose();
        }
    }
}
