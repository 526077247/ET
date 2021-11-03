using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public class UIWindowComponent:Entity
    {
		public Dictionary<string, UIWindow> windows;//所有存活的窗体  {ui_name:window}
        public Dictionary<UILayerNames, LinkedList<string>> window_stack;//窗口记录队列
	}
}
