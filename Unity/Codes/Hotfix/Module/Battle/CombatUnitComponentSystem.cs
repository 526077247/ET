using UnityEngine;
using System.Collections.Generic;
namespace ET
{
    [ObjectSystem]
    public class CombatUnitAwakeSystem : AwakeSystem<CombatUnitComponent,Unit,List<int>>
    {
        public override void Awake(CombatUnitComponent self,Unit unit,List<int> skills)
        {
            self.unit = unit;
            self.AddComponent<SpellComponent>();//技能施法组件
            for (int i = 0; i < skills.Count; i++)
            {
                if (skills[i] != 0)
                {
                    if (SkillConfigCategory.Instance.Contain(skills[i]))
                    {
                        self.AttachSkill(skills[i]);
                        Log.Info("添加技能" + skills[i]);
                    }
                    else
                    {
                        Log.Error(skills[i] + "技能未配置");
                    }
                }
            }
            self.AddComponent<BuffComponent>();//buff容器组件
            EventSystem.Instance.Publish(new EventType.AfterCombatUnitComponentCreate
            {
                CombatUnitComponent = self
            });
        }
    }
    [ObjectSystem]
    public class CombatUnitAwakeSystem1 : AwakeSystem<CombatUnitComponent,Unit>
    {
        public override void Awake(CombatUnitComponent self,Unit unit)
        {
            self.unit = unit;
            self.AddComponent<SpellComponent>();//技能施法组件
            self.AddComponent<BuffComponent>();//buff容器组件
            EventSystem.Instance.Publish(new EventType.AfterCombatUnitComponentCreate
            {
                CombatUnitComponent = self
            });
        }
    }
    [ObjectSystem]
    public class CombatUnitDestroySystem : DestroySystem<CombatUnitComponent>
    {
        public override void Destroy(CombatUnitComponent self)
        {
            self.IdSkills.Clear();
        }
    }
    public static class CombatUnitComponentSystem
    {
        /// <summary>
        /// 添加技能
        /// </summary>
        /// <param name="self"></param>
        /// <param name="configId"></param>
        /// <returns></returns>
        public static SkillAbility AttachSkill(this CombatUnitComponent self,int configId)
        {
            if (!self.IdSkills.ContainsKey(configId))
            {
                var skill = self.AddChild<SkillAbility, int>(configId);
                self.IdSkills.Add(configId, skill);
            }
            return self.IdSkills[configId];
        }
    }
}