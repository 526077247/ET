using UnityEngine;
namespace ET
{
    public class UITransformOnCreateSystem: OnCreateSystem<UITransform,Transform>
    {
        public override void OnCreate(UITransform self,Transform transform)
        {
            self.__transform = transform;
        }
    }
    
    public static class UITransformSystem
    {
        public static Transform GetTransform(this UIBaseContainer self)
        {
            return self.GetComponent<UITransform>("").transform;
        }
        
        public static GameObject GetGameObject(this UIBaseContainer self)
        {
            return self.GetComponent<UITransform>("").transform.gameObject;
        }
    }
}