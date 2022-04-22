namespace ET
{
    // 离开视野
    [Event]
    public class AOIRemoveUnit_NotifyClient: AEvent<EventType.AOIRemoveUnit>
    {
        protected override void Run(EventType.AOIRemoveUnit args)
        {
            AOIUnitComponent a = args.Receive;
            AOIUnitComponent b = args.Unit;
            if (a.GetParent<Unit>().Type != UnitType.Player)
            {
                return;
            }

            UnitHelper.NoticeUnitRemove(a.GetParent<Unit>(), b.GetParent<Unit>());
        }
    }
}