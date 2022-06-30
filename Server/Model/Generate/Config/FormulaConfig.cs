using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Nino.Serialization;

namespace ET
{
    [NinoSerialize]
    [Config]
    public partial class FormulaConfigCategory : ProtoObject, IMerge
    {
        public static FormulaConfigCategory Instance;
		
        [NinoIgnore]
        [BsonIgnore]
        private Dictionary<int, FormulaConfig> dict = new Dictionary<int, FormulaConfig>();
		
        [BsonElement]
        [NinoMember(1)]
        private List<FormulaConfig> list = new List<FormulaConfig>();
		
        public FormulaConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            FormulaConfigCategory s = o as FormulaConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                FormulaConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
            }            
            this.AfterEndInit();
        }
		
        public FormulaConfig Get(int id)
        {
            this.dict.TryGetValue(id, out FormulaConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (FormulaConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, FormulaConfig> GetAll()
        {
            return this.dict;
        }
        public List<FormulaConfig> GetAllList()
        {
            return this.list;
        }
        public FormulaConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class FormulaConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>公式</summary>
		[NinoMember(2)]
		public string Formula { get; set; }

	}
}
