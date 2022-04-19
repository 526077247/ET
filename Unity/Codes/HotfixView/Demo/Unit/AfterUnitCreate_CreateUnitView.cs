using UnityEngine;

namespace ET
{
    [FriendClass(typeof(GlobalComponent))]
    public class AfterUnitCreate_CreateUnitView: AEventAsync<EventType.AfterUnitCreate>
    {
        protected override async ETTask Run(EventType.AfterUnitCreate args)
        {
            // Unit View层
            // 这里可以改成异步加载，demo就不搞了
            var go = await GameObjectPoolComponent.Instance.GetGameObjectAsync(args.Unit.Config.Perfab);
            go.transform.position = args.Unit.Position;
            go.transform.parent = GlobalComponent.Instance.Unit;
            args.Unit.AddComponent<GameObjectComponent>().GameObject = go;
            args.Unit.AddComponent<AnimatorComponent>();
        }
    }
}