namespace ET
{
    [UISystem]
    public class UIStageRoleInfoOnCreateSystem:OnCreateSystem<UIStageRoleInfo>
    {
        public override void OnCreate(UIStageRoleInfo self)
        {
            self.image = self.AddUIComponent<UIImage>("Image");
        }
    }
}