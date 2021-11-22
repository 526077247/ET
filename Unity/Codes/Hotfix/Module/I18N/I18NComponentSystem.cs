using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    [ObjectSystem]
    public class I18NComponentAwakeSystem : AwakeSystem<I18NComponent>
    {
        public override void Awake(I18NComponent self)
        {
            I18NComponent.Instance = self;
            var res = I18NConfigCategory.Instance.GetAll();
            self.curLangType = (I18NComponent.LangType)PlayerPrefs.GetInt(CacheKeys.CurLangType, 0);
            self.i18nTextDic = new Dictionary<int, I18NConfig>();
            self.i18nTextKeyDic = new Dictionary<string, I18NConfig>();
            foreach (var item in res)
            {
                self.i18nTextDic.Add(item.Key, item.Value);
                self.i18nTextKeyDic.Add(item.Value.Key, item.Value);
            }
            I18NBridge.Instance.GetValueById = self.I18NGetText;
            I18NBridge.Instance.GetValueByKey = self.I18NGetText;
        }
    }
    [ObjectSystem]
    public class I18NComponentDestroySystem : DestroySystem<I18NComponent>
    {
        public override void Destroy(I18NComponent self)
        {
            I18NComponent.Instance = null;
            self.i18nTextDic.Clear();
            self.i18nTextKeyDic.Clear();
            self.i18nTextDic = null;
            self.i18nTextKeyDic = null;
            I18NBridge.Instance.GetValueById = null;
            I18NBridge.Instance.GetValueByKey = null;
        }
    }
    public static class I18nComponentSystem
    {
        public static string I18NGetText(this I18NComponent self, string key)
        {
            if (!self.i18nTextKeyDic.TryGetValue(key, out var value))
            {
                value = self.i18nTextDic[0];
            }
            switch (self.curLangType)
            {
                case I18NComponent.LangType.Chinese:
                    return value.Chinese;
                case I18NComponent.LangType.English:
                    return value.English;
                default:
                    return value.Chinese;
            }
        }

        public static string I18NGetText(this I18NComponent self, int id)
        {
            if (!self.i18nTextDic.TryGetValue(id, out var value))
            {
                value = self.i18nTextDic[0];
            }
            switch (self.curLangType)
            {
                case I18NComponent.LangType.Chinese:
                    return value.Chinese;
                case I18NComponent.LangType.English:
                    return value.English;
                default:
                    return value.Chinese;
            }
        }
        /// <summary>
        /// 根据key取多语言取不到返回默认id0的
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static string I18NGetParamText(this I18NComponent self, string key, params object[] paras)
        {
            if (!self.i18nTextKeyDic.TryGetValue(key, out var value))
            {
                value = self.i18nTextDic[0];
            }
            string val;
            switch (self.curLangType)
            {
                case I18NComponent.LangType.Chinese:
                    val = value.Chinese;
                    break;
                case I18NComponent.LangType.English:
                    val = value.English;
                    break;
                default:
                    val = value.Chinese;
                    break;
            }
            if (paras != null)
                return string.Format(val, paras);
            else
                return val;
        }
        /// <summary>
        /// 取不到返回key
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool I18NTryGetText(this I18NComponent self, string key, out string result)
        {
            if (!self.i18nTextKeyDic.TryGetValue(key, out var value))
            {
                result = key;
                return false;
            }
            switch (self.curLangType)
            {
                case I18NComponent.LangType.Chinese:
                    result = value.Chinese;
                    return true;
                case I18NComponent.LangType.English:
                    result = value.English;
                    return true;
                default:
                    result = value.Chinese;
                    return true;
            }
        }
        /// <summary>
        /// 切换语言,外部接口
        /// </summary>
        /// <param name="langType"></param>
        public static void SwitchLanguage(this I18NComponent self, I18NComponent.LangType langType)
        {
            //修改当前语言
            PlayerPrefs.SetInt(CacheKeys.CurLangType, (int)langType);
            self.curLangType = langType;
            Messager.Instance.Broadcast(MessagerId.OnLanguageChange);
        }
    }
}
