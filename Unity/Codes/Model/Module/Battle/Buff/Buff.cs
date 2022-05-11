using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
    public class Buff:Entity,IAwake<int,long>,IAwake<int,long,bool>,IDestroy,ITransfer
    {
        public int ConfigId;
        [BsonIgnore]
        public BuffConfig Config
        {
            get => BuffConfigCategory.Instance.Get(ConfigId);
        }

        /// <summary>
        /// 持续到什么时间
        /// </summary>
        public long Timestamp;
        
        [BsonIgnore]
        public long TimerId;
    }
}