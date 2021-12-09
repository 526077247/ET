using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    public class UITextmesh: Entity
    {
        public TMPro.TMP_Text unity_uitextmesh;

        public I18NText unity_i18ncomp_touched;
        public string __text_key;
        public object[] keyParams;

    }
}
