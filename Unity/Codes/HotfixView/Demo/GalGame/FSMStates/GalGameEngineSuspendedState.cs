using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    [ObjectSystem]
    public class GalGameEngineSuspendedStateAwakeSystem : AwakeSystem<GalGameEngineSuspendedState, FSMComponent>
    {
        public override void Awake(GalGameEngineSuspendedState self, FSMComponent fsm)
        {
            self.FSM = fsm;
        }
    }
    [FSMSystem]
    public class GalGameEngineSuspendedStateFSMOnEnterSystem : FSMOnEnterSystem<GalGameEngineSuspendedState>
    {
        public override async ETTask FSMOnEnter(GalGameEngineSuspendedState self)
        {
            GalGameEngineComponent.Instance.State = GalGameEngineComponent.GalGameEngineState.Suspended;
            await ETTask.CompletedTask;
        }
    }

   
}
