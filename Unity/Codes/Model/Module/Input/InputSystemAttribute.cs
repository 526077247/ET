using System;
namespace ET
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true)]
    public class InputSystemAttribute: BaseAttribute
    {
        public int KeyCode { get; }
        public int Priority { get; }
        public int InputType{ get; }

        public InputSystemAttribute(int keyCode,int inputType,int priority=0)
        {
            this.KeyCode = keyCode;
            this.Priority = priority;
            this.InputType = inputType;
        }
    }
}