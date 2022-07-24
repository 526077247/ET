using System.Collections.Generic;
namespace ET.EventType
{
    #region Battle

    public struct AfterCombatUnitComponentCreate
    {
        public CombatUnitComponent CombatUnitComponent;
    }
    /// <summary>
    /// 当受到伤害或回复
    /// </summary>
    public struct AfterCombatUnitGetDamage
    {
        public CombatUnitComponent From;
        public CombatUnitComponent Unit;
        public long DamageValue;//计算伤害值
        public long RealValue;//生命变化值.正数少血，负数加血
    }
    
    /// <summary>
    /// 当技能触发
    /// </summary>
    public struct OnSkillTrigger
    {
        public AOITriggerType Type;
        public AOIUnitComponent From;
        public AOIUnitComponent To;
        public SkillStepPara Para;
        public List<int> CostId;
        public List<int> Cost;
        public SkillConfig Config;
    }
    #endregion
}