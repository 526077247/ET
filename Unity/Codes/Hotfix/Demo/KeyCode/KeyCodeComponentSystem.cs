using System;
using UnityEngine;

namespace ET
{
    [ObjectSystem]
    public class KeyCodeComponentAwakeSystem : AwakeSystem<KeyCodeComponent>
    {
        public override void Awake(KeyCodeComponent self)
        {
#if !NOT_UNITY
            var jstr = PlayerPrefs.GetString(CacheKeys.KeyCodeSetting);
            if (string.IsNullOrEmpty(jstr))
            {
#endif
                self.Skills = new int[6];
                self.Skills[0] = 49;//KeyCode.Alpha1;
                self.Skills[1] = 50;//KeyCode.Alpha2;
                self.Skills[2] = 51;//KeyCode.Alpha3;
                self.Skills[3] = 52;//KeyCode.Alpha4;
                self.Skills[4] = 53;//KeyCode.Alpha5;
                self.Skills[5] = 54;//KeyCode.Alpha6;
#if !NOT_UNITY
            }
            else
            {
                self.JsonText = jstr;
            }
#endif
            KeyCodeComponent.Instance = self;
        }
    }
    [ObjectSystem]
    public class KeyCodeComponentDestroySystem : DestroySystem<KeyCodeComponent>
    {
        public override void Destroy(KeyCodeComponent self)
        {
            KeyCodeComponent.Instance = null;
        }
    }

    public static class KeyCodeComponentSystem 
    {
        public static void Save(this KeyCodeComponent self)
        {
#if !NOT_UNITY
            PlayerPrefs.SetString(CacheKeys.KeyCodeSetting, self.JsonText);
#endif
        }
    }
}