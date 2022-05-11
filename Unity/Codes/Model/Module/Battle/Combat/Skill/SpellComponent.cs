
using MongoDB.Bson.Serialization.Attributes;
namespace ET
{
    /// <summary>
    /// 技能施法组件
    /// </summary>
    public class SpellComponent : Entity,IAwake,IDestroy,ITransfer
    {
       
        /// <summary>
        /// 当前步骤
        /// </summary>
        [BsonIgnore]
        public int CurrentSkillStep;
        /// <summary>
        /// 当前技能
        /// </summary>
        [BsonIgnore]
        public SkillAbility Skill;
        /// <summary>
        /// 当前参数
        /// </summary>
        [BsonIgnore]
        public SkillPara Para = new SkillPara();

        /// <summary>
        /// 是否生效
        /// </summary>
        public bool Enable;

        public long TimerId;
    }
}