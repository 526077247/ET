using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{

    [ObjectSystem]
    public class ToastComponentAwakeSystem : AwakeSystem<ToastComponent>
    {
        public override void Awake(ToastComponent self)
        {
            self.Awake();
        }
    }
    public class ToastComponent:Entity
    {
        public static ToastComponent Instance;
        public Transform root;
        public void Awake()
        {
            Instance = this;
            root = UIManagerComponent.Instance.GetLayer(UILayerNames.TipLayer.ToString()).transform;
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            Instance = null;
        }
    }
}
