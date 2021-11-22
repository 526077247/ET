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
            var view = target.GetComponent(target.ViewType) as UIBaseView;
            if (view.transform)
            {
                var layer = UIManagerComponent.Instance.GetComponent<UILayersComponent>().layers[target.Layer];
                view.transform.SetParent(layer.transform, false);
            }
        }
    }
}
