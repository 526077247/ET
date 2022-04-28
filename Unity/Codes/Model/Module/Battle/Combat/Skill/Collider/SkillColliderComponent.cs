using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
    public class SkillColliderComponent:Entity,IAwake<int,CombatUnitComponent>
    {
        public int ConfigId;
        [BsonIgnore]
        public SkillJudgeConfig Config => SkillJudgeConfigCategory.Instance.Get(ConfigId);
        /// <summary>
        /// 来源
        /// </summary>
        public CombatUnitComponent From;
    }
}