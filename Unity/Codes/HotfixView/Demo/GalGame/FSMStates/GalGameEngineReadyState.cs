using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    [ObjectSystem]
    public class GalGameEngineReadyStateAwakeSystem : AwakeSystem<GalGameEngineReadyState, FSMComponent>
    {
        public override void Awake(GalGameEngineReadyState self, FSMComponent fsm)
        {
            self.FSM = fsm;
        }
    }
    [FSMSystem]
    public class GalGameEngineReadyStateOnEnterSystem : FSMOnEnterSystem<GalGameEngineReadyState>
    {
        public override async ETTask FSMOnEnter(GalGameEngineReadyState self)
        {
            self.Engine = GalGameEngineComponent.Instance;
            self.Engine.State = GalGameEngineComponent.GalGameEngineState.Ready;
            self.Engine.CurCategory = null;
            self.Engine.Index = 0;
            self.Engine.AutoPlay = true;
            self.Engine.StageRoleMap.Clear();
            self.Engine.RoleExpressionMap.Clear();
            await UIManagerComponent.Instance.CloseWindow<UIMessageWindow>();
            await UIManagerComponent.Instance.CloseWindow<UIBaseMessageWindow>();
            await UIManagerComponent.Instance.CloseWindow<UIStageView>();
            await UIManagerComponent.Instance.CloseWindow<UIBgView>();
            await UIManagerComponent.Instance.CloseWindow<UIGalGameHelper>();
            await UIManagerComponent.Instance.CloseWindow<UIMaskView>();
            await ETTask.CompletedTask;
        }
    }
    [FSMSystem]
    public class GalGameEngineReadyStateOnEnterSystem1 : FSMOnEnterSystem<GalGameEngineReadyState,bool>
    {
        public override async ETTask FSMOnEnter(GalGameEngineReadyState self,bool value)
        {
            self.Engine = GalGameEngineComponent.Instance;
            self.Engine.State = GalGameEngineComponent.GalGameEngineState.Ready;
            self.Engine.CurCategory = null;
            self.Engine.Index = 0;
            self.Engine.AutoPlay = true;
            self.Engine.StageRoleMap.Clear();
            self.Engine.RoleExpressionMap.Clear();
            await UIManagerComponent.Instance.CloseWindow<UIMessageWindow>();
            await UIManagerComponent.Instance.CloseWindow<UIBaseMessageWindow>();
            await UIManagerComponent.Instance.CloseWindow<UIStageView>();
            await UIManagerComponent.Instance.CloseWindow<UIBgView>();
            await UIManagerComponent.Instance.CloseWindow<UIGalGameHelper>();
            await UIManagerComponent.Instance.CloseWindow<UIMaskView>();
            self.Engine.OnPlayOver?.Invoke(value);
            await ETTask.CompletedTask;
        }
    }

}
