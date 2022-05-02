namespace ET
{
    public class AOIRemoveUnit_BeforeUnitRemove: AEvent<EventType.AOIRemoveUnit>
    {
        protected override void Run(EventType.AOIRemoveUnit args)
        {
            var myunitId = args.Unit.GetMyUnitIdFromZoneScene();
            if (args.Receive.Id != myunitId)
            {
                return;
            }
            if (args.Unit != null)
            {
                var unit = args.Unit.GetParent<Unit>();
                var combatU = unit.GetComponent<CombatUnitComponent>();
                if (combatU != null)
                {
                    combatU.GetComponent<BuffComponent>()?.HideAllBuffView();
                }
                unit.RemoveComponent<AnimatorComponent>();
                unit.RemoveComponent<InfoComponent>();
                // unit.RemoveComponent<NumberComponent>();
                unit.RemoveComponent<GameObjectComponent>();
            }
        }
   
    }
}