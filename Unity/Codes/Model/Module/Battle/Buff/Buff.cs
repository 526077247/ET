using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
    public class Buff:Entity,IAwake<int>,IDestroy
    {
        public int ConfigId;
        [BsonIgnore]
        public BuffConfig Config
        {
            get => BuffConfigCategory.Instance.Get(ConfigId);
        }
    }
}