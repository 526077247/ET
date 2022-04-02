namespace ET
{
    public class SceneChangeStart_AddComponent: AEvent<EventType.SceneChangeStart>
    {
        protected override void Run(EventType.SceneChangeStart args)
        {
            RunAsync(args).Coroutine();
        }
        
        private async ETTask RunAsync(EventType.SceneChangeStart args)
        {
            Scene currentScene = args.ZoneScene.CurrentScene();
            SceneLoadComponent slc = EnterMap(currentScene);
            await SceneManagerComponent.Instance.SwitchScene(args.Name,slc:slc);
            
            await UIManagerComponent.Instance.DestroyWindow<UILoadingView>();
            currentScene.AddComponent<OperaComponent>();
            await UIManagerComponent.Instance.OpenWindow<UIHelpWin>(UIHelpWin.PrefabPath);
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