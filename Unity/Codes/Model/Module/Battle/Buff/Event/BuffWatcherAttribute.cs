namespace ET
{
    public class BuffWatcherAttribute : BaseAttribute
    {
        public int BuffType { get; }
        public bool IsAdd{ get; }

        public BuffWatcherAttribute(int type,bool isAdd)
        {
            this.BuffType = type;
            this.IsAdd = isAdd;
        }
    }
}