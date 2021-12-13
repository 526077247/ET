using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
namespace ET
{
    /// <summary>
    /// 在session上挂载的保存路由信息的组件.切换路由用
    /// </summary>
    public class RouterDataComponent : Entity
    {
        public long Gateid;
    }
    public class SwitchRouterComponentAwakeSystem : AwakeSystem<SwitchRouterComponent>
    {
        public override void Awake(SwitchRouterComponent self)
        {
            self.ChangeRouter().Coroutine();
        }
    }
    /// <summary>
    /// 切换路由组件
    /// </summary>
    public class SwitchRouterComponent : Entity
    {
        public async ETTask ChangeRouter()
        {
            Session session = GetParent<Session>();
            session.RemoveComponent<SessionIdleCheckerComponent>();
            var gateid = session.GetComponent<RouterDataComponent>().Gateid;
            var routercomponent = session.AddComponent<GetRouterComponent, long, long>(gateid, session.Id);
            string routerAddress = await routercomponent.Tcs;
            session.RemoveComponent<GetRouterComponent>();
            if (routerAddress == "")
            {
                session.Dispose();
                return;
            }
            (session.AService as KService).ChangeAddress(session.Id, NetworkHelper.ToIPEndPoint(routerAddress));
            session.LastRecvTime = TimeHelper.ClientNow();
            session.AddComponent<SessionIdleCheckerComponent,int>(NetThreadComponent.checkInteral);
            session.RemoveComponent<SwitchRouterComponent>();
        }
    }

}
