namespace ET
{
    [BuffWatcher(ActionControlType.Move,true)]
    public class BuffWatcher_MoveComponent_AddMoveBand:IBuffWatcher
    {
        public void Run(Unit unit,Buff buff)
        {
            var mc = unit.GetComponent<MoveComponent>();
            if (mc!=null)
            {
                mc.Enable = false;
                Log.Info(unit.Id+" Enable = false");
            }
        }
    }
    
    
    [BuffWatcher(ActionControlType.Move,false)]
    public class BuffWatcher_MoveComponent_RemoveMoveBand:IBuffWatcher
    {
        public void Run(Unit unit,Buff buff)
        {
            var mc = unit.GetComponent<MoveComponent>();
            if (mc!=null)
            {
                mc.Enable = true;
                Log.Info(unit.Id+" Enable = true");
            }
        }
    }
}