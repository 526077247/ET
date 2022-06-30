using ET;
using Nino.Serialization;
using System.Collections.Generic;
namespace ET
{
	[Message(MongoOpcode.ObjectQueryResponse)]
	[NinoSerialize]
	public partial class ObjectQueryResponse: Object, IActorResponse
	{
		[NinoMember(90)]
		public int RpcId { get; set; }

		[NinoMember(91)]
		public int Error { get; set; }

		[NinoMember(92)]
		public string Message { get; set; }

		[NinoMember(1)]
		public Entity entity { get; set; }

	}

	[ResponseType(nameof(M2M_UnitTransferResponse))]
	[Message(MongoOpcode.M2M_UnitTransferRequest)]
	[NinoSerialize]
	public partial class M2M_UnitTransferRequest: Object, IActorRequest
	{
		[NinoMember(1)]
		public int RpcId { get; set; }

		[NinoMember(2)]
		public Unit Unit { get; set; }

		[NinoMember(3)]
		public List<Entity> Entitys = new List<Entity>();

		[NinoMember(4)]
		public List<RecursiveEntitys> Map = new List<RecursiveEntitys>();

	}

	[Message(MongoOpcode.RecursiveEntitys)]
	[NinoSerialize]
	public partial class RecursiveEntitys: Object
	{
		[NinoMember(1)]
		public int IsChild { get; set; }

		[NinoMember(2)]
		public int ParentIndex { get; set; }

		[NinoMember(3)]
		public int ChildIndex { get; set; }

	}

}
