using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    [UISystem]
    public class UITextOnCreateSystem : OnCreateSystem<UIText>
    {
        public override void OnCreate(UIText self)
        {
            Messager.Instance.AddListener(MessagerId.OnLanguageChange, self.OnLanguageChange);
        }
    }
    [UISystem]
    public class UITextOnCreateSystem1 : OnCreateSystem<UIText, string>
    {
        public override void OnCreate(UIText self, string key)
        {
            Messager.Instance.AddListener(MessagerId.OnLanguageChange, self.OnLanguageChange);
            self.SetI18NKey(key);
        }
    }
    [UISystem]
    public class UITextOnDestroySystem : OnDestroySystem<UIText>
    {
        public override void OnDestroy(UIText self)
        {
            Messager.Instance.RemoveListener(MessagerId.OnLanguageChange, self.OnLanguageChange);
            self.unity_i18ncomp_touched = null;
            self.keyParams = null;

        }
    }

    public static class UITextSystem 
    {

        //当手动修改text的时候，需要将mono的i18textcomponent给禁用掉
        static void __DisableI18Component(this UIText self,bool enable = false)
        {
            if (self.unity_i18ncomp_touched != null)
            {
                self.unity_i18ncomp_touched.enabled = enable;
                if (!enable)
                    Log.Warning($"组件{self.gameObject.name}, text在Lua层进行了修改，所以应该去掉去预设里面的I18N组件，否则会被覆盖");
            }
        }

        public static string GetText(this UIText self)
        {
            return self.unity_uitext.text;
        }

        public static void SetText(this UIText self, string text)
        {
            self.__DisableI18Component();
            self.__text_key = null;
            self.unity_uitext.text = text;
        }
        public static void SetI18NKey(this UIText self, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                self.SetText("");
                return;
            }
            self.__DisableI18Component();
            self.__text_key = key;
            self.SetI18NText(null);
        }
        public static void SetI18NKey(this UIText self, string key, params object[] paras)
        {
            if (string.IsNullOrEmpty(key))
            {
                self.SetText("");
                return;
            }
            self.__DisableI18Component();
            self.__text_key = key;
            self.SetI18NText(paras);
        }

        public static void SetI18NText(this UIText self, params object[] paras)
        {
            if (string.IsNullOrEmpty(self.__text_key))
            {
                Log.Error("there is not key ");
            }
            else
            {
                self.__DisableI18Component();
                self.keyParams = paras;
                if (I18NComponent.Instance.I18NTryGetText(self.__text_key, out var text) && paras != null)
                    text = string.Format(text, paras);
                self.unity_uitext.text = text;
            }
        }

        public static void OnLanguageChange(this UIText self, object sender,EventArgs args)
        {
            if (self.__text_key != null)
                I18NComponent.Instance.I18NGetParamText(self.__text_key, self.keyParams);
        }

        public static void SetTextColor(this UIText self, Color color)
        {
            self.unity_uitext.color = color;
        }

        public static void SetTextWithColor(this UIText self, string text, string colorstr)
        {
            if (string.IsNullOrEmpty(colorstr))
                self.SetText(text);
            else
                self.SetText($"<color={colorstr}>{text}</color>");
        }

    }
}
