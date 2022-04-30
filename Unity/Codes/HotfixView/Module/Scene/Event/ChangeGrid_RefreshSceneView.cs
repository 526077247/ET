namespace ET
{
    [FriendClass(typeof(AOIGrid))]
    public class ChangeGrid_RefreshSceneView: AEvent<EventType.ChangeGrid>
    {
        protected override void Run(EventType.ChangeGrid args)
        {
            if (args.Unit.Id == args.Unit.GetMyUnitIdFromZoneScene())
            {
                var nc =args.Unit.Parent.GetComponent<NumericComponent>();
                if(args.NewGrid==null) return;
                AOISceneViewComponent.Instance.ChangeGrid(args.NewGrid.posx,args.NewGrid.posy,nc.GetAsInt(NumericType.AOI)).Coroutine();
            }
        }
    }
}