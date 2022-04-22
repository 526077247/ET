using UnityEngine;

namespace ET
{
    public class ChangeRotation_TurnAOIUnit: AEventClass<EventType.ChangeRotation>
    {
        protected override void Run(object changeRotation)
        {
            EventType.ChangePosition args = changeRotation as EventType.ChangePosition;;
            AOIUnitComponent aoiUnitComponent = args?.Unit?.GetComponent<AOIUnitComponent>();
            aoiUnitComponent?.Turn(args.Unit.Rotation);
        }
    }
}