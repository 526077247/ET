using UnityEngine;

namespace ET
{
    [MessageHandler]
    public class M2C_UseSkillHandler : AMHandler<M2C_UseSkill>
    {
        protected override void Run(Session session, M2C_UseSkill message)
        {
            UnitComponent uc = session.DomainScene().CurrentScene().GetComponent<UnitComponent>();
            Unit sender = uc.Get(message.Sender);
            if (sender == null)
            {
                return;
            }
            
            var combatU = sender.GetComponent<CombatUnitComponent>();
            if (combatU != null&&combatU.TryGetSkillAbility(message.SkillConfigId,out SkillAbility skill))
            {
                if (skill.SkillConfig.PreviewType == SkillPreviewType.SelectCircularInCircularArea)
                {
                    combatU.GetComponent<SpellComponent>().SpellWithPoint(skill,new Vector3(message.X,message.Y,message.Z));
                }
                else if (skill.SkillConfig.PreviewType == SkillPreviewType.SelectTarget)
                {
                    var aim = uc.Get(message.Reciver);
                    if (aim != null)
                    {
                        combatU.GetComponent<SpellComponent>().SpellWithTarget(skill,aim.GetComponent<CombatUnitComponent>());
                    }
                    else
                    {
                        combatU.GetComponent<SpellComponent>().SpellWithTarget(skill,null);
                        return;
                    }
                }
                else if (skill.SkillConfig.PreviewType == SkillPreviewType.SelectCircularInCircularArea)
                {
                    combatU.GetComponent<SpellComponent>().SpellWithDirect(skill,new Vector3(message.X,message.Y,message.Z));
                }
                else
                {
                    combatU.GetComponent<SpellComponent>().SpellWithAuto(skill);
                }
            }
            else
            {
                Log.Error("技能不存在");
                return;
            }
        }
    }
}