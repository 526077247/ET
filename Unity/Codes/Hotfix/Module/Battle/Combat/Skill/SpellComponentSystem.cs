using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
namespace ET
{
    [Timer(TimerType.PlayNextSkillStep)]
    [FriendClass(typeof(SpellComponent))]
    public class PlayNextSkillStep: ATimer<SpellComponent>
    {
        public override void Run(SpellComponent self)
        {
            try
            {
                self.PlayNextSkillStep(self.CurrentSkillStep);
            }
            catch (Exception e)
            {
                Log.Error($"move timer error: {self.Id}\n{e}");
            }
        }
    }
    [ObjectSystem]
    public class SpellComponentAwakeSystem : AwakeSystem<SpellComponent>
    {
        public override void Awake(SpellComponent self)
        {
            self.Skill = null;
        }
    }
    [ObjectSystem]
    public class SpellComponentDestroySystem : DestroySystem<SpellComponent>
    {
        public override void Destroy(SpellComponent self)
        {
            self.Skill = null;
        }
    }
    [FriendClass(typeof(SpellComponent))]
    [FriendClass(typeof(SkillAbility))]
    [FriendClass(typeof(CombatUnitComponent))]
    public static class SpellComponentSystem
    {
        /// <summary>
        /// 释放对目标技能
        /// </summary>
        /// <param name="self"></param>
        /// <param name="spellSkill"></param>
        /// <param name="targetEntity"></param>
        public static void SpellWithTarget(this SpellComponent self, SkillAbility spellSkill, CombatUnitComponent targetEntity)
        {
            if (self.Skill != null)
                return;
            if(!spellSkill.CanUse())return;
            self.Skill = spellSkill;
            var nowpos = self.GetParent<CombatUnitComponent>().unit.Position;
            var nowpos2 = targetEntity.unit.Position;
            if (Vector2.Distance(new Vector2(nowpos.x, nowpos.z), new Vector2(nowpos2.x, nowpos2.z)) >
                spellSkill.SkillConfig.PreviewRange[0])
            {
                return;
            }
            self.Para.Clear();
            self.Para.From = self.GetParent<CombatUnitComponent>();
            self.Para.Ability = spellSkill;
            self.Para.To = targetEntity;

            self.Skill.LastSpellTime = TimeHelper.ClientNow();
            self.PlayNextSkillStep(0);
        }
        /// <summary>
        /// 释放对点技能
        /// </summary>
        /// <param name="self"></param>
        /// <param name="spellSkill"></param>
        /// <param name="point"></param>
        public static void SpellWithPoint(this SpellComponent self,SkillAbility spellSkill, Vector3 point)
        {
            if (self.Skill != null)
                return;
            if(!spellSkill.CanUse())return;
            self.Skill = spellSkill;
            var nowpos = self.GetParent<CombatUnitComponent>().unit.Position;
            if (Vector2.Distance(new Vector2(nowpos.x, nowpos.z), new Vector2(point.x, point.z)) >
                spellSkill.SkillConfig.PreviewRange[0])
            {
                var dir =new Vector3(point.x - nowpos.x,0, point.z - nowpos.z).normalized;
                point = nowpos + dir * spellSkill.SkillConfig.PreviewRange[0];
            }
            self.Para.Clear();
            self.Para.Position = point;
            self.Para.From = self.GetParent<CombatUnitComponent>();
            self.Para.Ability = spellSkill;

            self.Skill.LastSpellTime = TimeHelper.ClientNow();
            self.PlayNextSkillStep(0);
        }
        /// <summary>
        /// 释放方向技能
        /// </summary>
        /// <param name="self"></param>
        /// <param name="spellSkill"></param>
        /// <param name="point"></param>
        public static void SpellWithDirect(this SpellComponent self,SkillAbility spellSkill, Vector3 point)
        {
            if (self.Skill != null)
                return;
            if(!spellSkill.CanUse())return;
            self.Skill = spellSkill;
            var nowpos = self.GetParent<CombatUnitComponent>().unit.Position;
            point = new Vector3(point.x, nowpos.y, point.z);
            var Rotation = Quaternion.LookRotation(point - nowpos,Vector3.up);
            
            self.Para.Clear();
            self.Para.Position = point;
            self.Para.Rotation = Rotation;
            self.Para.From = self.GetParent<CombatUnitComponent>();
            self.Para.Ability = spellSkill;

            self.Skill.LastSpellTime = TimeHelper.ClientNow();
            self.PlayNextSkillStep(0);
        }
        /// <summary>
        /// 播放下一个技能动画
        /// </summary>
        /// <param name="self"></param>
        /// <param name="index"></param>
        public static void PlayNextSkillStep(this SpellComponent self,int index)
        {
            do
            {
                if (self.Skill==null||self.Skill.StepType==null||index >=self.Skill.StepType.Count)
                {
                    if(self.Skill!=null) self.Skill.LastSpellOverTime = TimeHelper.ClientNow();
                    self.Skill = null;
                    return;
                }

                var id = self.Skill.StepType[index];
                self.Para.SetParaStep(index);
                SkillWatcherComponent.Instance.Run(id, self.Para);
                index++;
            } 
            while (self.Para.Interval<=0);
            self.CurrentSkillStep = index;
            TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + self.Para.Interval, TimerType.PlayNextSkillStep, self);
        }

        static void SetParaStep(this SkillPara para,int index)
        {
            if(para.Ability==null) return;
            para.Index = index;
            para.Paras = null;
            para.Interval = 0;
            if (para.Ability.Paras != null && index < para.Ability.Paras.Count)
            {
                para.Paras = para.Ability.Paras[index];
            }
            if (para.Ability.TimeLine != null && index < para.Ability.TimeLine.Count)
            {
                para.Interval = para.Ability.TimeLine[index];
            }
        }
    }
}