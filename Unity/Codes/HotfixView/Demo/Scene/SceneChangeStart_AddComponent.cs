namespace ET
{
    public class SceneChangeStart_AddComponent: AEvent<EventType.SceneChangeStart>
    {
        protected override async ETTask Run(EventType.SceneChangeStart args)
        {
            Scene currentScene = args.ZoneScene.CurrentScene();
            SceneLoadComponent slc = EnterMap(currentScene);
            if(args.Name=="Map1")
                await SceneManagerComponent.Instance.SwitchScene(SceneNames.Map1,slc:slc);
            else if(args.Name=="Map2")
                await SceneManagerComponent.Instance.SwitchScene(SceneNames.Map2,slc:slc);
            else
                Log.Error("args.Name: "+args.Name+" Not Found!!");
            await UIManagerComponent.Instance.DestroyWindow<UILoadingView>();
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