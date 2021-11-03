using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public static class UIBaseViewSystem
    {
        public static void CloseSelf(this UIBaseView self)
        {
            UIManagerComponent.Instance.CloseWindow(self.__BaseViewName);
        }

        public static void DestroySelf(this UIBaseView self)
        {
            UIManagerComponent.Instance.DestroyWindow(self.__BaseViewName);
        }

        public static bool IsActiveSelf(this UIBaseView self)
        {
            return self.GetComponent<UIBaseComponent>().gameObject.activeSelf;
        }
    }
}
