using UnityEngine;
namespace ET
{
    public class UITransform:UIBaseContainer
    {
        
        public Transform __transform;
        public Transform transform
        {
            get
            {
                if (__transform == null)
                {
                    var pui = this.Parent as UIBaseContainer;
                    __transform = this.ParentTransform?.Find(pui.Path);
                }
                return __transform;
            }
        }
        private Transform __ParentTransform;
        public Transform ParentTransform
        {
            get
            {
                if (__ParentTransform == null)
                {
                    var pui = this.Parent.Parent as UIBaseContainer;
                    var uitrans = pui.GetUIComponent<UITransform>("");
                    if (uitrans == null)
                    {
                        Log.Error("ParentTransform is null Path:"+this.Path);
                    }
                    else
                    {
                        __ParentTransform = uitrans.transform;
                    }
                }
                return __ParentTransform;
            }
        }
    }
}