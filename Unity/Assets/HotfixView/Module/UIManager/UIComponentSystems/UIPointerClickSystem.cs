using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace ET
{
    public class UIPointerClickDestorySystem : DestroySystem<UIPointerClick>
    {
        public override void Destroy(UIPointerClick self)
        {
            if (self.__onclick != null)
                self.unity_pointerclick.onClick.RemoveListener(self.__onclick);
            self.__onclick = null;
        }
    }
    public static class UIPointerClickSystem
    {

        //虚拟点击
        public static void Click(this UIPointerClick self)
        {
            self.__onclick?.Invoke();
        }

        public static void SetOnClick(this UIPointerClick self,UnityAction callback)
        {
            self.RemoveOnClick();
            self.__onclick = () =>
            {
                //AkSoundEngine.PostEvent("ConFirmation", Camera.main.gameObject);
                callback();
            };
            self.unity_pointerclick.onClick.AddListener(self.__onclick);
        }

        public static void RemoveOnClick(this UIPointerClick self)
        {
            if (self.__onclick != null)
                self.unity_pointerclick.onClick.RemoveListener(self.__onclick);
            self.__onclick = null;
        }

        public static void SetEnabled(this UIPointerClick self,bool flag)
        {
            self.unity_pointerclick.enabled = flag;
        }

    }
}