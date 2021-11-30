using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    public class UIUpdateView : UIBaseContainer
    {
        public readonly int BTN_CANCEL = 1;
        public readonly int BTN_CONFIRM = 2;

        public UISlider m_slider;

        public UIMsgBoxWin.MsgBoxPara para { get; private set; } = new UIMsgBoxWin.MsgBoxPara();

        public List<DownLoadInfo> m_needdownloadinfo = null;
        public string m_rescdn_url;
        public float last_progress;
        public long download_size;
        public int overCount;
        public static string PrefabPath => "UI/UIUpdate/Prefabs/UIUpdateView.prefab";

    }
}
