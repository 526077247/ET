using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace ET
{

    public class RouterIPEndPoint
    {
        public IPEndPoint ClientEndPoint;

        public IPEndPoint TargetEndPoint;
        public uint MsgTime;
        public RouterIPEndPoint(IPEndPoint clientEndPoint,IPEndPoint targetEndPoint, uint msgTime)
        {
            ClientEndPoint = clientEndPoint;
            TargetEndPoint = targetEndPoint;
            MsgTime = msgTime;
        }
    }
    [ObjectSystem]
    public class RouterServiceComponentAwakeSystem : AwakeSystem<RouterServiceComponent, IPEndPoint>
    {
        public override void Awake(RouterServiceComponent self, IPEndPoint address)
        {
            self.Awake(address);
        }
    }
    [ObjectSystem]
    public class RouterServiceComponentUpdateSystem : UpdateSystem<RouterServiceComponent>
    {
        public override void Update(RouterServiceComponent self)
        {
            self.Update();
        }
    }
    [ObjectSystem]
    public class RouterServiceComponentDestroySystem : DestroySystem<RouterServiceComponent>
    {
        public override void Destroy(RouterServiceComponent self)
        {
            self.Destroy();
        }
    }
    /// <summary>
    /// 软路由服务组件
    /// </summary>
    public sealed class RouterServiceComponent : Entity,IAwake<IPEndPoint>,IUpdate,IDestroy
    {
        // RouterService创建的时间
        public long StartTime;

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
        }
        private readonly Dictionary<uint, RouterIPEndPoint> waitConnectChannels = new Dictionary<uint, RouterIPEndPoint>();
        private readonly List<uint> waitRemoveConnectChannels = new List<uint>();
        private readonly Dictionary<ulong, RouterIPEndPoint> clientsAddress = new Dictionary<ulong, RouterIPEndPoint>();
        private readonly List<ulong> waitRemoveAddress = new List<ulong>();

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
                        case KcpProtocalType.RouterSYN:
                            remoteConn = BitConverter.ToUInt32(this.cache, 1);
                            int gateid = BitConverter.ToInt32(this.cache, 5);
                            if (StartSceneConfigCategory.Instance.Contain(gateid))
                            {
                                var conf = StartSceneConfigCategory.Instance.Get(gateid);
                                if (conf.SceneType == "Gate")
                                {
                                    if (waitConnectChannels.TryGetValue(remoteConn, out var ipendpoint))
                                    {
                                        //如果是自己重复发.就再返回,否则直接抛弃
                                        if (ipendpoint.ClientEndPoint.ToString() == this.ipEndPoint.ToString())
                                        {
                                            byte[] newbuffer = this.cache;
                                            newbuffer.WriteTo(0, KcpProtocalType.RouterACK);
                                            this.socket.SendTo(newbuffer, 0, 1, SocketFlags.None, this.ipEndPoint);
                                            Log.Debug("RouterSYN repeated:" + this.ipEndPoint.ToString());
                                        }
                                        break;
                                    }
                                    //这是第一次添加
                                    else 
                                    {
                                        var inneraddress = conf.InnerIPOutPort;
                                        waitConnectChannels[remoteConn]= new RouterIPEndPoint(this.CloneAddress(), inneraddress, this.TimeNow);
                                        byte[] newbuffer = this.cache;
                                        newbuffer.WriteTo(0, KcpProtocalType.RouterACK);
                                        this.socket.SendTo(newbuffer, 0, 1, SocketFlags.None, this.ipEndPoint);
                                        Log.Debug("RouterSYN 成功:" + this.ipEndPoint.ToString());
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                Log.Debug("Router SYN error:not found gate:" + gateid.ToString());
                                break;
                            }
                            break;
                        case KcpProtocalType.SYN: // accept
                        case KcpProtocalType.RouterReconnect:
                            if (messageLength < 9)
                            {
                                break;
                            }
                            remoteConn = BitConverter.ToUInt32(this.cache, 1);
                            if (waitConnectChannels.TryGetValue(remoteConn,out var routerIPEnd))
                            {
                                //syn的时候更新客户端地址.有可能是不同的socket发来的
                                Log.Debug("SYN 之前地址:" + routerIPEnd.ClientEndPoint.ToString());
                                routerIPEnd.ClientEndPoint = this.CloneAddress();
                                Domain.GetComponent<RouterServiceInnerComponent>().SendToGate(this.cache, 9, routerIPEnd.TargetEndPoint);
                                Log.Debug("SYN 地址变更成功:" + this.ipEndPoint.ToString());
                            }
                            break;
                        case KcpProtocalType.MSG:
                            if (messageLength < 9)
                            {
                                break;
                            }
                            remoteConn = BitConverter.ToUInt32(this.cache, 1);
                            localConn = BitConverter.ToUInt32(this.cache, 5);
                            remotelocalConn = ((ulong)remoteConn << 32) | localConn;
                            if (clientsAddress.TryGetValue(remotelocalConn, out var realTargetAddress))
                            {
                                Domain.GetComponent<RouterServiceInnerComponent>().SendToGate(this.cache, messageLength, realTargetAddress.TargetEndPoint);
                                realTargetAddress.MsgTime = this.TimeNow;
                            }
                            else
                            {
                                Log.Debug("Router MSG error:not found gateaddress:" + this.ipEndPoint.ToString());
                                break;
                            }
                            break;
                        case KcpProtocalType.FIN: // 断开
                            if (messageLength < 9)
                            {
                                break;
                            }
                            remoteConn = BitConverter.ToUInt32(this.cache, 1);
                            localConn = BitConverter.ToUInt32(this.cache, 5);
                            remotelocalConn = ((ulong)remoteConn << 32) | localConn;
                            if (clientsAddress.TryGetValue(remotelocalConn, out var finTargetAddress))
                            {
                                Domain.GetComponent<RouterServiceInnerComponent>().SendToGate(this.cache, messageLength, finTargetAddress.TargetEndPoint);
                                RemoveClientAddress(remotelocalConn);
                            }
                            else
                            {
                                Log.Debug("Router MSG FIN:not found gateaddress:" + this.ipEndPoint.ToString());
                                break;
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"RouterService error: {flag} {remoteConn} {localConn}\n{e}");
                }
            }
        }

        public void RemoveClientAddress(ulong remotelocalConn)
        {
            clientsAddress.Remove(remotelocalConn);
        }
        /// <summary>
        /// 获取ack或者切换路由成功时,移除连接信息.加入完整的路由信息
        /// </summary>
        /// <param name="remoteConn"></param>
        /// <param name="localConn"></param>
        /// <returns></returns>
        public bool GetACK(uint remoteConn, uint localConn)
        {
            Log.Debug($"GetACK:{localConn} {remoteConn}");
            if (waitConnectChannels.TryGetValue(remoteConn,out var routerIPEndPoint))
            {
                ulong remotelocal = ((ulong)remoteConn << 32) | localConn;
                clientsAddress[remotelocal] = routerIPEndPoint;
                waitConnectChannels.Remove(remoteConn);
                return true;
            }
            return false;
        }

        public bool SendToClient(ulong remotelocalConn, int messageLength, byte[] cache)
        {
            if (clientsAddress.TryGetValue(remotelocalConn, out var iPEndPointEntity))
            {
                this.socket.SendTo(cache, 0, messageLength, SocketFlags.None, iPEndPointEntity.ClientEndPoint);
                return true;
            }
            else
                return false;
        }

        public void Update()
        {
            this.Recv();
            var nowtime = TimeHelper.ClientNowSeconds();
            if (this.CurrTimeSecond != nowtime)
            {
                this.CurrTimeSecond = nowtime;
                if (this.CurrTimeSecond % 3 == 0)
                {
                    this.RemoveConnectTimeoutIds();
                }
            }
        }
        private void RemoveConnectTimeoutIds()
        {
            this.waitRemoveAddress.Clear();
            foreach (var clientaddress in this.clientsAddress.Keys)
            {
                if (this.TimeNow > this.clientsAddress[clientaddress].MsgTime + 30000)
                {
                    this.waitRemoveAddress.Add(clientaddress);
                }
            }
            foreach (var clientkey in waitRemoveAddress)
            {
                this.clientsAddress.Remove(clientkey);
            }
            if (clientsAddress.Count > 1000)
            {
                Log.Debug("clientsAddress.Count要报警了!:" + clientsAddress.Count);
            }
            //下面清理半连接
            this.waitRemoveConnectChannels.Clear();
            foreach (var channel in this.waitConnectChannels.Keys)
            {
                if (this.TimeNow > this.waitConnectChannels[channel].MsgTime + 10000)
                {
                    this.waitRemoveConnectChannels.Add(channel);
                }
            }
            foreach (var channelkey in waitRemoveConnectChannels)
            {
                this.waitConnectChannels.Remove(channelkey);
            }
            if (waitConnectChannels.Count > 1000)
            {
                Log.Debug("waitConnectChannels.Count要报警了!:" + waitConnectChannels.Count);
            }
        }
    }

}