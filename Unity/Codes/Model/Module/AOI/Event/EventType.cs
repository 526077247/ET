namespace ET
{
    namespace EventType
    {
        #region AOI

        public struct AOIRemoveUnit
        {
            public AOIUnitComponent Receive;
            public AOIUnitComponent Unit;
        }

        public struct AOIRegisterUnit
        {
            public AOIUnitComponent Receive;
            public AOIUnitComponent Unit;
        }

        public struct ChangeGrid
        {
            public AOIUnitComponent Unit;
            public AOIGrid NewGrid;
            public AOIGrid OldGrid;
        }
        #endregion
    }
}