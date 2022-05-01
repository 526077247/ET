namespace ET
{
    public class AfterRemoveBuff_RemoveBuffView: AEvent<EventType.AfterRemoveBuff>
    {
        protected override void Run(EventType.AfterRemoveBuff args)
        {
            var goc = args.Buff.GetComponent<GameObjectComponent>();
            if (goc != null)
            {
                goc.Dispose();
            }
        }
        
    }
}