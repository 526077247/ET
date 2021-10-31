using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ET;
using System;

public class I18nText : MonoBehaviour
{
    [Tooltip("多语言Text组件")]
    public string key;
    private Text m_Text;
    private TMP_Text m_MeshText;
    void Awake()
    {
        m_Text = GetComponent<Text>();
        m_MeshText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        OnSwitchLanguage();
        Messager.Instance.AddListener<Action>(MessagerId.OnLanguageChange, OnSwitchLanguage);
    }

    private void OnDisable()
    {
        Messager.Instance.RemoveListener<Action>(MessagerId.OnLanguageChange, OnSwitchLanguage);
    }

    private void OnSwitchLanguage()
    {
        if (m_Text != null)
            m_Text.text = I18nBridge.Instance.GetText(key);
        if (m_MeshText != null)
            m_MeshText.text = I18nBridge.Instance.GetText(key);
    }
}

