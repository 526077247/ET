namespace ET.EventType
{
    #region Battle

    public struct AfterCombatUnitComponentCreate
    {
        public CombatUnitComponent CombatUnitComponent;
    }
    /// <summary>
    /// 当受到伤害
    /// </summary>
    public struct AfterCombatUnitGetDamage
    {
        public CombatUnitComponent CombatUnitComponent;
    }
    /// <summary>
    /// 伤害飘字
    /// </summary>
    public struct EmergingDamageText
    {
        public Unit Unit;
        public int Type;//伤害类型
        public int Value;//伤害最终值
    }
    #endregion
}