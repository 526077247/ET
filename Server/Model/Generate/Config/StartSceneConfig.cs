using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Nino.Serialization;

namespace ET
{
    [NinoSerialize]
    [Config]
    public partial class StartSceneConfigCategory : ProtoObject, IMerge
    {
        public static StartSceneConfigCategory Instance;
		
        [NinoIgnore]
        [BsonIgnore]
        private Dictionary<int, StartSceneConfig> dict = new Dictionary<int, StartSceneConfig>();
		
        [BsonElement]
        [NinoMember(1)]
        private List<StartSceneConfig> list = new List<StartSceneConfig>();
		
        public StartSceneConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            StartSceneConfigCategory s = o as StartSceneConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                StartSceneConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
            }            
            this.AfterEndInit();
        }
		
        public StartSceneConfig Get(int id)
        {
            this.dict.TryGetValue(id, out StartSceneConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (StartSceneConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, StartSceneConfig> GetAll()
        {
            return this.dict;
        }
        public List<StartSceneConfig> GetAllList()
        {
            return this.list;
        }
        public StartSceneConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class StartSceneConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>所属进程</summary>
		[NinoMember(2)]
		public int Process { get; set; }
		/// <summary>所属区</summary>
		[NinoMember(3)]
		public int Zone { get; set; }
		/// <summary>类型</summary>
		[NinoMember(4)]
		public string SceneType { get; set; }
		/// <summary>名字</summary>
		[NinoMember(5)]
		public string Name { get; set; }
		/// <summary>外网端口</summary>
		[NinoMember(6)]
		public int OuterPort { get; set; }

	}
}
