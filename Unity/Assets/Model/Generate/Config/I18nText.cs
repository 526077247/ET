using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class I18nTextCategory : ProtoObject
    {
        public static I18nTextCategory Instance;
		
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<int, I18nText> dict = new Dictionary<int, I18nText>();
		
        [BsonElement]
        [ProtoMember(1)]
        private List<I18nText> list = new List<I18nText>();
		
        public I18nTextCategory()
        {
            Instance = this;
        }
		
		[ProtoAfterDeserialization]
        public void AfterDeserialization()
        {
            foreach (I18nText config in list)
            {
                this.dict.Add(config.Id, config);
            }
            list.Clear();
            this.EndInit();
        }
		
        public I18nText Get(int id)
        {
            this.dict.TryGetValue(id, out I18nText item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (I18nText)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, I18nText> GetAll()
        {
            return this.dict;
        }

        public I18nText GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [ProtoContract]
	public partial class I18nText: ProtoObject, IConfig
	{
		[ProtoMember(1, IsRequired  = true)]
		public int Id { get; set; }
		[ProtoMember(2, IsRequired  = true)]
		public string Key { get; set; }
		[ProtoMember(3, IsRequired  = true)]
		public string Chinese { get; set; }
		[ProtoMember(4, IsRequired  = true)]
		public string English { get; set; }


		[ProtoAfterDeserialization]
        public void AfterDeserialization()
        {
            this.EndInit();
        }
	}
}
