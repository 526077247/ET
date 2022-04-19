namespace ET
{
    [UISystem]
    [FriendClass(typeof(UIHelpWin))]
    public class UIHelpWinOnCreateSystem : OnCreateSystem<UIHelpWin>
    {
        public override void OnCreate(UIHelpWin self)
        {
            self.text = self.AddUIComponent<UIText>("Text");
            self.GalBtn = self.AddUIComponent<UIButton>("Button");
            self.GalBtn.SetOnClick(()=>{self.OnGalBtnClick();});
        }
    }
    public static class UIHelpWinSystem
    {
        public static void OnGalBtnClick(this UIHelpWin self)
        {
            GalGameEngineComponent.Instance.PlayChapterByName("StartChapter").Coroutine();
        }
    }
}