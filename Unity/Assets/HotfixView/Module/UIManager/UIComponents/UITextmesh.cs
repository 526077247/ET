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
        TMPro.TMP_Text unity_uitextmesh
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
                    unity_i18ncomp_touched = this.gameObject.GetComponent<I18nTextComponent>();
                }
                return __unity_uitextmesh;
            }
        }
        I18nTextComponent unity_i18ncomp_touched;
        string __text_key;
        object[] keyParams;
        public override bool HasI18N => true;

        public override void OnCreate<T>(T t)
        {
            base.OnCreate();
            __text_key = t as string;
            SetI18NKey(__text_key);
        }

        //当手动修改text的时候，需要将mono的i18textcomponent给禁用掉
        void __DisableI18Component(bool enable = false)
        {
            if (unity_i18ncomp_touched != null)
            {
                unity_i18ncomp_touched.enabled = enable;
                if(!enable)
                    Log.Warning($"组件{this.gameObject.name}, text在Lua层进行了修改，所以应该去掉去预设里面的I18N组件，否则会被覆盖");
            }
        }

        public string GetText()
        {
            return unity_uitextmesh.text;
        }

        public void SetText(string text)
        {
            __DisableI18Component();
            __text_key = null;
            unity_uitextmesh.text = text;
        }
        public void SetI18NKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                SetText("");
                return;
            }
            __DisableI18Component();
            __text_key = key;
            SetI18NText(null);
        }
        public void SetI18NKey(string key,params object[] paras)
        {
            if (string.IsNullOrEmpty(key))
            {
                SetText("");
                return;
            }
            __DisableI18Component();
            __text_key = key;
            SetI18NText(paras);
        }

        public void SetI18NText(params object[] paras)
        {
            if (string.IsNullOrEmpty(__text_key))
            {
                Log.Error("there is not key ");
            }
            else
            {
                __DisableI18Component();
                keyParams = paras;
                if(I18nComponent.Instance.I18NTryGetText(__text_key, out var text)&& paras != null)
                    text =string.Format(text, paras);
                unity_uitextmesh.text = text;
            }
        }

        public override void OnLanguageChange(object sender = null, EventArgs args = null)
        {
            base.OnLanguageChange(sender, args);
            if (__text_key!=null)
                I18nComponent.Instance.I18NGetParamText(__text_key, keyParams);
        }

        public void SetTextColor(Color color)
        {
            unity_uitextmesh.color = color;
        }

        public void SetTextWithColor(string text,string colorstr)
        {
            if (string.IsNullOrEmpty(colorstr))
                SetText(text);
            else
                SetText($"<color={colorstr}>{text}</color>");
        }
        public override void Dispose()
        {
            base.Dispose();
            __unity_uitextmesh = null;
            unity_i18ncomp_touched = null;
            keyParams = null;
        }
        
    }
}
