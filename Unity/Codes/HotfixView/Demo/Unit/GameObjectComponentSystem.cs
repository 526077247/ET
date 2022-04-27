using System;
using UnityEngine;

namespace ET
{

    [ObjectSystem]
    public class GameObjectComponentAwakeSystem: AwakeSystem<GameObjectComponent,GameObject>
    {
        public override void Awake(GameObjectComponent self,GameObject a)
        {
            self.GameObject = a;
            self.IsDebug = false;
        }
    }
    [ObjectSystem]
    public class GameObjectComponentAwakeSystem1: AwakeSystem<GameObjectComponent,GameObject,Action>
    {
        public override void Awake(GameObjectComponent self,GameObject a,Action b)
        {
            self.OnDestroyAction = b;
            self.GameObject = a;
            self.IsDebug = false;
        }
    }
    [ObjectSystem]
    public class GameObjectComponentDestroySystem: DestroySystem<GameObjectComponent>
    {
        public override void Destroy(GameObjectComponent self)
        {
            if (self.OnDestroyAction != null)
            {
                self.OnDestroyAction();
            }
            else
            {
                UnityEngine.Object.Destroy(self.GameObject);
            }
            self.GameObject = null;
        }
    }
    public static class GameObjectComponentSystem
    {
        
    }
}