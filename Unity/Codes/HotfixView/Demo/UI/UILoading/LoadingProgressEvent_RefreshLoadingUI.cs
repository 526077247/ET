namespace ET
{
    public class LoadingProgressEvent_RefreshLoadingUI : AEvent<EventType.LoadingProgress>
    {
        protected override async ETTask Run(EventType.LoadingProgress args)
        {
            if(UILoadingView.Instance!=null)
                UILoadingView.Instance.SetSlidValue(args.Progress);
            await ETTask.CompletedTask;
        }
    }
}
