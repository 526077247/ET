using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class RedDotConfigCategory : ProtoObject
    {
        public static RedDotConfigCategory Instance;
		
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<int, RedDotConfig> dict = new Dictionary<int, RedDotConfig>();
		
        [BsonElement]
        [ProtoMember(1)]
        private List<RedDotConfig> list = new List<RedDotConfig>();
		
        public RedDotConfigCategory()
        {
            Instance = this;
        }
		
        public override void EndInit()
        {
            foreach (RedDotConfig config in list)
            {
                config.EndInit();
                this.dict.Add(config.Id, config);
            }            
            this.AfterEndInit();
        }
		
        public RedDotConfig Get(int id)
        {
            this.dict.TryGetValue(id, out RedDotConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (RedDotConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, RedDotConfig> GetAll()
        {
            return this.dict;
        }

        public RedDotConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [ProtoContract]
	public partial class RedDotConfig: ProtoObject, IConfig
	{
		[ProtoMember(1)]
		public int Id { get; set; }
		[ProtoMember(2)]
		public string Target { get; set; }
		[ProtoMember(3)]
		public string Parent { get; set; }

	}
}
