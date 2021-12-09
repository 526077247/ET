using System;
namespace ET
{
    public class UISettingItem:Entity
    {
        public UIButton Button;
        public UIText Text;
        public ServerConfig Data;
        public Action<int> Callback;
    }
}