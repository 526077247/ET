namespace ET
{
    public class SceneChangeStart_AddComponent: AEvent<EventType.SceneChangeStart>
    {
        protected override async ETTask Run(EventType.SceneChangeStart args)
        {
            await ETTask.CompletedTask;
            
            Scene zoneScene = args.ZoneScene;
            SceneLoadComponent slc = EnterMap(zoneScene);
            if(args.Name=="Map")
                await SceneManagerComponent.Instance.SwitchScene(SceneNames.Map,slc:slc);

            await UIManagerComponent.Instance.DestroyWindow<UILoadingView>();
            args.ZoneScene.AddComponent<OperaComponent>();
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