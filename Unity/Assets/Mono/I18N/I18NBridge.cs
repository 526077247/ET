using System;
using System.Collections.Generic;

public class I18NBridge
{

    public static I18NBridge Instance { get; private set; } = new I18NBridge();

    public Dictionary<string, string> i18nTextKeyDic;

    /// <summary>
    /// 通过key获取多语言文本
    /// </summary>
    /// <param name="key">key</param>
    /// <returns></returns>
    public string GetText(string key)
    {
        return i18nTextKeyDic[key];
    }

}

