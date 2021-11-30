namespace ET
{
    public class AddComponent_SetUITransform : AEvent<UIEventType.AddComponent>
    {
        protected override async ETTask Run(UIEventType.AddComponent args)
        {
            if (args.entity.GetType() != typeof(UITransform))
            {
                args.entity.AddComponent<UITransform>("");
            }
            await ETTask.CompletedTask;
        }
    }
}