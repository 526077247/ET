using System;
namespace ET
{
    public class UISettingItem:UIBaseContainer
    {
        public UIButton Button;
        public UIText Text;
        public ServerConfig Data;
        public Action<int> Callback;
    }
}