using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ET
{
    public class I18nComponent : Entity
    {
        public static I18nComponent Instance;
        //”Ô—‘¿‡–Õ√∂æŸ
        public enum LangType
        {
            Chinese,
            English,
        }
        public LangType curLangType;
        public readonly Dictionary<int, I18nText> i18nTextDic = new Dictionary<int, I18nText>();
        public readonly Dictionary<string, I18nText> i18nTextKeyDic = new Dictionary<string, I18nText>();
    }

}