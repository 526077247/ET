using System;
using UnityEngine;
namespace ET
{

    public static class UICopyGameObjectSystem
    {
        static void ActivatingComponent(this UICopyGameObject self)
        {
            if (self.unity_comp == null)
            {
                self.unity_comp = self.GetGameObject().GetComponent<CopyGameObject>();
                if (self.unity_comp == null)
                {
                    self.unity_comp = self.GetGameObject().AddComponent<CopyGameObject>();
                    Log.Error($"添加UI侧组件UICopyGameObject时，物体{self.GetGameObject().name}上没有找到CopyGameObject组件");
                }
            }
        }

        public static void InitListView(this UICopyGameObject self,int total_count, Action<int, GameObject> ongetitemcallback = null, int? start_sibling_index = null)
        {
            self.ActivatingComponent();
            self.unity_comp.InitListView(total_count, ongetitemcallback, start_sibling_index);
        }

        public static void SetListItemCount(this UICopyGameObject self, int total_count, int? start_sibling_index = null)
        {
            self.unity_comp.SetListItemCount(total_count, start_sibling_index);
        }

        public static void RefreshAllShownItem(this UICopyGameObject self, int? start_sibling_index = null)
        {
            self.unity_comp.RefreshAllShownItem(start_sibling_index);
        }
        
        public static GameObject GetItemByIndex(this UICopyGameObject self,int index)
        {
            return self.unity_comp.GetItemByIndex(index);
        }

        public static int GetListItemCount(this UICopyGameObject self)
        {
            return self.unity_comp.GetListItemCount();
        }
    }
}