using ET;
using Nino.Serialization;
using System.Collections.Generic;
namespace ET
{
	[ResponseType(nameof(ObjectQueryResponse))]
	[Message(InnerOpcode.ObjectQueryRequest)]
	[NinoSerialize]
	public partial class ObjectQueryRequest: Object, IActorRequest
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(1)]
		public long Key { get; set; }

		[NinoMember(2)]
		public long InstanceId { get; set; }

	}

	[ResponseType(nameof(A2M_Reload))]
	[Message(InnerOpcode.M2A_Reload)]
	[NinoSerialize]
	public partial class M2A_Reload: Object, IActorRequest
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

	}

	[Message(InnerOpcode.A2M_Reload)]
	[NinoSerialize]
	public partial class A2M_Reload: Object, IActorResponse
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(91)]
		public int Error { get; set; }

		[NinoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(G2G_LockResponse))]
	[Message(InnerOpcode.G2G_LockRequest)]
	[NinoSerialize]
	public partial class G2G_LockRequest: Object, IActorRequest
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(1)]
		public long Id { get; set; }

		[NinoMember(2)]
		public string Address { get; set; }

	}

	[Message(InnerOpcode.G2G_LockResponse)]
	[NinoSerialize]
	public partial class G2G_LockResponse: Object, IActorResponse
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(91)]
		public int Error { get; set; }

		[NinoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(G2G_LockReleaseResponse))]
	[Message(InnerOpcode.G2G_LockReleaseRequest)]
	[NinoSerialize]
	public partial class G2G_LockReleaseRequest: Object, IActorRequest
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(1)]
		public long Id { get; set; }

		[NinoMember(2)]
		public string Address { get; set; }

	}

	[Message(InnerOpcode.G2G_LockReleaseResponse)]
	[NinoSerialize]
	public partial class G2G_LockReleaseResponse: Object, IActorResponse
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(91)]
		public int Error { get; set; }

		[NinoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(ObjectAddResponse))]
	[Message(InnerOpcode.ObjectAddRequest)]
	[NinoSerialize]
	public partial class ObjectAddRequest: Object, IActorRequest
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(1)]
		public long Key { get; set; }

		[NinoMember(2)]
		public long InstanceId { get; set; }

	}

	[Message(InnerOpcode.ObjectAddResponse)]
	[NinoSerialize]
	public partial class ObjectAddResponse: Object, IActorResponse
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(91)]
		public int Error { get; set; }

		[NinoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(ObjectLockResponse))]
	[Message(InnerOpcode.ObjectLockRequest)]
	[NinoSerialize]
	public partial class ObjectLockRequest: Object, IActorRequest
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(1)]
		public long Key { get; set; }

		[NinoMember(2)]
		public long InstanceId { get; set; }

		[NinoMember(3)]
		public int Time { get; set; }

	}

	[Message(InnerOpcode.ObjectLockResponse)]
	[NinoSerialize]
	public partial class ObjectLockResponse: Object, IActorResponse
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(91)]
		public int Error { get; set; }

		[NinoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(ObjectUnLockResponse))]
	[Message(InnerOpcode.ObjectUnLockRequest)]
	[NinoSerialize]
	public partial class ObjectUnLockRequest: Object, IActorRequest
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(1)]
		public long Key { get; set; }

		[NinoMember(2)]
		public long OldInstanceId { get; set; }

		[NinoMember(3)]
		public long InstanceId { get; set; }

	}

	[Message(InnerOpcode.ObjectUnLockResponse)]
	[NinoSerialize]
	public partial class ObjectUnLockResponse: Object, IActorResponse
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(91)]
		public int Error { get; set; }

		[NinoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(ObjectRemoveResponse))]
	[Message(InnerOpcode.ObjectRemoveRequest)]
	[NinoSerialize]
	public partial class ObjectRemoveRequest: Object, IActorRequest
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(1)]
		public long Key { get; set; }

	}

	[Message(InnerOpcode.ObjectRemoveResponse)]
	[NinoSerialize]
	public partial class ObjectRemoveResponse: Object, IActorResponse
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(91)]
		public int Error { get; set; }

		[NinoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(ObjectGetResponse))]
	[Message(InnerOpcode.ObjectGetRequest)]
	[NinoSerialize]
	public partial class ObjectGetRequest: Object, IActorRequest
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(1)]
		public long Key { get; set; }

	}

	[Message(InnerOpcode.ObjectGetResponse)]
	[NinoSerialize]
	public partial class ObjectGetResponse: Object, IActorResponse
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(91)]
		public int Error { get; set; }

		[NinoMember(92)]
		public string Message { get; set; }

		[NinoMember(1)]
		public long InstanceId { get; set; }

	}

	[ResponseType(nameof(G2R_GetLoginKey))]
	[Message(InnerOpcode.R2G_GetLoginKey)]
	[NinoSerialize]
	public partial class R2G_GetLoginKey: Object, IActorRequest
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(1)]
		public string Account { get; set; }

	}

	[Message(InnerOpcode.G2R_GetLoginKey)]
	[NinoSerialize]
	public partial class G2R_GetLoginKey: Object, IActorResponse
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(91)]
		public int Error { get; set; }

		[NinoMember(92)]
		public string Message { get; set; }

		[NinoMember(1)]
		public long Key { get; set; }

		[NinoMember(2)]
		public long GateId { get; set; }

	}

	[Message(InnerOpcode.M2M_UnitTransferResponse)]
	[NinoSerialize]
	public partial class M2M_UnitTransferResponse: Object, IActorResponse
	{
		[NinoMember(1)]
		public int RpcId { get; set; }

		[NinoMember(2)]
		public int Error { get; set; }

		[NinoMember(3)]
		public string Message { get; set; }

		[NinoMember(4)]
		public long NewInstanceId { get; set; }

	}

	[Message(InnerOpcode.G2M_SessionDisconnect)]
	[NinoSerialize]
	public partial class G2M_SessionDisconnect: Object, IActorLocationMessage
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

	}

}
