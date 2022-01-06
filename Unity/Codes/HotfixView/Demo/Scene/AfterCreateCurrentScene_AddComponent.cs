namespace ET
{
    public class AfterCreateCurrentScene_AddComponent: AEvent<EventType.AfterCreateCurrentScene>
    {
        protected override async ETTask Run(EventType.AfterCreateCurrentScene args)
        {
            Scene zoneScene = args.CurrentScene;

            await ETTask.CompletedTask;
        }
    }
}