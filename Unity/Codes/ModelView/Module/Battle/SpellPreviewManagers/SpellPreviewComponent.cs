using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{

    /// <summary>
    /// 主动技能的预览组件，预览功能不是所有游戏都会有，moba类游戏一般会有技能预览的功能，部分mmorpg游戏也可能有，回合制、卡牌、arpg动作游戏一般没有
    /// </summary>
    public class SpellPreviewComponent : Entity,IAwake<Dictionary<int,int>>,IAwake
    {
#if SERVER //单机去掉
        public SpellComponent SpellComp => parent.GetComponent<SpellComponent>();
#endif
        public bool Previewing;
        public SkillAbility PreviewingSkill { get; set; }

        public Entity CurSelect;
        public Dictionary<int, SkillAbility> InputSkills { get;  } = new Dictionary<int, SkillAbility>();
    }
}
