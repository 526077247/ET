using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public static class UIGameObjectPoolSystem
    {
        public static async ETTask<T> GetUIGameObjectAsync<T>(this GameObjectPoolComponent self, string path) where T : UIBaseContainer
        {
            var obj = await self.GetGameObjectAsync(path);
            if (obj == null) return null;
            T res = self.AddChild<T>();
            res.gameObject = obj;
            res.OnCreate();
            return res;
        }

        public static void RecycleUIGameObject<T>(this GameObjectPoolComponent self, T obj,bool isClear = false) where T : UIBaseContainer
        {
	        self.RecycleGameObject(obj.gameObject, isClear);
	        obj.Dispose();
        }
    }
}
