using System.Collections.Generic;

namespace ET
{
    public class SkillStepComponent:Entity,IAwake,IDestroy
    {
        public static SkillStepComponent Instance;
        public DictionaryComponent<int, List<int>> TimeLine;
        public DictionaryComponent<int, List<int>> StepType;
        public DictionaryComponent<int, List<object[]>> Params;
    }
}