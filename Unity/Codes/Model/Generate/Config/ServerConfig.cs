using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class ServerConfigCategory : ProtoObject
    {
        public static ServerConfigCategory Instance;
		
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<int, ServerConfig> dict = new Dictionary<int, ServerConfig>();
		
        [BsonElement]
        [ProtoMember(1)]
        private List<ServerConfig> list = new List<ServerConfig>();
		
        public ServerConfigCategory()
        {
            Instance = this;
        }
		
        public override void EndInit()
        {
            foreach (ServerConfig config in list)
            {
                config.EndInit();
                this.dict.Add(config.Id, config);
            }            
            this.AfterEndInit();
        }
		
        public ServerConfig Get(int id)
        {
            this.dict.TryGetValue(id, out ServerConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (ServerConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, ServerConfig> GetAll()
        {
            return this.dict;
        }

        public ServerConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [ProtoContract]
	public partial class ServerConfig: ProtoObject, IConfig
	{
		[ProtoMember(1)]
		public int Id { get; set; }
		[ProtoMember(2)]
		public string Name { get; set; }
		[ProtoMember(3)]
		public string RealmIp { get; set; }
		[ProtoMember(4)]
		public string UpdateListUrl { get; set; }
		[ProtoMember(5)]
		public string RouterListUrl { get; set; }
		[ProtoMember(6)]
		public string ResUrl { get; set; }
		[ProtoMember(7)]
		public string TestUpdateListUrl { get; set; }
		[ProtoMember(8)]
		public int EnvId { get; set; }
		[ProtoMember(9)]
		public int IsPriority { get; set; }

	}
}
