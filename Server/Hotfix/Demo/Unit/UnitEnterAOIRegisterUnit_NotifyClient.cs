namespace ET
{
    // 进入视野通知
    [Event]
    public class UnitEnterAOIRegisterUnit_NotifyClient: AEvent<EventType.AOIRegisterUnit>
    {
        protected override void Run(EventType.AOIRegisterUnit args)
        {
            
            AOIUnitComponent a = args.Receive;
            AOIUnitComponent b = args.Unit;
            if (a == b) return;
            Unit ua = a.GetParent<Unit>();
            if (ua.Type != UnitType.Player)
            {
                return;
            }

            Unit ub = b.GetParent<Unit>();

            UnitHelper.NoticeUnitAdd(ua, ub);
        }
    }
}