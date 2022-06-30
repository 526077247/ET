using Nino.Serialization;

namespace ET
{
    [Message(ushort.MaxValue)]
    [NinoSerialize()]
    public partial class ActorResponse: ProtoObject, IActorResponse
    {
        [NinoMember(1)]
        public int RpcId { get; set; }
        [NinoMember(2)]
        public int Error { get; set; }
        [NinoMember(3)]
        public string Message { get; set; }
    }
}