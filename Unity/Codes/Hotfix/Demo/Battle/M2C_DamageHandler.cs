namespace ET
{
    [MessageHandler]
    public class M2C_DamageHandler : AMHandler<M2C_Damage>
    {
        protected override void Run(Session session, M2C_Damage message)
        {
            UnitComponent uc = session.DomainScene().CurrentScene().GetComponent<UnitComponent>();
            Log.Info(message.FromId+"使用"+message.ConfigId+"对"+ message.ToId+"造成"+message.Damage+"点伤害");
            var unit = uc.Get(message.ToId);
            if (unit != null)
            {
                var t = unit.GetComponent<NumericComponent>();
                int now = t.GetAsInt(NumericType.HpBase);
                if (now < message.Damage)
                {
                    t.Set(NumericType.HpBase,0);
                }
                else
                {
                    t.Set(NumericType.HpBase,now - message.Damage);
                }
                var from = uc.Get(message.FromId);
                EventSystem.Instance.Publish(new EventType.AfterCombatUnitGetDamage()
                {
                    Unit = unit.GetComponent<CombatUnitComponent>(),
                    From = from.GetComponent<CombatUnitComponent>(),
                    Value = message.Damage,
                    SkillId = message.ConfigId
                });
            }
        }

    }
}