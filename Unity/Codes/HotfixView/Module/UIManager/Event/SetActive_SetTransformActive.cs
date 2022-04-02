namespace ET
{
    public class SetActive_SetTransformActive : AEvent<UIEventType.SetActive>
    {
        protected override void Run(UIEventType.SetActive args)
        {
            args.entity.GetGameObject()?.SetActive(args.Active);
        }
    }
}