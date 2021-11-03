using SuperScrollView;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ET
{
    public class UILoopGridView : UIBaseComponent
    {
        LoopGridView __unity_uiloopgridview;
        LoopGridView unity_uiloopgridview
        {
            get
            {
                if(__unity_uiloopgridview == null)
                {
                    __unity_uiloopgridview = this.gameObject.GetComponent<LoopGridView>();
                }
                return __unity_uiloopgridview;
            }
        }

        public void InitGridView(int itemTotalCount,
                System.Func<LoopGridView, int, int, int, LoopGridViewItem> onGetItemByRowColumn,
                LoopGridViewSettingParam settingParam = null,
                LoopGridViewInitParam initParam = null)
        {
            unity_uiloopgridview.InitGridView(itemTotalCount, onGetItemByRowColumn, settingParam, initParam);
        }

        //item��Unity���item���������ﴴ����Ӧ��UI����
        public void AddItemViewComponent<T>(LoopGridViewItem item) where T : UIBaseComponent
        {
            //��֤���ֲ�����ͬ ��Ȼû��cache
            item.gameObject.name = item.gameObject.name + item.ItemId;
            this.AddComponent<T>(item.gameObject);
        }

        //����Unity��item��ȡUI���item
        public T GetUIItemView<T>(LoopGridViewItem item) where T : UIBaseComponent
        {
            return this.GetComponent<T>(item.gameObject.name);
        }
        //itemCount����item��������resetPos�Ƿ�ˢ�µ�ǰ��ʾ��λ��
        public void SetListItemCount(int itemCount, bool resetPos = true)
        {
            unity_uiloopgridview.SetListItemCount(itemCount, resetPos);
        }

        //��ȡ��ǰindex��Ӧ��item û����ʾ�Ļ�����null
        public LoopGridViewItem GetShownItemByItemIndex(int itemIndex)
        {
            return unity_uiloopgridview.GetShownItemByItemIndex(itemIndex);
        }

        public void MovePanelToItemByRowColumn(int row, int column, int offsetX = 0, int offsetY = 0)
        {
            unity_uiloopgridview.MovePanelToItemByRowColumn(row, column, offsetX, offsetY);
        }


        public void RefreshAllShownItem()
        {
            unity_uiloopgridview.RefreshAllShownItem();
        }

        public void SetItemSize(Vector2 sizeDelta)
        {
            unity_uiloopgridview.SetItemSize(sizeDelta);
        }

        public void SetOnBeginDragAction(Action<PointerEventData> callback)
        {
            unity_uiloopgridview.mOnBeginDragAction = callback;
        }

        public void SetOnDragingAction(Action<PointerEventData> callback)
        {
            unity_uiloopgridview.mOnDragingAction = callback;
        }

        public void SetOnEndDragAction(Action<PointerEventData> callback)
        {
            unity_uiloopgridview.mOnEndDragAction = callback;
        }

        public override void Dispose()
        {
            unity_uiloopgridview.ClearListView();
            __unity_uiloopgridview = null;
            base.Dispose();

        }
    }
}