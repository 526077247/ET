using System;
using System.Collections.Generic;
using UnityEngine;
namespace ET
{

    [ObjectSystem]
    [FriendClass(typeof(KeyCodeComponent))]
    public class SpellPreviewComponentAwakeSystem1: AwakeSystem<SpellPreviewComponent>
    {
        public override void Awake(SpellPreviewComponent self)
        {
            var combatU = self.GetParent<CombatUnitComponent>();
            Log.Info("技能未绑定按键，使用默认按键配置");
            int i = 0;
            foreach (var item in combatU.IdSkills)
            {
                if (i < KeyCodeComponent.Instance.Skills.Length)
                {
                    var keyCode = (int) KeyCodeComponent.Instance.Skills[i];
                    self.BindSkillKeyCode(keyCode, item.Value);
                }
                else
                {
                    break;
                }

                i++;
            }
        }
    }

    [ObjectSystem]
    [FriendClass(typeof(KeyCodeComponent))]
    public class SpellPreviewComponentAwakeSystem : AwakeSystem<SpellPreviewComponent,Dictionary<int,int>>
    {
        public override void Awake(SpellPreviewComponent self,Dictionary<int,int> info)
        {
            var combatU = self.GetParent<CombatUnitComponent>();
            if (info != null)
            {
                for (int i = 0; i < KeyCodeComponent.Instance.Skills.Length; i++)
                {
                    var keyCode = KeyCodeComponent.Instance.Skills[i];
                    if (info.ContainsKey(keyCode) && combatU.IdSkills.ContainsKey(info[keyCode]))
                    {
                        self.BindSkillKeyCode(keyCode, combatU.IdSkills[info[keyCode]]);
                    }
                }
            }
            else
            {
                Log.Info("技能未绑定按键，使用默认按键配置");
                int i = 0;
                foreach (var item in combatU.IdSkills)
                {
                    if (i < KeyCodeComponent.Instance.Skills.Length)
                    {
                        var keyCode = KeyCodeComponent.Instance.Skills[i];
                        self.BindSkillKeyCode(keyCode, item.Value);
                    }
                    else
                    {
                        break;
                    }

                    i++;
                }
            }
        }
    }
    [FriendClass(typeof(SpellPreviewComponent))]
    public static class SpellPreviewComponentSystem
    {
        /// <summary>
        /// 绑定技能与按键
        /// </summary>
        /// <param name="self"></param>
        /// <param name="keyCode"></param>
        /// <param name="ability"></param>
        public static void BindSkillKeyCode(this SpellPreviewComponent self, int keyCode, SkillAbility ability)
        {
            self.InputSkills[keyCode]=ability;
        }
        /// <summary>
        /// 进入预览
        /// </summary>
        /// <param name="self"></param>
        public static void EnterPreview(this SpellPreviewComponent self)
        {
            self.CancelPreview();
            self.Previewing = true;
            //伤害作用对象(0自身1己方2敌方)
            var affectTargetType = self.PreviewingSkill.SkillConfig.DamageTarget;
            //技能预览类型(0大圈选一个目标，1大圈选小圈)
            var previewType = self.PreviewingSkill.SkillConfig.PreviewType;
            // Log.Info("affectTargetType"+affectTargetType+" targetSelectType"+targetSelectType+" previewType"+previewType);
            
            //0大圈选一个目标
            if (previewType == SkillPreviewType.SelectTarget)
            {
                var comp = self.GetComponent<TargetSelectComponent>();
                if (comp==null)
                {
                    comp = self.AddComponent<TargetSelectComponent>();
                }
                comp.TargetLimitType = affectTargetType;
                SelectEventSystem.Instance.Show<Action<Unit>,int[]>(comp,(a)=> { self.OnSelectedTarget(a); },self.PreviewingSkill.SkillConfig.PreviewRange).Coroutine();
                self.CurSelect = comp;
            }
            //1大圈选小圈
            else if (previewType == SkillPreviewType.SelectCircularInCircularArea)
            {
                var comp = self.GetComponent<PointSelectComponent>();
                if (comp==null)
                {
                    comp = self.AddComponent<PointSelectComponent>();
                }
                SelectEventSystem.Instance.Show<Action<Vector3>,int[]>(comp,(a)=> { self.OnInputPoint(a); },self.PreviewingSkill.SkillConfig.PreviewRange).Coroutine();
                self.CurSelect = comp;
            }
            //2矩形
            else if (previewType == SkillPreviewType.SelectRectangleArea)
            {
                var comp = self.GetComponent<DirectRectSelectComponent>();
                if (comp==null)
                {
                    comp = self.AddComponent<DirectRectSelectComponent>();
                }
                SelectEventSystem.Instance.Show<Action<Vector3>,int[]>(comp,(a)=> { self.OnInputDirect(a); },self.PreviewingSkill.SkillConfig.PreviewRange).Coroutine();
                self.CurSelect = comp;
            }
            //自动
            else
            {
                self.SpellWithAuto();
            }
            
            
        }

        public static void CancelPreview(this SpellPreviewComponent self)
        {
            self.Previewing = false;
            if(self.CurSelect!=null)
                SelectEventSystem.Instance.Hide(self.CurSelect);
        }

        private static void SpellWithAuto(this SpellPreviewComponent self)
        {
#if SERVER //单机去掉
            self.SpellComp.SpellWithAuto(self.PreviewingSkill);
#else
            self.PreviewingSkill.UseSkill(Vector3.zero);
#endif
        }
        private static void OnSelectedTarget(this SpellPreviewComponent self,Unit unit)
        {
#if SERVER //单机去掉
            self.SpellComp.SpellWithTarget(self.PreviewingSkill, unit?.GetComponent<CombatUnitComponent>());
#else
            self.PreviewingSkill.UseSkill(Vector3.zero,unit.Id);
#endif
        }   

        private static void OnInputPoint(this SpellPreviewComponent self,Vector3 point)
        {
#if SERVER //单机去掉
            self.SpellComp.SpellWithPoint(self.PreviewingSkill, point);
#else
            self.PreviewingSkill.UseSkill(point);
#endif
        }

        private static void OnInputDirect(this SpellPreviewComponent self, Vector3 point)
        {
#if SERVER //单机去掉
            self.SpellComp.SpellWithDirect(self.PreviewingSkill, point);
#else
            self.PreviewingSkill.UseSkill(point);
#endif
        }

        public static void SelectTargetsWithDistance(this SpellPreviewComponent self,Vector3 point)
        {
            
        }
        
    }
}