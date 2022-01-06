namespace ET
{
    public class SceneChangeStart_AddComponent: AEvent<EventType.SceneChangeStart>
    {
        protected override async ETTask Run(EventType.SceneChangeStart args)
        {
            await ETTask.CompletedTask;

            Scene zoneScene = args.ZoneScene;
            if(args.Name=="Map")
                await SceneManagerComponent.Instance.SwitchScene<MapScene>(SceneNames.Map);

            args.ZoneScene.AddComponent<OperaComponent>();
        }
    }
}