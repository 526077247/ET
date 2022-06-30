using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Nino.Serialization;

namespace ET
{
    [NinoSerialize]
    [Config]
    public partial class StagePosConfigCategory : ProtoObject, IMerge
    {
        public static StagePosConfigCategory Instance;
		
        [NinoIgnore]
        [BsonIgnore]
        private Dictionary<int, StagePosConfig> dict = new Dictionary<int, StagePosConfig>();
		
        [BsonElement]
        [NinoMember(1)]
        private List<StagePosConfig> list = new List<StagePosConfig>();
		
        public StagePosConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            StagePosConfigCategory s = o as StagePosConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                StagePosConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
            }            
            this.AfterEndInit();
        }
		
        public StagePosConfig Get(int id)
        {
            this.dict.TryGetValue(id, out StagePosConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (StagePosConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, StagePosConfig> GetAll()
        {
            return this.dict;
        }
        public List<StagePosConfig> GetAllList()
        {
            return this.list;
        }
        public StagePosConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class StagePosConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>位置关键字（不能重复）</summary>
		[NinoMember(2)]
		public string NameKey { get; set; }
		/// <summary>位置</summary>
		[NinoMember(3)]
		public float[] Position { get; set; }

	}
}
