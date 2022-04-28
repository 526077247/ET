using System.Collections.Generic;
namespace ET
{
    public class SkillAbility:Entity,IAwake<int>,IDestroy
    {
        public int ConfigId;
        public SkillConfig SkillConfig => SkillConfigCategory.Instance.Get(ConfigId);

        public long LastSpellTime;//上次施法时间
        public long LastSpellOverTime;//上次施法完成时间

        public List<int> TimeLine;
        public List<int> StepType;
        public List<object[]> Paras;
    }
}