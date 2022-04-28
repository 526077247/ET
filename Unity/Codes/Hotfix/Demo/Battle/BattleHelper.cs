using System;
using UnityEngine;

namespace ET
{
    [FriendClass(typeof(SkillAbility))]
    public static class BattleHelper
    {
        public static void UseSkill(this SkillAbility skill, Vector3 pos,long id = 0)
        {
            C2M_UseSkill msg = new C2M_UseSkill()
            {
                SkillConfigId = skill.ConfigId,
                X = pos.x,
                Y = pos.y,
                Z = pos.z,
                Id = id
            };
            skill.ZoneScene().GetComponent<SessionComponent>().Session.Send(msg);
        }
    }
}