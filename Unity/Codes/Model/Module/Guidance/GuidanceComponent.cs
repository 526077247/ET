using MongoDB.Bson.Serialization.Attributes;
using System;
namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class GuidanceComponent:Entity,IAwake
    {
        public static GuidanceComponent Instance;

        public int Group;
        [BsonIgnore]
        public GuidanceGroupConfig Config => GuidanceConfigCategory.Instance.GetGroup(Group);
        
        public int CurIndex;
        
        [BsonIgnore]
        public GuidanceConfig StepConfig => Config?.Steps[this.CurIndex];
    }
}