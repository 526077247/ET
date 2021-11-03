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

        //item是Unity侧的item对象，在这里创建相应的UI对象
        public void AddItemViewComponent<T>(LoopGridViewItem item) where T : UIBaseComponent
        {
            //保证名字不能相同 不然没法cache
            item.gameObject.name = item.gameObject.name + item.ItemId;
            this.AddComponent<T>(item.gameObject);
        }

        //根据Unity侧item获取UI侧的item
        public T GetUIItemView<T>(LoopGridViewItem item) where T : UIBaseComponent
        {
            return this.GetComponent<T>(item.gameObject.name);
        }
        //itemCount重设item的数量，resetPos是否刷新当前显示的位置
        public void SetListItemCount(int itemCount, bool resetPos = true)
        {
            unity_uiloopgridview.SetListItemCount(itemCount, resetPos);
        }

        //获取当前index对应的item 没有显示的话返回null
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