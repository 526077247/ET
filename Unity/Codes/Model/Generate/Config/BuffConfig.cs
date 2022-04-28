using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class BuffConfigCategory : ProtoObject, IMerge
    {
        public static BuffConfigCategory Instance;
		
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<int, BuffConfig> dict = new Dictionary<int, BuffConfig>();
		
        [BsonElement]
        [ProtoMember(1)]
        private List<BuffConfig> list = new List<BuffConfig>();
		
        public BuffConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            BuffConfigCategory s = o as BuffConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                BuffConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
            }            
            this.AfterEndInit();
        }
		
        public BuffConfig Get(int id)
        {
            this.dict.TryGetValue(id, out BuffConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (BuffConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, BuffConfig> GetAll()
        {
            return this.dict;
        }
        public List<BuffConfig> GetAllList()
        {
            return this.list;
        }
        public BuffConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [ProtoContract]
	public partial class BuffConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		[ProtoMember(1)]
		public int Id { get; set; }
		/// <summary>状态名称</summary>
		[ProtoMember(2)]
		public string Name { get; set; }
		/// <summary>状态描述</summary>
		[ProtoMember(3)]
		public string Description { get; set; }
		/// <summary>图标路径</summary>
		[ProtoMember(4)]
		public string Icon { get; set; }
		/// <summary>显示在状态栏</summary>
		[ProtoMember(5)]
		public int StatusSlot { get; set; }
		/// <summary>游戏特效表现</summary>
		[ProtoMember(6)]
		public string BuffObj { get; set; }
		/// <summary>表现位置(1:Head)</summary>
		[ProtoMember(7)]
		public int ObjRoot { get; set; }
		/// <summary>结束后是否移除加成（0:是）</summary>
		[ProtoMember(8)]
		public int IsRemove { get; set; }
		/// <summary>随时间变化方程（不填表示不随时间变化）</summary>
		[ProtoMember(9)]
		public string Equation { get; set; }
		/// <summary>叠加判别组(同组只取最高优先级)</summary>
		[ProtoMember(10)]
		public int Group { get; set; }
		/// <summary>优先级（数字越小越优先，相同则Id最小优先）</summary>
		[ProtoMember(11)]
		public int Priority { get; set; }
		/// <summary>属性修饰</summary>
		[ProtoMember(12)]
		public string[] AttributeType { get; set; }
		/// <summary>修饰参数</summary>
		[ProtoMember(13)]
		public int[] AttributePct { get; set; }
		/// <summary>修饰参数</summary>
		[ProtoMember(14)]
		public int[] AttributeAdd { get; set; }
		/// <summary>修饰参数</summary>
		[ProtoMember(15)]
		public int[] AttributeFinalAdd { get; set; }
		/// <summary>修饰参数</summary>
		[ProtoMember(16)]
		public int[] AttributeFinalPct { get; set; }
		/// <summary>行为禁制(1禁止施法，2禁止攻击，3禁止移动)</summary>
		[ProtoMember(17)]
		public int[] ActionControl { get; set; }

	}
}
