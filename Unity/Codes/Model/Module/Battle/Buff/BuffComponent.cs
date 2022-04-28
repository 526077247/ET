using System.Collections.Generic;

namespace ET
{
    [ChildType(typeof(Buff))]
    public class BuffComponent:Entity, IAwake, IDestroy
    {
        public Unit unit;
        public DictionaryComponent<int, Buff> Groups;
        public DictionaryComponent<int, int> ActionControls;
    }
}