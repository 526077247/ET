using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Nino.Serialization;

namespace ET
{
    [NinoSerialize]
    [Config]
    public partial class AIConfigCategory : ProtoObject, IMerge
    {
        public static AIConfigCategory Instance;
		
        [NinoIgnore]
        [BsonIgnore]
        private Dictionary<int, AIConfig> dict = new Dictionary<int, AIConfig>();
		
        [BsonElement]
        [NinoMember(1)]
        private List<AIConfig> list = new List<AIConfig>();
		
        public AIConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            AIConfigCategory s = o as AIConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                AIConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
            }            
            this.AfterEndInit();
        }
		
        public AIConfig Get(int id)
        {
            this.dict.TryGetValue(id, out AIConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (AIConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, AIConfig> GetAll()
        {
            return this.dict;
        }
        public List<AIConfig> GetAllList()
        {
            return this.list;
        }
        public AIConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class AIConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>所属ai</summary>
		[NinoMember(2)]
		public int AIConfigId { get; set; }
		/// <summary>此ai中的顺序</summary>
		[NinoMember(3)]
		public int Order { get; set; }
		/// <summary>节点名字</summary>
		[NinoMember(4)]
		public string Name { get; set; }
		/// <summary>节点参数</summary>
		[NinoMember(5)]
		public int[] NodeParams { get; set; }

	}
}
