using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UITextmesh = TMPro.TMP_Text;
namespace ET
{
    public class ConfirmPara
    {
        public string Content;
        public string ConfirmText;
        public string CancelText;
        public Action ConfirmCallback;
        public Action CancelCallback;
        public bool Compulsive;//是否要强制选择
    }
    public class ConfirmWin : UIBaseView
    {
        public UIPointerClick mask;
        public Button confirm;
        public Button cancel;
        public UITextmesh content;
        public UITextmesh confirmText;
        public UITextmesh cancelText;
        public UIDrag UIDrag;

        ConfirmPara confirmPara;
        Transform window;
        Vector3 StartPos;
        Vector3 BeginDragPos;

        public override string PrefabPath => "UI/UICommon/Prefabs/ConfirmWin.prefab";

        public ConfirmWin()
        {

        }

        public override void OnCreate()
        {
            base.OnCreate();
            mask = transform.Find("mask").GetComponent<UIPointerClick>();
            mask.onClick.AddListener(ClickMask);
            confirm = transform.Find("window/btns/ConfirmBtn").GetComponent<Button>();
            confirm.onClick.AddListener(OnConfirm);
            cancel = transform.Find("window/btns/CancelBtn").GetComponent<Button>();
            cancel.onClick.AddListener(OnCancel);
            content = AddComponent<UITextmesh>("window/ui_Content");
            confirmText = AddComponent<UITextmesh>("window/btns/ConfirmBtn/Text");
            cancelText = AddComponent<UITextmesh>("window/btns/CancelBtn/Text");
            window = transform.Find("window");
            UIDrag = transform.Find("window").GetComponent<UIDrag>();
            UIDrag.onBeginDrag.AddListener(OnBeginDrag);
            UIDrag.onDrag.AddListener(OnDrag);
            UIDrag.onEndDrag.AddListener(OnEndDrag);
        }
        public override void OnEnable<T>(T para)
        {
            base.OnEnable(para);
            confirmPara = para as ConfirmPara;
            if (confirmPara != null)
            {
                content.SetText(confirmPara.Content);
                confirmText.SetText(confirmPara.ConfirmText);
                cancelText.SetText(confirmPara.CancelText);
            }   
        }

        public virtual void Close()
        {
            confirmPara = null;
            CloseSelf();
        }

        public void ClickMask()
        {
            if (confirmPara != null && confirmPara.Compulsive) return;
            Close();
        }

        public void OnConfirm()
        {
            if (confirmPara != null)
                confirmPara.ConfirmCallback?.Invoke();
            Close();
        }

        public void OnCancel()
        {
            if (confirmPara != null)
                confirmPara.CancelCallback?.Invoke();
            Close();
        }

        public void OnBeginDrag(PointerEventData data)
        {
            StartPos = window.position;
            BeginDragPos = data.position;
        }

        public void OnDrag(PointerEventData data)
        {
            window.position = (new Vector3(data.position.x, data.position.y) - BeginDragPos) * UIManagerComponent.Instance.ScreenSizeflag + StartPos;
        }

        public void OnEndDrag(PointerEventData data)
        {
            StartPos = Vector3.zero;
            BeginDragPos = Vector3.zero;
        }
    }
}
