using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class BuffBleedConfigCategory : ProtoObject, IMerge
    {
        public static BuffBleedConfigCategory Instance;
		
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<int, BuffBleedConfig> dict = new Dictionary<int, BuffBleedConfig>();
		
        [BsonElement]
        [ProtoMember(1)]
        private List<BuffBleedConfig> list = new List<BuffBleedConfig>();
		
        public BuffBleedConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            BuffBleedConfigCategory s = o as BuffBleedConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                BuffBleedConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
            }            
            this.AfterEndInit();
        }
		
        public BuffBleedConfig Get(int id)
        {
            this.dict.TryGetValue(id, out BuffBleedConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (BuffBleedConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, BuffBleedConfig> GetAll()
        {
            return this.dict;
        }
        public List<BuffBleedConfig> GetAllList()
        {
            return this.list;
        }
        public BuffBleedConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [ProtoContract]
	public partial class BuffBleedConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		[ProtoMember(1)]
		public int Id { get; set; }
		/// <summary>计算方式</summary>
		/// <summary> 0=固定值(非千分比)</summary>
		/// <summary> 1=自身物理攻击力比例(千分比)</summary>
		/// <summary> 2=自身魔法攻击力比例(千分比)</summary>
		/// <summary> 3=自身物理防御力比例(千分比)</summary>
		/// <summary> 4=自身魔法防御力比例(千分比)</summary>
		/// <summary> 5=自身生命值上限比例(千分比)</summary>
		/// <summary> 6=自身已损失生命值比例(千分比)</summary>
		/// <summary> 7=自身当前失生命值比例(千分比)</summary>
		/// <summary> 8=目标物理攻击力比例(千分比)</summary>
		/// <summary> 9=目标魔法攻击力比例(千分比)</summary>
		/// <summary> 10=目标物理防御力比例(千分比)</summary>
		/// <summary> 11=目标魔法防御力比例(千分比)</summary>
		/// <summary> 12=目标生命值上限比例(千分比)</summary>
		/// <summary> 13=目标已损失生命值比例(千分比)</summary>
		/// <summary> 14=目标当前失生命值比例(千分比)</summary>
		[ProtoMember(2)]
		public int OperateType { get; set; }
		/// <summary>出血间隔</summary>
		[ProtoMember(3)]
		public int CD { get; set; }

	}
}
