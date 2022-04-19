using UnityEngine;
namespace ET
{
    [ObjectSystem]
    [FriendClass(typeof(UIWindow))]
    public class UIManagerComponentUpdateSystem : UpdateSystem<UIManagerComponent>
    {
        public override void Update(UIManagerComponent self)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                var win = self.GetTopWindow();
                if (win != null)
                {
                    if(!win.BanKey)
                        UIManagerComponent.Instance.CloseWindow(win.Name).Coroutine();
                }
            }
        }
    }
}