using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UITextmesh = TMPro.TMP_Text;
namespace ET
{
    public abstract class BigCommonWin : UIBaseView
    {
        public UIPointerClick mask;
        public Button exit;
        public Transform content;
        public UITextmesh title;
        public UIDrag UIDrag;

        Transform window;
        Vector3 StartPos;
        Vector3 BeginDragPos;

        public override void OnCreate()
        {
            base.OnCreate();

            mask = transform.Find("mask").GetComponent<UIPointerClick>();
            mask.onClick.AddListener(Close);
            exit = transform.Find("window/ui_ExitBtn").GetComponent<Button>();
            exit.onClick.AddListener(Close);
            content = transform.Find("window/ui_ExitBtn");
            title = transform.Find("window/topName").GetComponent<UITextmesh>();
            UIDrag = transform.Find("window").GetComponent<UIDrag>();
            window = transform.Find("window");
            UIDrag.onBeginDrag.AddListener(OnBeginDrag);
            UIDrag.onDrag.AddListener(OnDrag);
            UIDrag.onEndDrag.AddListener(OnEndDrag);
        }


        public virtual void Close()
        {
            CloseSelf();
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
