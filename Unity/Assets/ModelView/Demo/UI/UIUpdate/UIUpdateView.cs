using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    public class UIUpdateView : UIBaseView
    {

        public readonly int BTN_NONE = 0;
        public readonly int BTN_CANCEL = 1;
        public readonly int BTN_CONFIRM = 2;

        public UISlider m_slider;
        public GameObject m_msgBoxView;
        public UIText m_msgBoxViewText;
        public UIButton m_msgBoxViewBtnCancel;
        public UIText m_msgBoxViewBtnCancelText;
        public UIButton m_msgBoxViewBtnConfirm;
        public UIText m_msgBoxViewBtnConfirmText;

        public List<DownLoadInfo> m_needdownloadinfo = null;
        public string m_rescdn_url;
        public float last_progress;

        public override string PrefabPath => "UI/UIUpdate/Prefabs/UIUpdateView.prefab";

    }
}
