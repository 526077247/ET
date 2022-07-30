using System.Collections.Generic;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class InputComponent:Entity,IAwake,IUpdate
    {
        public static InputComponent Instance;
        public List<int> KeysForListen;
    }
}