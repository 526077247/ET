using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ET
{
    public class I18NComponent : Entity
    {
        public static I18NComponent Instance;
        //语言类型枚举
        public static class LangType
        {
            public const int Chinese = 0;
            public const int English = 1;
        }
        public int curLangType;
        public Dictionary<string, string> i18nTextKeyDic;
        public Dictionary<long, Entity> I18NEntity;
    }

}