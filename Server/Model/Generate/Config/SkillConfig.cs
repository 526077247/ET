using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Nino.Serialization;

namespace ET
{
    [NinoSerialize]
    [Config]
    public partial class SkillConfigCategory : ProtoObject, IMerge
    {
        public static SkillConfigCategory Instance;
		
        [NinoIgnore]
        [BsonIgnore]
        private Dictionary<int, SkillConfig> dict = new Dictionary<int, SkillConfig>();
		
        [BsonElement]
        [NinoMember(1)]
        private List<SkillConfig> list = new List<SkillConfig>();
		
        public SkillConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            SkillConfigCategory s = o as SkillConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                SkillConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
            }            
            this.AfterEndInit();
        }
		
        public SkillConfig Get(int id)
        {
            this.dict.TryGetValue(id, out SkillConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (SkillConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, SkillConfig> GetAll()
        {
            return this.dict;
        }
        public List<SkillConfig> GetAllList()
        {
            return this.list;
        }
        public SkillConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class SkillConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>伤害作用对象(0自身1己方2敌方)</summary>
		[NinoMember(2)]
		public int DamageTarget { get; set; }
		/// <summary>名字</summary>
		[NinoMember(3)]
		public string Name { get; set; }
		/// <summary>图标</summary>
		[NinoMember(4)]
		public string Icon { get; set; }
		/// <summary>稀有度</summary>
		[NinoMember(5)]
		public int RareLv { get; set; }
		/// <summary>可用等级</summary>
		[NinoMember(6)]
		public int Lv { get; set; }
		/// <summary>描述</summary>
		[NinoMember(7)]
		public string Description { get; set; }
		/// <summary>冷却时间</summary>
		[NinoMember(8)]
		public int CDTime { get; set; }
		/// <summary>施法模式（0：距离不够则选最大施法范围ps选目标的则不施法;1:距离不够走到最远距离施法）</summary>
		[NinoMember(9)]
		public int Mode { get; set; }
		/// <summary>技能预览类型(0大圈选一个目标，1大圈选小圈，2从脚底出发指向型……)</summary>
		[NinoMember(10)]
		public int PreviewType { get; set; }
		/// <summary>技能预览释放范围（0半径；1半径，小圈半径；2，长度，宽度）</summary>
		[NinoMember(11)]
		public int[] PreviewRange { get; set; }
		/// <summary>技能配置</summary>
		[NinoMember(12)]
		public string JsonFile { get; set; }

	}
}
