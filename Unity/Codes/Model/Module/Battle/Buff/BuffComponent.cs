using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
    [ChildType(typeof(Buff))]
    [ComponentOf(typeof(CombatUnitComponent))]
    public class BuffComponent:Entity, IAwake, IDestroy,ITransfer
    {
        [BsonIgnore]
        public Unit unit => this.GetParent<CombatUnitComponent>().unit;
        
        public DictionaryComponent<int, Buff> Groups;
        public DictionaryComponent<int, int> ActionControls;
    }
}