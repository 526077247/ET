using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    public class UIText : Entity,IAwake
    {
        public Text unity_uitext;
        public I18NText unity_i18ncomp_touched;
        public string __text_key;
        public object[] keyParams;
    }
}
