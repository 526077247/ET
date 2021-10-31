namespace ET
{
    namespace EventType
    {
        public struct AppStart
        {
        }

        public struct ChangePosition
        {
            public Unit Unit;
        }

        public struct ChangeRotation
        {
            public Unit Unit;
        }

        public struct PingChange
        {
            public Scene ZoneScene;
            public long Ping;
        }
        
        public struct AfterCreateZoneScene
        {
            public Scene ZoneScene;
        }
        
        public struct AfterCreateLoginScene
        {
            public Scene LoginScene;
        }

        public struct AppStartInitFinish
        {
            public Scene ZoneScene;
        }

        public struct LoginFinish
        {
            public Scene ZoneScene;
        }

        public struct EnterMapFinish
        {
            public Scene ZoneScene;
        }

        public struct AfterUnitCreate
        {
            public Unit Unit;
        }
        
        public struct MoveStart
        {
            public Unit Unit;
        }

        public struct MoveStop
        {
            public Unit Unit;
        }

        #region UI
        public struct DestroyWindowExceptNames
        {
            public string[] names;
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


        #endregion
    }
}