namespace ET
{
    [FriendClass(typeof(AOICell))]
    public class ChangeGrid_RefreshSceneView: AEvent<EventType.ChangeGrid>
    {
        protected override void Run(EventType.ChangeGrid args)
        {
            if (args.Unit.Id == args.Unit.GetMyUnitIdFromZoneScene())
            {
                var nc =args.Unit.Parent.GetComponent<NumericComponent>();
                if(args.NewCell==null) return;
                AOISceneViewComponent.Instance.ChangeGrid(args.Unit.ZoneScene(), args.NewCell.posx,args.NewCell.posy,nc.GetAsInt(NumericType.AOI)).Coroutine();
            }
        }
    }
}