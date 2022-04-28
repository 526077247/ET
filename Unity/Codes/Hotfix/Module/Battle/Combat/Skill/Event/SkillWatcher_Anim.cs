namespace ET
{
    /// <summary>
    /// 播动画
    /// </summary>
    [SkillWatcher(SkillStepType.Anim)]
    [FriendClass(typeof(CombatUnitComponent))]
    public class SkillWatcher_Anim : ISkillWatcher
    {
        public void Run(SkillPara para)
        {
            var unit = para.From.unit;
        }
        
    }
}