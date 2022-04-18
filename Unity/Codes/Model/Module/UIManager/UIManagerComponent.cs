using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public class UIManagerComponent : Entity,IAwake,IDestroy,ILoad
    {
        public static UIManagerComponent Instance;
        public Dictionary<string, UIWindow> windows;//所有存活的窗体  {ui_name:window}
        public Dictionary<UILayerNames, LinkedList<string>> window_stack;//窗口记录队列
        public int MaxOderPerWindow = 10;
        public float ScreenSizeflag;
        public float WidthPadding;
        public Dictionary<long, Dictionary<string, Dictionary<Type, Entity>>> componentsMap = new Dictionary<long, Dictionary<string, Dictionary<Type, Entity>>>();
        public Dictionary<long, int> lengthMap = new Dictionary<long, int>();
        public Dictionary<long, string> pathMap = new Dictionary<long, string>();
    }
}
