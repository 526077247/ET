using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Nino.Serialization;

namespace ET
{
    [NinoSerialize]
    [Config]
    public partial class SkillStepConfigCategory : ProtoObject, IMerge
    {
        public static SkillStepConfigCategory Instance;
		
        [NinoIgnore]
        [BsonIgnore]
        private Dictionary<int, SkillStepConfig> dict = new Dictionary<int, SkillStepConfig>();
		
        [BsonElement]
        [NinoMember(1)]
        private List<SkillStepConfig> list = new List<SkillStepConfig>();
		
        public SkillStepConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            SkillStepConfigCategory s = o as SkillStepConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                SkillStepConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
            }            
            this.AfterEndInit();
        }
		
        public SkillStepConfig Get(int id)
        {
            this.dict.TryGetValue(id, out SkillStepConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (SkillStepConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, SkillStepConfig> GetAll()
        {
            return this.dict;
        }
        public List<SkillStepConfig> GetAllList()
        {
            return this.list;
        }
        public SkillStepConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class SkillStepConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>参数数量</summary>
		[NinoMember(2)]
		public int ParaCount { get; set; }
		/// <summary>时间节点0</summary>
		[NinoMember(3)]
		public int TriggerTime0 { get; set; }
		/// <summary>步骤类型0</summary>
		[NinoMember(4)]
		public int StepStyle0 { get; set; }
		/// <summary>步骤参数0</summary>
		[NinoMember(5)]
		public string[] StepParameter0 { get; set; }
		/// <summary>时间节点1</summary>
		[NinoMember(6)]
		public int TriggerTime1 { get; set; }
		/// <summary>步骤类型1</summary>
		[NinoMember(7)]
		public int StepStyle1 { get; set; }
		/// <summary>步骤参数1</summary>
		[NinoMember(8)]
		public string[] StepParameter1 { get; set; }
		/// <summary>时间节点2</summary>
		[NinoMember(9)]
		public int TriggerTime2 { get; set; }
		/// <summary>步骤类型2</summary>
		[NinoMember(10)]
		public int StepStyle2 { get; set; }
		/// <summary>步骤参数2</summary>
		[NinoMember(11)]
		public string[] StepParameter2 { get; set; }
		/// <summary>时间节点3</summary>
		[NinoMember(12)]
		public int TriggerTime3 { get; set; }
		/// <summary>步骤类型3</summary>
		[NinoMember(13)]
		public int StepStyle3 { get; set; }
		/// <summary>步骤参数3</summary>
		[NinoMember(14)]
		public string[] StepParameter3 { get; set; }
		/// <summary>时间节点4</summary>
		[NinoMember(15)]
		public int TriggerTime4 { get; set; }
		/// <summary>步骤类型4</summary>
		[NinoMember(16)]
		public int StepStyle4 { get; set; }
		/// <summary>步骤参数4</summary>
		[NinoMember(17)]
		public string[] StepParameter4 { get; set; }
		/// <summary>时间节点5</summary>
		[NinoMember(18)]
		public int TriggerTime5 { get; set; }
		/// <summary>步骤类型5</summary>
		[NinoMember(19)]
		public int StepStyle5 { get; set; }
		/// <summary>步骤参数5</summary>
		[NinoMember(20)]
		public string[] StepParameter5 { get; set; }
		/// <summary>时间节点6</summary>
		[NinoMember(21)]
		public int TriggerTime6 { get; set; }
		/// <summary>步骤类型6</summary>
		[NinoMember(22)]
		public int StepStyle6 { get; set; }
		/// <summary>步骤参数6</summary>
		[NinoMember(23)]
		public string[] StepParameter6 { get; set; }
		/// <summary>时间节点7</summary>
		[NinoMember(24)]
		public int TriggerTime7 { get; set; }
		/// <summary>步骤类型7</summary>
		[NinoMember(25)]
		public int StepStyle7 { get; set; }
		/// <summary>步骤参数7</summary>
		[NinoMember(26)]
		public string[] StepParameter7 { get; set; }
		/// <summary>时间节点8</summary>
		[NinoMember(27)]
		public int TriggerTime8 { get; set; }
		/// <summary>步骤类型8</summary>
		[NinoMember(28)]
		public int StepStyle8 { get; set; }
		/// <summary>步骤参数8</summary>
		[NinoMember(29)]
		public string[] StepParameter8 { get; set; }
		/// <summary>时间节点9</summary>
		[NinoMember(30)]
		public int TriggerTime9 { get; set; }
		/// <summary>步骤类型9</summary>
		[NinoMember(31)]
		public int StepStyle9 { get; set; }
		/// <summary>步骤参数9</summary>
		[NinoMember(32)]
		public string[] StepParameter9 { get; set; }

	}
}
