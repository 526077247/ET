namespace ET
{
    [UISystem]
    public class UIHelpWinOnCreateSystem : OnCreateSystem<UIHelpWin>
    {
        public override void OnCreate(UIHelpWin self)
        {
            self.text = self.AddUIComponent<UIText>("Text");
        }
    }
    public class UIHelpWinSystem
    {
        
    }
}