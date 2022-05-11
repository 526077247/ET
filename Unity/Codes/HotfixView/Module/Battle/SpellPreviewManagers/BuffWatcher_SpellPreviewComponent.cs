namespace ET
{
    [BuffWatcher(ActionControlType.Spell,true)]
    public class BuffWatcher_SpellPreviewComponent_AddBeAttackBand:IBuffWatcher
    {
        public void Run(Unit unit)
        {
            var cc = unit.GetComponent<CombatUnitComponent>();
            if (cc!=null)
            {
                cc.GetComponent<SpellPreviewComponent>()?.SetEnable(false);
            }
        }
    }
    
    
    [BuffWatcher(ActionControlType.Spell,false)]
    public class BuffWatcher_SpellPreviewComponent_RemoveBeAttackBand:IBuffWatcher
    {
        public void Run(Unit unit)
        {
            var cc = unit.GetComponent<CombatUnitComponent>();
            if (cc!=null)
            {
                cc.GetComponent<SpellPreviewComponent>()?.SetEnable(true);
                
            }
        }
    }
}