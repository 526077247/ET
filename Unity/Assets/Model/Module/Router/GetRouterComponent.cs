using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
namespace ET
{
    /// <summary>
    /// 初始获取路由组件
    /// </summary>
    public class GetRouterComponent : Entity
    {
        public int ChangeTimes;
        public Socket socket;
        public readonly byte[] cache = new byte[8192];
        public EndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 0);
        public ETCancellationToken CancellationToken;
        public ETTask<string> Tcs;
        public bool IsChangingRouter;
        public void Recv()
        {
            if (this.socket == null)
            {
                return;
            }

            while (socket != null && this.socket.Available > 0)
            {
                int messageLength = this.socket.ReceiveFrom(this.cache, ref this.ipEndPoint);

                // 长度小于1，不是正常的消息
                if (messageLength < 1)
                {
                    continue;
                }
                byte flag = this.cache[0];
                try
                {
                    switch (flag)
                    {
                        case KcpProtocalType.RouterACK:
                            Log.Debug("RouterACK:"+ this.ipEndPoint.ToString());
                            this.Tcs?.SetResult(this.ipEndPoint.ToString());
                            this.Tcs = null;
                            this.CancellationToken?.Cancel();
                            break;
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"kservice error: {flag}\n{e}");
                }
            }
        }
    }

}
