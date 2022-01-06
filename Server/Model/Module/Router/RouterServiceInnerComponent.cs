using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace ET
{

    [ObjectSystem]
    public class RouterServiceInnerComponentAwakeSystem : AwakeSystem<RouterServiceInnerComponent, IPEndPoint>
    {
        public override void Awake(RouterServiceInnerComponent self, IPEndPoint address)
        {
            self.Awake(address);
        }
    }
    [ObjectSystem]
    public class RouterServiceInnerComponentAwakeSystem2 : AwakeSystem<RouterServiceInnerComponent>
    {
        public override void Awake(RouterServiceInnerComponent self)
        {
            self.Awake(new IPEndPoint(IPAddress.Any, 0));
        }
    }
    [ObjectSystem]
    public class RouterServiceInnerComponentUpdateSystem : UpdateSystem<RouterServiceInnerComponent>
    {
        public override void Update(RouterServiceInnerComponent self)
        {
            self.Update();
        }
    }
    [ObjectSystem]
    public class RouterServiceInnerComponentDestroySystem : DestroySystem<RouterServiceInnerComponent>
    {
        public override void Destroy(RouterServiceInnerComponent self)
        {
            self.Destroy();
        }
    }
    public class RouterServiceInnerData
    {
        public string Clientaddress;
        public uint MsgTime;

        public RouterServiceInnerData(string clientaddress, uint msgTime)
        {
            Clientaddress = clientaddress;
            MsgTime = msgTime;
        }
    }
    /// <summary>
    /// 软路由服务组件
    /// </summary>
    public sealed class RouterServiceInnerComponent : Entity,IAwake,IAwake<IPEndPoint>,IDestroy,IUpdate
    {
        // RouterService创建的时间
        public long StartTime;
        RouterServiceComponent OuterRouterService;

        // 当前时间 - RouterService创建的时间, 线程安全
        public uint TimeNow
        {
            get
            {
                return (uint)(TimeHelper.ClientNow() - this.StartTime);
            }
        }

        private Socket socket;
        private long CurrTimeSecond;
       

        public void Awake(IPEndPoint ipEndPoint)
        {
            this.StartTime = TimeHelper.ClientNow();
            this.CurrTimeSecond = TimeHelper.ClientNowSeconds();
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                this.socket.SendBufferSize = Kcp.OneM * 64;
                this.socket.ReceiveBufferSize = Kcp.OneM * 64;
            }

            this.socket.Bind(ipEndPoint);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                const uint IOC_IN = 0x80000000;
                const uint IOC_VENDOR = 0x18000000;
                uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                this.socket.IOControl((int)SIO_UDP_CONNRESET, new[] { Convert.ToByte(false) }, null);
            }
            OuterRouterService = Domain.GetComponent<RouterServiceComponent>();
        }

        private readonly byte[] cache = new byte[8192];
        private EndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 0);



        public void Destroy()
        {
            this.socket.Close();
            this.socket = null;
        }

        private IPEndPoint CloneAddress()
        {
            IPEndPoint ip = (IPEndPoint)this.ipEndPoint;
            return new IPEndPoint(ip.Address, ip.Port);
        }
        private void Recv()
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

                // accept
                byte flag = this.cache[0];

                // conn从100开始，如果为1，2，3则是特殊包
                uint remoteConn = 0;
                uint localConn = 0;
                ulong remotelocalConn = 0;
                try
                {
                    switch (flag)
                    {
                        //此处映射gate过来的消息发给哪个客户端
                        case KcpProtocalType.ACK: // accept
                        case KcpProtocalType.RouterReconnectAck:
                            if (messageLength < 9)
                            {
                                break;
                            }
                            remoteConn = BitConverter.ToUInt32(this.cache, 1);
                            localConn = BitConverter.ToUInt32(this.cache, 5);
                            remotelocalConn = ((ulong)localConn << 32) | remoteConn;
                            if (OuterRouterService.GetACK(localConn, remoteConn))
                            {
                                OuterRouterService.SendToClient(remotelocalConn, messageLength, this.cache);
                            }
                            break;
                        case KcpProtocalType.MSG:
                            if (messageLength < 9)
                            {
                                break;
                            }
                            remoteConn = BitConverter.ToUInt32(this.cache, 1);
                            localConn = BitConverter.ToUInt32(this.cache, 5);
                            remotelocalConn = ((ulong)localConn << 32) | remoteConn;
                            if (!OuterRouterService.SendToClient(remotelocalConn,messageLength,this.cache))
                            {
                                //todo: 这里发送失败的话应该主动给服务端发一条FIN消息.免得服务端继续发消息
                                Log.Debug("Router MSG error:not found client:" + remotelocalConn);
                            }
                            break;
                        case KcpProtocalType.FIN: // 断开
                            if (messageLength < 9)
                            {
                                break;
                            }
                            remoteConn = BitConverter.ToUInt32(this.cache, 1);
                            localConn = BitConverter.ToUInt32(this.cache, 5);
                            remotelocalConn = ((ulong)localConn << 32) | remoteConn;
                            OuterRouterService.SendToClient(remotelocalConn, messageLength, this.cache);
                            OuterRouterService.RemoveClientAddress(remotelocalConn);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"RouterService error: {flag} {remoteConn} {localConn}\n{e}");
                }
            }
        }

        public void SendToGate(byte[] buffer, int length, IPEndPoint inneraddress)
        {
            this.socket.SendTo(buffer, 0, length, SocketFlags.None, inneraddress);

        }

        public void Update()
        {
            this.Recv();

        }
    }
}