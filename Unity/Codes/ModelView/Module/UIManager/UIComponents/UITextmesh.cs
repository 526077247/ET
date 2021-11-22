using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    public class UITextmesh: UIBaseComponent
    {
        TMPro.TMP_Text __unity_uitextmesh;
        public TMPro.TMP_Text unity_uitextmesh
        {
            get
            {
                if (__unity_uitextmesh == null)
                {
                    __unity_uitextmesh = this.gameObject.GetComponent<TMPro.TMP_Text>();
                    if (__unity_uitextmesh == null)
                    {
                        __unity_uitextmesh = this.gameObject.AddComponent<TMPro.TMP_Text>();
                        Log.Info($"添加UI侧组件UITextmesh时，物体{this.gameObject.name}上没有找到TMPro.TMP_Text组件");
                    }
                    unity_i18ncomp_touched = this.gameObject.GetComponent<I18NText>();
                }
                return __unity_uitextmesh;
            }
        }
        public I18NText unity_i18ncomp_touched;
        public string __text_key;
        public object[] keyParams;

        public override void Dispose()
        {
            base.Dispose();
            __unity_uitextmesh = null;
        }
        
    }
}
