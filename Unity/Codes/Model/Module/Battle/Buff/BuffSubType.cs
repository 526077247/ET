namespace ET
{
    public static class BuffSubType
    {
        /// <summary>
        /// 属性修饰
        /// </summary>
        public const int Attribute = 1 << 0;
        /// <summary>
        /// 行为禁制
        /// </summary>
        public const int ActionControl = 1 << 1;
        /// <summary>
        /// 持续掉血
        /// </summary>
        public const int Bleed = 1 << 2;
    }
}