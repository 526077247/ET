using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    public class UIText : UIBaseComponent
    {
        Text __unity_uitext;
        Text unity_uitext
        {
            get
            {
                if (__unity_uitext == null)
                {
                    __unity_uitext = this.gameObject.GetComponent<Text>();
                    if (__unity_uitext == null)
                    {
                        __unity_uitext = this.gameObject.AddComponent<Text>();
                        Log.Info($"���UI�����UITextmeshʱ������{this.gameObject.name}��û���ҵ�Text���");
                    }
                    unity_i18ncomp_touched = this.gameObject.GetComponent<I18nTextComponent>();
                }
                return __unity_uitext;
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

        //���ֶ��޸�text��ʱ����Ҫ��mono��i18textcomponent�����õ�
        void __DisableI18Component(bool enable = false)
        {
            if (unity_i18ncomp_touched != null)
            {
                unity_i18ncomp_touched.enabled = enable;
                if (!enable)
                    Log.Warning($"���{this.gameObject.name}, text��Lua��������޸ģ�����Ӧ��ȥ��ȥԤ�������I18N���������ᱻ����");
            }
        }

        public string GetText()
        {
            return unity_uitext.text;
        }

        public void SetText(string text)
        {
            __DisableI18Component();
            __text_key = null;
            unity_uitext.text = text;
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
        public void SetI18NKey(string key, params object[] paras)
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
                if (I18nComponent.Instance.I18NTryGetText(__text_key, out var text) && paras != null)
                    text = string.Format(text, paras);
                unity_uitext.text = text;
            }
        }

        public override void OnLanguageChange(object sender = null,EventArgs args = null)
        {
            base.OnLanguageChange(sender,args);
            if (__text_key != null)
                I18nComponent.Instance.I18NGetParamText(__text_key, keyParams);
        }

        public void SetTextColor(Color color)
        {
            unity_uitext.color = color;
        }

        public void SetTextWithColor(string text, string colorstr)
        {
            if (string.IsNullOrEmpty(colorstr))
                SetText(text);
            else
                SetText($"<color={colorstr}>{text}</color>");
        }
        public override void Dispose()
        {
            base.Dispose();
            __unity_uitext = null;
            unity_i18ncomp_touched = null;
            keyParams = null;
        }

    }
}
