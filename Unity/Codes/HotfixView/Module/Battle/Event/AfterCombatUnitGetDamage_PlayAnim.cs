namespace ET
{
    [FriendClass(typeof(CombatUnitComponent))]
    public class AfterCombatUnitGetDamage_PlayAnim:AEvent<EventType.AfterCombatUnitGetDamage>
    {
        protected override void Run(EventType.AfterCombatUnitGetDamage args)
        {
            var anim = args.CombatUnitComponent.unit.GetComponent<AnimatorComponent>();
            if (anim != null)
            {
                if(args.CombatUnitComponent.unit.GetComponent<NumericComponent>().GetAsInt(NumericType.Hp)<=0)
                {
                    anim.Play(MotionType.Died);
                }
                else
                    anim.Play(MotionType.Damage);
            }
            else if(args.CombatUnitComponent.unit.GetComponent<NumericComponent>().GetAsInt(NumericType.Hp)<=0)//直接死了
            {
                args.CombatUnitComponent.unit.Dispose();
            }
            
        }
        
    }
}