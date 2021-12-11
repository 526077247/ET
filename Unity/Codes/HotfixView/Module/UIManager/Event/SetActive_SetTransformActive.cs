namespace ET
{
    public class SetActive_SetTransformActive : AEvent<UIEventType.SetActive>
    {
        protected override async ETTask Run(UIEventType.SetActive args)
        {
            args.entity.GetGameObject()?.SetActive(args.Active);
            await ETTask.CompletedTask;
        }
    }
}