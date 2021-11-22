using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ET
{
    public class I18NComponent : Entity
    {
        public static I18NComponent Instance;
        //”Ô—‘¿‡–Õ√∂æŸ
        public enum LangType:byte
        {
            Chinese,
            English,
        }
        public LangType curLangType;
        public Dictionary<int, I18NConfig> i18nTextDic;
        public Dictionary<string, I18NConfig> i18nTextKeyDic;
    }

}