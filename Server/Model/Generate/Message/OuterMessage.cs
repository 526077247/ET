using ET;
using Nino.Serialization;
using System.Collections.Generic;
namespace ET
{
	[ResponseType(nameof(M2C_TestResponse))]
	[Message(OuterOpcode.C2M_TestRequest)]
	[NinoSerialize]
	public partial class C2M_TestRequest: Object, IActorLocationRequest
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(1)]
		public string request { get; set; }

	}

	[Message(OuterOpcode.M2C_TestResponse)]
	[NinoSerialize]
	public partial class M2C_TestResponse: Object, IActorLocationResponse
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(91)]
		public int Error { get; set; }

		[NinoMember(92)]
		public string Message { get; set; }

		[NinoMember(1)]
		public string response { get; set; }

	}

	[ResponseType(nameof(Actor_TransferResponse))]
	[Message(OuterOpcode.Actor_TransferRequest)]
	[NinoSerialize]
	public partial class Actor_TransferRequest: Object, IActorLocationRequest
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(1)]
		public int MapIndex { get; set; }

	}

	[Message(OuterOpcode.Actor_TransferResponse)]
	[NinoSerialize]
	public partial class Actor_TransferResponse: Object, IActorLocationResponse
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(91)]
		public int Error { get; set; }

		[NinoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(G2C_EnterMap))]
	[Message(OuterOpcode.C2G_EnterMap)]
	[NinoSerialize]
	public partial class C2G_EnterMap: Object, IRequest
	{
		[NinoMember(1)]
		public int RpcId { get; set; }

	}

	[Message(OuterOpcode.G2C_EnterMap)]
	[NinoSerialize]
	public partial class G2C_EnterMap: Object, IResponse
	{
		[NinoMember(1)]
		public int RpcId { get; set; }

		[NinoMember(2)]
		public int Error { get; set; }

		[NinoMember(3)]
		public string Message { get; set; }

// 自己unitId
		[NinoMember(4)]
		public long MyId { get; set; }

	}

	[Message(OuterOpcode.MoveInfo)]
	[NinoSerialize]
	public partial class MoveInfo: Object
	{
		[NinoMember(1)]
		public List<float> X = new List<float>();

		[NinoMember(2)]
		public List<float> Y = new List<float>();

		[NinoMember(3)]
		public List<float> Z = new List<float>();

		[NinoMember(4)]
		public float A { get; set; }

		[NinoMember(5)]
		public float B { get; set; }

		[NinoMember(6)]
		public float C { get; set; }

		[NinoMember(7)]
		public float W { get; set; }

		[NinoMember(8)]
		public int TurnSpeed { get; set; }

	}

	[Message(OuterOpcode.UnitInfo)]
	[NinoSerialize]
	public partial class UnitInfo: Object
	{
		[NinoMember(1)]
		public long UnitId { get; set; }

		[NinoMember(2)]
		public int ConfigId { get; set; }

		[NinoMember(3)]
		public int Type { get; set; }

		[NinoMember(4)]
		public float X { get; set; }

		[NinoMember(5)]
		public float Y { get; set; }

		[NinoMember(6)]
		public float Z { get; set; }

		[NinoMember(7)]
		public float ForwardX { get; set; }

		[NinoMember(8)]
		public float ForwardY { get; set; }

		[NinoMember(9)]
		public float ForwardZ { get; set; }

		[NinoMember(10)]
		public List<int> Ks = new List<int>();

		[NinoMember(11)]
		public List<long> Vs = new List<long>();

		[NinoMember(12)]
		public MoveInfo MoveInfo { get; set; }

		[NinoMember(13)]
		public List<int> SkillIds = new List<int>();

		[NinoMember(14)]
		public List<int> BuffIds = new List<int>();

		[NinoMember(15)]
		public List<long> BuffTimestamp = new List<long>();

	}

	[Message(OuterOpcode.M2C_CreateUnits)]
	[NinoSerialize]
	public partial class M2C_CreateUnits: Object, IActorMessage
	{
		[NinoMember(2)]
		public List<UnitInfo> Units = new List<UnitInfo>();

	}

	[Message(OuterOpcode.M2C_CreateMyUnit)]
	[NinoSerialize]
	public partial class M2C_CreateMyUnit: Object, IActorMessage
	{
		[NinoMember(1)]
		public UnitInfo Unit { get; set; }

	}

	[Message(OuterOpcode.M2C_StartSceneChange)]
	[NinoSerialize]
	public partial class M2C_StartSceneChange: Object, IActorMessage
	{
		[NinoMember(1)]
		public long SceneInstanceId { get; set; }

		[NinoMember(2)]
		public string SceneName { get; set; }

	}

	[Message(OuterOpcode.M2C_RemoveUnits)]
	[NinoSerialize]
	public partial class M2C_RemoveUnits: Object, IActorMessage
	{
		[NinoMember(2)]
		public List<long> Units = new List<long>();

	}

	[Message(OuterOpcode.C2M_PathfindingResult)]
	[NinoSerialize]
	public partial class C2M_PathfindingResult: Object, IActorLocationMessage
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(1)]
		public float X { get; set; }

		[NinoMember(2)]
		public float Y { get; set; }

		[NinoMember(3)]
		public float Z { get; set; }

	}

	[Message(OuterOpcode.C2M_Stop)]
	[NinoSerialize]
	public partial class C2M_Stop: Object, IActorLocationMessage
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

	}

	[Message(OuterOpcode.M2C_PathfindingResult)]
	[NinoSerialize]
	public partial class M2C_PathfindingResult: Object, IActorMessage
	{
		[NinoMember(1)]
		public long Id { get; set; }

		[NinoMember(2)]
		public float X { get; set; }

		[NinoMember(3)]
		public float Y { get; set; }

		[NinoMember(4)]
		public float Z { get; set; }

		[NinoMember(5)]
		public List<float> Xs = new List<float>();

		[NinoMember(6)]
		public List<float> Ys = new List<float>();

		[NinoMember(7)]
		public List<float> Zs = new List<float>();

	}

	[Message(OuterOpcode.M2C_Stop)]
	[NinoSerialize]
	public partial class M2C_Stop: Object, IActorMessage
	{
		[NinoMember(1)]
		public int Error { get; set; }

		[NinoMember(2)]
		public long Id { get; set; }

		[NinoMember(3)]
		public float X { get; set; }

		[NinoMember(4)]
		public float Y { get; set; }

		[NinoMember(5)]
		public float Z { get; set; }

		[NinoMember(6)]
		public float A { get; set; }

		[NinoMember(7)]
		public float B { get; set; }

		[NinoMember(8)]
		public float C { get; set; }

		[NinoMember(9)]
		public float W { get; set; }

	}

	[ResponseType(nameof(G2C_Ping))]
	[Message(OuterOpcode.C2G_Ping)]
	[NinoSerialize]
	public partial class C2G_Ping: Object, IRequest
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

	}

	[Message(OuterOpcode.G2C_Ping)]
	[NinoSerialize]
	public partial class G2C_Ping: Object, IResponse
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(91)]
		public int Error { get; set; }

		[NinoMember(92)]
		public string Message { get; set; }

		[NinoMember(1)]
		public long Time { get; set; }

	}

	[Message(OuterOpcode.G2C_Test)]
	[NinoSerialize]
	public partial class G2C_Test: Object, IMessage
	{
	}

	[ResponseType(nameof(M2C_Reload))]
	[Message(OuterOpcode.C2M_Reload)]
	[NinoSerialize]
	public partial class C2M_Reload: Object, IRequest
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(1)]
		public string Account { get; set; }

		[NinoMember(2)]
		public string Password { get; set; }

	}

	[Message(OuterOpcode.M2C_Reload)]
	[NinoSerialize]
	public partial class M2C_Reload: Object, IResponse
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(91)]
		public int Error { get; set; }

		[NinoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(R2C_Login))]
	[Message(OuterOpcode.C2R_Login)]
	[NinoSerialize]
	public partial class C2R_Login: Object, IRequest
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(1)]
		public string Account { get; set; }

		[NinoMember(2)]
		public string Password { get; set; }

	}

	[Message(OuterOpcode.R2C_Login)]
	[NinoSerialize]
	public partial class R2C_Login: Object, IResponse
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(91)]
		public int Error { get; set; }

		[NinoMember(92)]
		public string Message { get; set; }

		[NinoMember(1)]
		public string Address { get; set; }

		[NinoMember(2)]
		public long Key { get; set; }

		[NinoMember(3)]
		public long GateId { get; set; }

	}

	[ResponseType(nameof(G2C_LoginGate))]
	[Message(OuterOpcode.C2G_LoginGate)]
	[NinoSerialize]
	public partial class C2G_LoginGate: Object, IRequest
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(1)]
		public long Key { get; set; }

		[NinoMember(2)]
		public long GateId { get; set; }

	}

	[Message(OuterOpcode.G2C_LoginGate)]
	[NinoSerialize]
	public partial class G2C_LoginGate: Object, IResponse
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(91)]
		public int Error { get; set; }

		[NinoMember(92)]
		public string Message { get; set; }

		[NinoMember(1)]
		public long PlayerId { get; set; }

	}

	[Message(OuterOpcode.G2C_TestHotfixMessage)]
	[NinoSerialize]
	public partial class G2C_TestHotfixMessage: Object, IMessage
	{
		[NinoMember(1)]
		public string Info { get; set; }

	}

	[ResponseType(nameof(M2C_TestRobotCase))]
	[Message(OuterOpcode.C2M_TestRobotCase)]
	[NinoSerialize]
	public partial class C2M_TestRobotCase: Object, IActorLocationRequest
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(1)]
		public int N { get; set; }

	}

	[Message(OuterOpcode.M2C_TestRobotCase)]
	[NinoSerialize]
	public partial class M2C_TestRobotCase: Object, IActorLocationResponse
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(91)]
		public int Error { get; set; }

		[NinoMember(92)]
		public string Message { get; set; }

		[NinoMember(1)]
		public int N { get; set; }

	}

	[ResponseType(nameof(M2C_TransferMap))]
	[Message(OuterOpcode.C2M_TransferMap)]
	[NinoSerialize]
	public partial class C2M_TransferMap: Object, IActorLocationRequest
	{
		[NinoMember(1)]
		public int RpcId { get; set; }

	}

	[Message(OuterOpcode.M2C_TransferMap)]
	[NinoSerialize]
	public partial class M2C_TransferMap: Object, IActorLocationResponse
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(91)]
		public int Error { get; set; }

		[NinoMember(92)]
		public string Message { get; set; }

	}

	[Message(OuterOpcode.C2M_UseSkill)]
	[NinoSerialize]
	public partial class C2M_UseSkill: Object, IActorLocationMessage
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(1)]
		public int SkillConfigId { get; set; }

		[NinoMember(2)]
		public long Id { get; set; }

		[NinoMember(3)]
		public float X { get; set; }

		[NinoMember(4)]
		public float Y { get; set; }

		[NinoMember(5)]
		public float Z { get; set; }

	}

	[Message(OuterOpcode.M2C_UseSkill)]
	[NinoSerialize]
	public partial class M2C_UseSkill: Object, IActorMessage
	{
		[NinoMember(1)]
		public int Error { get; set; }

		[NinoMember(2)]
		public int SkillConfigId { get; set; }

		[NinoMember(3)]
		public long Sender { get; set; }

		[NinoMember(4)]
		public long Reciver { get; set; }

		[NinoMember(5)]
		public float X { get; set; }

		[NinoMember(6)]
		public float Y { get; set; }

		[NinoMember(7)]
		public float Z { get; set; }

	}

	[Message(OuterOpcode.M2C_AddBuff)]
	[NinoSerialize]
	public partial class M2C_AddBuff: Object, IActorMessage
	{
		[NinoMember(1)]
		public int Error { get; set; }

		[NinoMember(2)]
		public int ConfigId { get; set; }

		[NinoMember(3)]
		public long Timestamp { get; set; }

		[NinoMember(4)]
		public long UnitId { get; set; }

	}

	[Message(OuterOpcode.M2C_Damage)]
	[NinoSerialize]
	public partial class M2C_Damage: Object, IActorMessage
	{
		[NinoMember(1)]
		public int Error { get; set; }

		[NinoMember(2)]
		public long FromId { get; set; }

		[NinoMember(3)]
		public long ToId { get; set; }

		[NinoMember(4)]
		public int ConfigId { get; set; }

		[NinoMember(5)]
		public long Damage { get; set; }

	}

}
