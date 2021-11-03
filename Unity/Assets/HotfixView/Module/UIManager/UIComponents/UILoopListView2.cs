using SuperScrollView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ET
{
    public class UILoopListView2: UIBaseComponent
    {
        LoopListView2 __unity_uilooplistview;
        LoopListView2 unity_uilooplistview
        {
            get
            {
                if (__unity_uilooplistview == null)
                {
                    __unity_uilooplistview = this.gameObject.GetComponent<LoopListView2>();
                }
                return __unity_uilooplistview;
            }
        }

        public void InitListView(int itemTotalCount,
            System.Func<LoopListView2, int, LoopListViewItem2> onGetItemByIndex,
            LoopListViewInitParam initParam = null)
        {
            unity_uilooplistview.InitListView(itemTotalCount, onGetItemByIndex, initParam);
        }


        //item是Unity侧的item对象，在这里创建相应的UI对象
        public void AddItemViewComponent<T>(LoopListViewItem2 item) where T : UIBaseComponent
        {
            //保证名字不能相同 不然没法cache
            item.gameObject.name = item.gameObject.name + item.ItemId;
            this.AddComponent<T>(item.gameObject);
        }

        //根据Unity侧item获取UI侧的item
        public T GetUIItemView<T>(LoopListViewItem2 item) where T : UIBaseComponent
        {
            return this.GetComponent<T>(item.gameObject.name);
        }
        //itemCount重设item的数量，resetPos是否刷新当前显示的位置
        public void SetListItemCount(int itemCount, bool resetPos = true)
        {
            unity_uilooplistview.SetListItemCount(itemCount, resetPos);
        }

        //获取当前index对应的item 没有显示的话返回null
        public LoopListViewItem2 GetShownItemByItemIndex(int itemIndex)
        {
            return unity_uilooplistview.GetShownItemByItemIndex(itemIndex);
        }

        public void MovePanelToItemByRowColumn(int itemIndex, float offset)
        {
            unity_uilooplistview.MovePanelToItemIndex(itemIndex, offset);
        }


        public void RefreshAllShownItem()
        {
            unity_uilooplistview.RefreshAllShownItem();
        }


        public void SetOnBeginDragAction(Action callback)
        {
            unity_uilooplistview.mOnBeginDragAction = callback;
        }

        public void SetOnDragingAction(Action callback)
        {
            unity_uilooplistview.mOnDragingAction = callback;
        }

        public void SetOnEndDragAction(Action callback)
        {
            unity_uilooplistview.mOnEndDragAction = callback;
        }

        public override void Dispose()
        {
            unity_uilooplistview.ClearListView();
            __unity_uilooplistview = null;
            base.Dispose();

        }
    }
}
