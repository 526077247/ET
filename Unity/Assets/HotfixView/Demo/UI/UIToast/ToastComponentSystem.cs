using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    public class ToastComponentAwakeSystem : AwakeSystem<ToastComponent>
    {
        public override void Awake(ToastComponent self)
        {
            ToastComponent.Instance = self;
            self.root = UIManagerComponent.Instance.GetLayer(UILayerNames.TipLayer).transform;
        }
    }

    public class ToastComponentDestroySystem : DestroySystem<ToastComponent>
    {
        public override void Destroy(ToastComponent self)
        {
            ToastComponent.Instance = null;
            self.root = null;
        }
    }
}
