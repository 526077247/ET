namespace ET
{
    [FriendClass(typeof(AreaComponent))]
    public static class AreaComponentSystem
    {
        public class AwakeSystem: AwakeSystem<AreaComponent,string>
        {
            public override void Awake(AreaComponent self,string name)
            {
                self.AreaConfigCategory = AreaConfigComponent.Instance.Get(name);
            }
        }
    }
}