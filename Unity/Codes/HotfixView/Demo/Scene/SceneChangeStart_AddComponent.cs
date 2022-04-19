namespace ET
{
    [FriendClass(typeof(SceneLoadComponent))]
    public class SceneChangeStart_AddComponent: AEventAsync<EventType.SceneChangeStart>
    {
        protected override async ETTask Run(EventType.SceneChangeStart args)
        {
            Scene currentScene = args.ZoneScene.CurrentScene();
            SceneLoadComponent slc = EnterMap(currentScene);
            await SceneManagerComponent.Instance.SwitchScene(args.Name,slc:slc);
            currentScene.AddComponent<OperaComponent>();
            slc.Dispose();
        }

        public SceneLoadComponent EnterMap(Entity self)
        {
            var slc = self.AddComponent<SceneLoadComponent>();
            var role = UnitConfigCategory.Instance.GetAll();
            foreach (var item in role)
                slc.PreLoadTask.Add(slc.AddPreloadGameObject(item.Value.Perfab, 1));
            return slc;
        }
    }
}