using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class I18NConfigCategory : ProtoObject
    {
        public static I18NConfigCategory Instance;
		
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<int, I18NConfig> dict = new Dictionary<int, I18NConfig>();
		
        [BsonElement]
        [ProtoMember(1)]
        private List<I18NConfig> list = new List<I18NConfig>();
		
        public I18NConfigCategory()
        {
            Instance = this;
        }
		
        public override void EndInit()
        {
            foreach (I18NConfig config in list)
            {
                config.EndInit();
                this.dict.Add(config.Id, config);
            }            
            this.AfterEndInit();
        }
		
        public I18NConfig Get(int id)
        {
            this.dict.TryGetValue(id, out I18NConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (I18NConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, I18NConfig> GetAll()
        {
            return this.dict;
        }

        public I18NConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [ProtoContract]
	public partial class I18NConfig: ProtoObject, IConfig
	{
		[ProtoMember(1)]
		public int Id { get; set; }
		[ProtoMember(2)]
		public string Key { get; set; }
		[ProtoMember(3)]
		public string Chinese { get; set; }
		[ProtoMember(4)]
		public string English { get; set; }

	}
}
