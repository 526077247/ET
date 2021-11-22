using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ET
{
    public class UISliderDestorySystem : DestroySystem<UISlider>
    {
        public override void Destroy(UISlider self)
        {
            self.RemoveOnValueChanged();
        }
    }
    public static class UISliderSystem
    {

        public static void SetOnValueChanged(this UISlider self,UnityAction<float> callback)
        {
            self.RemoveOnValueChanged();
            self.__onValueChanged = callback;
            self.unity_uislider.onValueChanged.AddListener(self.__onValueChanged);
        }

        public static void RemoveOnValueChanged(this UISlider self)
        {
            if (self.__onValueChanged != null)
            {
                self.unity_uislider.onValueChanged.RemoveListener(self.__onValueChanged);
                self.__onValueChanged = null;
            }
        }

        public static void SetWholeNumbers(this UISlider self, bool wholeNumbers)
        {
            self.unity_uislider.wholeNumbers = wholeNumbers;
            self.isWholeNumbers = true;
        }

        public static void SetMaxValue(this UISlider self, float value)
        {
            self.unity_uislider.maxValue = value;
        }

        public static void SetMinValue(this UISlider self, float value)
        {
            self.unity_uislider.minValue = value;
        }

        public static void SetValueList(this UISlider self, object[] value_list)
        {
            self.value_list = value_list;
            self.SetWholeNumbers(true);
            self.SetMinValue(0);
            self.SetMaxValue(value_list.Length - 1);
        }

        public static object[] GetValueList(this UISlider self)
        {
            return self.value_list;
        }

        public static object GetValue(this UISlider self)
        {
            if (self.isWholeNumbers)
            {
                var index = (int)self.unity_uislider.value;
                return self.value_list[index];
            }
            else
            {
                return self.unity_uislider.normalizedValue;
            }
        }

        /// <summary>
        /// 设置进度
        /// </summary>
        /// <param name="value">wholeNumbers 时value是ui侧的index</param>
        public static void SetValue(this UISlider self, int value)
        {
            if (self.isWholeNumbers)
                self.unity_uislider.value = value;
            else
                self.unity_uislider.normalizedValue = value;
        }
        /// <summary>
        /// 设置进度
        /// </summary>
        /// <param name="value"></param>
        public static void SetValue(this UISlider self, float value)
        {
            if (!self.isWholeNumbers)
                self.unity_uislider.normalizedValue = value;
            else
            {
                Log.Warning("请先设置WholeNumbers为false");
                self.unity_uislider.value = (int)value;
            }
        }

    }
}
