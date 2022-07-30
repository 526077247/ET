using System.Collections.Generic;
using UnityEngine;
namespace ET
{
    [FriendClass(typeof(InputComponent))]
    public static class InputComponentSystem
    {
        public class InputComponentAwakeSystem:AwakeSystem<InputComponent>
        {
            public override void Awake(InputComponent self)
            {
                InputComponent.Instance = self;
                self.KeysForListen = new List<int>();
            }
            
             
        }
        
        public class InputComponentUpdateSystem:UpdateSystem<InputComponent>
        {
            public override void Update(InputComponent self)
            {
                for (int i= 0; i< self.KeysForListen.Count; ++i)
                {
                    int key = self.KeysForListen[i];
                    if (InputHelper.GetKeyDown(key))
                    {
                        InputWatcherComponent.Instance.Run(key,InputType.KeyDown);
                    }
                    if (InputHelper.GetKeyUp(key))
                    {
                        InputWatcherComponent.Instance.Run(key,InputType.KeyUp);
                    }
                    if (InputHelper.GetKey(key))
                    {
                        InputWatcherComponent.Instance.Run(key,InputType.Key);
                    }
                }
            }

        }
        
        public static void AddListenter(this InputComponent self, int key)
        {
            if (!self.KeysForListen.Contains(key))
            {
                self.KeysForListen.Add(key);
            }
        }
    }
}