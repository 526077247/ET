

namespace ET
{
    /// <summary>
    /// 技能施法组件
    /// </summary>
    public class SpellComponent : Entity,IAwake,IDestroy
    {
        /// <summary>
        /// 当前步骤
        /// </summary>
        public int CurrentSkillStep;

        /// <summary>
        /// 当前技能
        /// </summary>
        public SkillAbility Skill;
        /// <summary>
        /// 当前参数
        /// </summary>
        public SkillPara Para;
    }
}