using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public class ResetWindowLayer_ResetWindowLayer : AEvent<UIEventType.ResetWindowLayer>
    {
        protected override async ETTask Run(UIEventType.ResetWindowLayer args)
        {
            var target = args.window;
            var view = target.GetComponent(target.ViewType);
            var uiTrans = view.GetUIComponent<UITransform>();
            if (uiTrans!=null)
            {
                var layer = UIManagerComponent.Instance.GetComponent<UILayersComponent>().layers[target.Layer];
                uiTrans.transform.SetParent(layer.transform, false);
            }
            await ETTask.CompletedTask;
        }
    }
}
