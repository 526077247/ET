using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    public static class UIGameObjectPoolSystem
    {
        public static async ETTask<T> GetUIGameObjectAsync<T>(this GameObjectPoolComponent self, string path) where T : Entity,IAwake
        {
            var obj = await self.GetGameObjectAsync(path);
            if (obj == null) return null;
            T res = self.AddChild<T>();
            res.AddUIComponent<UITransform,Transform>("", obj.transform);
            UIEventSystem.Instance.OnCreate(res);
            return res;
        }

        public static void RecycleUIGameObject<T>(this GameObjectPoolComponent self, T obj,bool isClear = false) where T : Entity
        {
            var uiTrans = obj.GetUIComponent<UITransform>();
            self.RecycleGameObject(uiTrans.transform.gameObject, isClear);
            obj.BeforeOnDestroy();
            UIEventSystem.Instance.OnDestroy(obj);
        }
    }
}
