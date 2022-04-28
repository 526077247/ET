using System;
using UnityEngine;
using System.Collections.Generic;
namespace ET
{
    [ChildType(typeof(SkillAbility))]
    public class CombatUnitComponent:Entity,IAwake<Unit>,IAwake<Unit,List<int>>,IDestroy
    {
        public Unit unit;
        public Dictionary<int, SkillAbility> IdSkills { get;  } = new Dictionary<int, SkillAbility>();

    }
}