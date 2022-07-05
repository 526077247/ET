namespace ET
{
    // 进入视野通知
    [Event]
    public class UnitEnterAOIRegisterUnit_NotifyClient: AEvent<EventType.AOIRegisterUnit>
    {
        protected override void Run(EventType.AOIRegisterUnit args)
        {
            
            AOIUnitComponent a = args.Receive;
            if (args.Units==null||args.Units.Count==0) return;
            Unit ua = a.GetParent<Unit>();
            if (ua.Type != UnitType.Player)
            {
                return;
            }

            UnitHelper.NoticeUnitsAdd(ua, args.Units);
        }
    }
}