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
        Slider unity_uislider
        {
            get
            {
                if (__unity_uislider == null)
                {
                    __unity_uislider = this.gameObject.GetComponent<Slider>();
                    if (__unity_uislider == null)
                        Log.Error($"添加UI侧组件UISlider时，物体{this.gameObject.name}上没有找到Slider组件");
                }
                return __unity_uislider;
            }
        }

        UnityAction<float> __onValueChanged;
        bool isWholeNumbers;
        object[] value_list;
        public override void OnCreate()
        {
            base.OnCreate();
        }

        public void SetOnValueChanged(UnityAction<float> callback)
        {
            RemoveOnValueChanged();
            __onValueChanged = callback;
            unity_uislider.onValueChanged.AddListener(__onValueChanged);
        }

        public void RemoveOnValueChanged()
        {
            if (__onValueChanged != null)
            {
                unity_uislider.onValueChanged.RemoveListener(__onValueChanged);
                __onValueChanged = null;
            }
        }

        public void SetWholeNumbers(bool wholeNumbers)
        {
            unity_uislider.wholeNumbers = wholeNumbers;
            isWholeNumbers = true;
        }

        public void SetMaxValue(float value)
        {
            unity_uislider.maxValue = value;
        }

        public void SetMinValue(float value)
        {
            unity_uislider.minValue = value;
        }

        public void SetValueList(object[] value_list)
        {
            this.value_list = value_list;
            SetWholeNumbers(true);
            SetMinValue(0);
            SetMaxValue(value_list.Length - 1);
        }

        public object[] GetValueList()
        {
            return value_list;
        }

        public object GetValue()
        {
            if (isWholeNumbers)
            {
                var index = (int)unity_uislider.value;
                return value_list[index];
            }
            else
            {
                return unity_uislider.normalizedValue;
            }
        }

        /// <summary>
        /// 设置进度
        /// </summary>
        /// <param name="value">wholeNumbers 时value是ui侧的index</param>
        public void SetValue(int value)
        {
            if (isWholeNumbers)
                unity_uislider.value = value;
            else
                unity_uislider.normalizedValue = value;
        }
        /// <summary>
        /// 设置进度
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(float value)
        {
            if (!isWholeNumbers)
                unity_uislider.normalizedValue = value;
            else
            {
                Log.Warning("请先设置WholeNumbers为false");
                unity_uislider.value = (int)value;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            RemoveOnValueChanged();
            __unity_uislider = null;
        }
    }
}
