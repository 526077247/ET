using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    [ObjectSystem]
    public class GalGameEngineFastForwardStateAwakeSystem : AwakeSystem<GalGameEngineFastForwardState, FSMComponent>
    {
        public override void Awake(GalGameEngineFastForwardState self, FSMComponent fsm)
        {
            self.FSM = fsm;
            self.Base = self.AddComponent<GalGameEngineRunningState,FSMComponent>(fsm);
        }
    }
    [FSMSystem]
    public class GalGameEngineFastForwardStateFSMOnEnterSystem : FSMOnEnterSystem<GalGameEngineFastForwardState>
    {
        public override async ETTask FSMOnEnter(GalGameEngineFastForwardState self)
        {
            self.BaseAutoPlay = self.Base.Engine.AutoPlay;
            self.Base.Engine.Speed = GalGameEngineComponent.FastSpeed;
            self.Base.Engine.AutoPlay = true;
            self.Base.Engine.State = GalGameEngineComponent.GalGameEngineState.FastForward;
            self.Base.ChapterCategory = self.Base.Engine.CurCategory;
            self.Base.MainRun().Coroutine();
            await ETTask.CompletedTask;
        }
    }
    [FSMSystem]
    public class GalGameEngineFastForwardStateFSMOnExitSystem : FSMOnExitSystem<GalGameEngineFastForwardState>
    {
        public override async ETTask FSMOnExit(GalGameEngineFastForwardState self)
        {
            GalGameEngineComponent.Instance.Speed = GalGameEngineComponent.NormalSpeed;
            GalGameEngineComponent.Instance.AutoPlay = self.BaseAutoPlay;
            await self.Base.Stop();
        }
    }
}
