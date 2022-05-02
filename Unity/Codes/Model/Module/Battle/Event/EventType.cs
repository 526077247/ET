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
        public long Value;//伤害最终值.正数少血，负数加血
        public int SkillId;
    }
    #endregion
}