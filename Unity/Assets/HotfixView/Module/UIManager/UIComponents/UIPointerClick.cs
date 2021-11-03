using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace ET
{
    public class UIPointerClick : UIBaseComponent
    {

        UnityAction __onclick;
        PointerClick __unity_pointerclick;

        PointerClick unity_pointerclick
        {
            get
            {
                if (__unity_pointerclick == null)
                {
                    __unity_pointerclick = this.gameObject.GetComponent<PointerClick>();
                }
                return __unity_pointerclick;
            }
        }
        
        //虚拟点击
        public void Click()
        {
            __onclick?.Invoke();
        }

        public void SetOnClick(UnityAction callback)
        {
            RemoveOnClick();
            __onclick = () =>
            {
                //AkSoundEngine.PostEvent("ConFirmation", Camera.main.gameObject);
                callback();
            };
            unity_pointerclick.onClick.AddListener(__onclick);
        }

        public void RemoveOnClick()
        {
            if (__onclick != null)
                unity_pointerclick.onClick.RemoveListener(__onclick);
            __onclick = null;
        }

        public void SetEnabled(bool flag)
        {
            unity_pointerclick.enabled = flag;
        }

       
        public override void Dispose()
        {
            base.Dispose();
            if (__onclick != null)
                __unity_pointerclick.onClick.RemoveListener(__onclick);
            __unity_pointerclick = null;
            __onclick = null;
        }

    }
}