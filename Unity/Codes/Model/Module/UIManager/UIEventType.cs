﻿using System;
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

        //加载界面
        public struct LoadingBegin
        {

        }
        public struct LoadingProgress
        {
            public float Progress;
        }

        public struct LoadingFinish
        {
            public string[] cleanup_besides_path;
        }
        //即时消息弹出框
        public struct ShowToast
        {
            public Scene Scene;
            public string Text;
        }
        //新创建UI组件
        public struct AddComponent
        {
            public UIBaseContainer entity;
        } 
        public struct SetActive
        {
            public UIBaseContainer entity;
            public bool Active;
        } 
    }
}
