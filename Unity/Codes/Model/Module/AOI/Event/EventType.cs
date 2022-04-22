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

        #endregion
    }
}