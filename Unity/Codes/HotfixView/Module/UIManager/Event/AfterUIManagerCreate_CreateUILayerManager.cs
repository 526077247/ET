using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    public class AfterUIManagerCreate_CreateUILayerManager : AEvent<UIEventType.AfterUIManagerCreate>
    {
        protected override async ETTask Run(UIEventType.AfterUIManagerCreate args)
        {
            UIManagerComponent.Instance.AddComponent<UILayersComponent>();
            await ETTask.CompletedTask;
        }
    }
}
