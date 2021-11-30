namespace ET
{
    public class SetActive_SetTransformActive : AEvent<UIEventType.SetActive>
    {
        protected override async ETTask Run(UIEventType.SetActive args)
        {
            var uitrans = args.entity.GetComponent<UITransform>();
            if (uitrans!=null)
            {
                uitrans.transform?.gameObject.SetActive(args.Active);
            }
            await ETTask.CompletedTask;
        }
    }
}