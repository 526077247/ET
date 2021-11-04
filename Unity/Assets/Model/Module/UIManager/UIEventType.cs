using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    namespace UIEventType
    {
        public struct AfterUIManagerCreate
        {

        }

        public struct InnerDestroyWindow
        {
            public UIWindow target;
        }

        public struct InitWindow
        {
            public UIBaseContainer uibaseview;
            public UIWindow window;
            public string name;
        }

        public struct InnerOpenWindow
        {
            public UIWindow window;
            public string path;
        }
        public struct ResetWindowLayer
        {
            public UIWindow window;
        }
        public struct AddWindowToStack
        {
            public UIWindow window;
        }
    }
}
