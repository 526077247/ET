using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class MapAreaConfigCategory : ProtoObject, IMerge
    {
        public static MapAreaConfigCategory Instance;
		
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<int, MapAreaConfig> dict = new Dictionary<int, MapAreaConfig>();
		
        [BsonElement]
        [ProtoMember(1)]
        private List<MapAreaConfig> list = new List<MapAreaConfig>();
		
        public MapAreaConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            MapAreaConfigCategory s = o as MapAreaConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                MapAreaConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
            }            
            this.AfterEndInit();
        }
		
        public MapAreaConfig Get(int id)
        {
            this.dict.TryGetValue(id, out MapAreaConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (MapAreaConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, MapAreaConfig> GetAll()
        {
            return this.dict;
        }
        public List<MapAreaConfig> GetAllList()
        {
            return this.list;
        }
        public MapAreaConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [ProtoContract]
	public partial class MapAreaConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		[ProtoMember(1)]
		public int Id { get; set; }
		/// <summary>加载的Area数据表名</summary>
		[ProtoMember(2)]
		public string Area { get; set; }

	}
}
