using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ET
{
    public class UISlider: UIBaseComponent
    {
        Slider __unity_uislider;
        public Slider unity_uislider
        {
            get
            {
                if (__unity_uislider == null)
                {
                    __unity_uislider = this.gameObject.GetComponent<Slider>();
                    if (__unity_uislider == null)
                    {
                        __unity_uislider = this.gameObject.AddComponent<Slider>();
                        Log.Info($"添加UI侧组件UISlider时，物体{this.gameObject.name}上没有找到Slider组件");
                    }
                }
                return __unity_uislider;
            }
        }

        public UnityAction<float> __onValueChanged;
        public bool isWholeNumbers;
        public object[] value_list;

        public override void Dispose()
        {
            base.Dispose();
            __unity_uislider = null;
        }
    }
}
