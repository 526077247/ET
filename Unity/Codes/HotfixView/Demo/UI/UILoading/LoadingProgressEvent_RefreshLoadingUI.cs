namespace ET
{
    public class LoadingProgressEvent_RefreshLoadingUI : AEvent<UIEventType.LoadingProgress>
    {
        protected override async ETTask Run(UIEventType.LoadingProgress args)
        {
            if(UILoadingView.Instance!=null)
                UILoadingView.Instance.SetSlidValue(args.Progress);
            await ETTask.CompletedTask;
        }
    }
}
