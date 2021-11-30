using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    [UISystem]
    public class UITextmeshOnCreateSystem : OnCreateSystem<UITextmesh>
    {
        public override void OnCreate(UITextmesh self)
        {
            I18NComponent.Instance.RegisterI18NEntity(self);
        }
    }
    [UISystem]
    public class UITextmeshOnCreateSystem1 : OnCreateSystem<UITextmesh, string>
    {
        public override void OnCreate(UITextmesh self, string key)
        {
            I18NComponent.Instance.RegisterI18NEntity(self);
            self.SetI18NKey(key);
        }

    }
    [UISystem]
    public class UITextmeshOnDestroySystem : OnDestroySystem<UITextmesh>
    {
        public override void OnDestroy(UITextmesh self)
        {
            I18NComponent.Instance.RemoveI18NEntity(self);
            self.unity_i18ncomp_touched = null;
            self.keyParams = null;
        }
    }
    [UISystem]
    public class UITextmeshI18NSystem: I18NSystem<UITextmesh>
    {
        public override void OnLanguageChange(UITextmesh self)
        {
            self.OnLanguageChange();
        }
    }
    public static class UITextmeshSystem
    {
        static void ActivatingComponent(this UITextmesh self)
        {
            if (self.unity_uitextmesh == null)
            {
                self.unity_uitextmesh = self.GetGameObject().GetComponent<TMPro.TMP_Text>();
                if (self.unity_uitextmesh == null)
                {
                    self.unity_uitextmesh = self.GetGameObject().AddComponent<TMPro.TMP_Text>();
                    Log.Info($"添加UI侧组件UITextmesh时，物体{self.GetGameObject().name}上没有找到TMPro.TMP_Text组件");
                }
                self.unity_i18ncomp_touched = self.GetGameObject().GetComponent<I18NText>();
            }
        }
        //当手动修改text的时候，需要将mono的i18textcomponent给禁用掉
        static void __DisableI18Component(this UITextmesh self,bool enable = false)
        {
            self.ActivatingComponent();
            if (self.unity_i18ncomp_touched != null)
            {
                self.unity_i18ncomp_touched.enabled = enable;
                if(!enable)
                    Log.Warning($"组件{self.GetGameObject().name}, text在Lua层进行了修改，所以应该去掉去预设里面的I18N组件，否则会被覆盖");
            }
        }

        public static string GetText(this UITextmesh self)
        {
            self.ActivatingComponent();
            return self.unity_uitextmesh.text;
        }

        public static void SetText(this UITextmesh self, string text)
        {
            self.__DisableI18Component();
            self.__text_key = null;
            self.unity_uitextmesh.text = text;
        }
        public static void SetI18NKey(this UITextmesh self, string key)
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
        public static void SetI18NKey(this UITextmesh self, string key,params object[] paras)
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

        public static void SetI18NText(this UITextmesh self, params object[] paras)
        {
            if (string.IsNullOrEmpty(self.__text_key))
            {
                Log.Error("there is not key ");
            }
            else
            {
                self.__DisableI18Component();
                self.keyParams = paras;
                if(I18NComponent.Instance.I18NTryGetText(self.__text_key, out var text)&& paras != null)
                    text =string.Format(text, paras);
                self.unity_uitextmesh.text = text;
            }
        }

        public static void OnLanguageChange(this UITextmesh self)
        {
            self.ActivatingComponent();
            if (self.__text_key !=null)
                I18NComponent.Instance.I18NGetParamText(self.__text_key, self.keyParams);
        }

        public static void SetTextColor(this UITextmesh self, Color color)
        {
            self.ActivatingComponent();
            self.unity_uitextmesh.color = color;
        }

        public static void SetTextWithColor(this UITextmesh self, string text,string colorstr)
        {
            if (string.IsNullOrEmpty(colorstr))
                self.SetText(text);
            else
                self.SetText($"<color={colorstr}>{text}</color>");
        }
        
    }
}
