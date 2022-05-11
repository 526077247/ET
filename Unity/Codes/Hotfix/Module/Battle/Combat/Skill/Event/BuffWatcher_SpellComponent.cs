namespace ET
{
    [BuffWatcher(ActionControlType.Spell,true)]
    public class BuffWatcher_SpellComponent_AddBeAttackBand:IBuffWatcher
    {
        public void Run(Unit unit)
        {
            var cc = unit.GetComponent<CombatUnitComponent>();
            if (cc!=null)
            {
                cc.GetComponent<SpellComponent>()?.SetEnable(false);
            }
        }
    }
    
    
    [BuffWatcher(ActionControlType.Spell,false)]
    public class BuffWatcher_SpellComponent_RemoveBeAttackBand:IBuffWatcher
    {
        public void Run(Unit unit)
        {
            var cc = unit.GetComponent<CombatUnitComponent>();
            if (cc!=null)
            {
                cc.GetComponent<SpellComponent>()?.SetEnable(true);
                
            }
        }
    }
}