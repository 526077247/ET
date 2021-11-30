using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ET
{
    public class UISlider: UIBaseContainer
    {
        public Slider unity_uislider;
        public UnityAction<float> __onValueChanged;
        public bool isWholeNumbers;
        public object[] value_list;
    }
}
