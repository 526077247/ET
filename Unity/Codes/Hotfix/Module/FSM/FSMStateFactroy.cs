using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public static class FSMStateFactroy
    {
        public static State CreateFSMState<State>(Scene scene, FSMComponent self) where State : Entity,IAwake<FSMComponent>
        {
            return scene.AddChildWithId<State, FSMComponent>(IdGenerater.Instance.GenerateId(), self);
        }

        public static State CreateFSMState<State, T>(Scene scene, FSMComponent self, T t) where State : Entity,IAwake<FSMComponent,T>
        {
            return scene.AddChildWithId<State, FSMComponent, T>(IdGenerater.Instance.GenerateId(), self, t);
        }

        public static State CreateFSMState<State, T, U>(Scene scene, FSMComponent self, T t, U u) where State : Entity,IAwake<FSMComponent,T,U>
        {
            return scene.AddChildWithId<State, FSMComponent, T, U>(IdGenerater.Instance.GenerateId(), self, t, u);
        }

    }
}
