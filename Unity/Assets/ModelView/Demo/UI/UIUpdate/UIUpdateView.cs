using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using AssetBundles;
using System.Linq;

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
        public Dictionary<int, bool> m_download_finish_index = null;
        public int conn_num = 4;//默认连接数
        public Dictionary<int, PoolConn> pool_conn = null;
        public int finish_count = 0;
        public float last_progress;

        public override string PrefabPath => "UI/UIUpdate/Prefabs/UIUpdateView.prefab";

    }
}
