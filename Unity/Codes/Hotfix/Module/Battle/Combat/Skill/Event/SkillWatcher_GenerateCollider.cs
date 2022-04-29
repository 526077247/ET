using UnityEngine;
using System.Collections.Generic;
using System;

namespace ET
{
    [Timer(TimerType.SkillColliderRemove)]
    public class SkillColliderRemove: ATimer<Unit>
    {
        public override void Run(Unit self)
        {
            try
            {
                self.Dispose();
            }
            catch (Exception e)
            {
                Log.Error($"move timer error: {self.Id}\n{e}");
            }
        }
    }
    
    /// <summary>
    /// 生成碰撞体
    /// </summary>
    [SkillWatcher(SkillStepType.GenerateCollider)]
    [FriendClass(typeof(AOIUnitComponent))]
    [FriendClass(typeof(CombatUnitComponent))]
    public class SkillWatcher_GenerateCollider : ISkillWatcher
    {
        public void Run(SkillPara para)
        {
#if SERVER
            Log.Info("SkillWatcher_GenerateCollider");
            if(int.TryParse(para.Paras[0].ToString(),out var colliderId))
            {
                SkillJudgeConfig collider = SkillJudgeConfigCategory.Instance.Get(colliderId);
                if (collider != null)
                {
                    var aoiUnit = para.From.unit.GetComponent<AOIUnitComponent>();
                    var scene = aoiUnit.Scene.GetParent<Scene>();
                    Unit unit = null;
                    Vector3 FromUnitPos = para.From.unit.Position;
                    Vector3 ToUnitPos = Vector3.zero;
                    if(para.To!=null)
                        ToUnitPos = para.To.unit.Position;

                    #region 创建碰撞体AOIUnit
                    
                    if (collider.ColliderType == SkillJudgeType.FixedPosition)//固定位置碰撞体
                    {
                        if(collider.StartPosType == ColliderStartPosType.Self)
                            unit = UnitFactory.CreateSkillCollider(scene, colliderId, FromUnitPos,para.Rotation,para.From);
                        else if (collider.StartPosType == ColliderStartPosType.Aim && para.To != null)
                        {
                            unit = UnitFactory.CreateSkillCollider(scene, colliderId, ToUnitPos,para.Rotation,para.From);
                        }
                        else if(collider.StartPosType == ColliderStartPosType.MousePos)
                            unit = UnitFactory.CreateSkillCollider(scene, colliderId, para.Position,para.Rotation,para.From);
                        else
                        {
                            Log.Info("目标未指定,或触发体类型不存在");
                            return;
                        }
                    }
                    else if (collider.ColliderType == SkillJudgeType.FixedRotation)//固定方向碰撞体
                    {
                        var dir =new Vector3(para.Position.x - FromUnitPos.x,para.Position.y- FromUnitPos.y, para.Position.z - FromUnitPos.z).normalized;
                        if (collider.ColliderShape == SkillColliderShapeType.OBB)//立方找到中点
                        {
                            var point = FromUnitPos + dir * collider.ColliderPara[2] / 2;
                            if (collider.StartPosType == ColliderStartPosType.Self)
                            {
                                unit = UnitFactory.CreateSkillCollider(scene, colliderId, point, para.Rotation,
                                    para.From);
                            }
                            else
                            {
                                Log.Info("目标未指定,或触发体类型不存在");
                                return;
                            }
                        }
                        else
                        {
                            Log.Info("目标未指定,或触发体类型不存在");
                            return;
                        }
                    }
                    else if (collider.ColliderType == SkillJudgeType.Target)//朝指定位置方向飞行碰撞体
                    {
                        Vector3 startPos = FromUnitPos;
                        if (collider.StartPosType == ColliderStartPosType.Self)
                            startPos = FromUnitPos;
                        else if(collider.StartPosType == ColliderStartPosType.Aim&&para.To!=null)
                            startPos = ToUnitPos;
                        else if (collider.StartPosType == ColliderStartPosType.MousePos)
                            startPos = para.Position;
                        else
                        {
                            Log.Info("目标未指定,或触发体类型不存在");
                            return;
                        }
                        unit = UnitFactory.CreateSkillCollider(scene, colliderId, startPos,para.Rotation,para.From);
                        var numc = unit.AddComponent<NumericComponent>();
                        numc.Set(NumericType.SpeedBase,collider.Speed);
                        var moveComp = unit.AddComponent<MoveComponent>();
                        Log.Info(startPos+" "+startPos+(para.Position-startPos).normalized*collider.Speed*collider.Time/1000f);
                        List<Vector3> target = new List<Vector3>();
                        target.Add(startPos);
                        target.Add(startPos+(para.Position-startPos).normalized*collider.Speed*collider.Time/1000f);
                        moveComp.MoveToAsync(target, collider.Speed).Coroutine();
                    }
                    else if (collider.ColliderType == SkillJudgeType.Aim)//锁定目标飞行
                    {
                        Vector3 startPos = FromUnitPos;
                        if (collider.StartPosType == ColliderStartPosType.Self&&para.To!=null)
                            startPos = FromUnitPos;
                        else if(collider.StartPosType == ColliderStartPosType.Aim&&para.To!=null)
                            startPos = ToUnitPos;
                        else if (collider.StartPosType == ColliderStartPosType.MousePos&&para.To!=null)
                            startPos = para.Position;
                        else
                        {
                            Log.Info("目标未指定,或触发体类型不存在");
                            return;
                        }
                        unit = UnitFactory.CreateSkillCollider(scene, colliderId, startPos,para.Rotation,para.From);
                        var numc = unit.AddComponent<NumericComponent>();
                        numc.Set(NumericType.SpeedBase,collider.Speed);
                        unit.AddComponent<MoveComponent>();
                        unit.AddComponent<ZhuiZhuAimComponent, Unit, Action>(para.To.unit, () =>
                        {
                            unit.Dispose();
                        });
                        unit.AddComponent<AIComponent,int,int>(2,50);
                    }
                    else if (collider.ColliderType == SkillJudgeType.Immediate) //立刻结算
                    {
                        if (collider.StartPosType == ColliderStartPosType.Self)
                            OnColliderIn(aoiUnit, aoiUnit,AOITriggerType.Enter,para, collider);
                        else if(collider.StartPosType == ColliderStartPosType.Aim&&para.To!=null)
                            OnColliderIn(aoiUnit, para.To.unit.GetComponent<AOIUnitComponent>()
                                ,AOITriggerType.Enter,para,collider);
                        else if (collider.StartPosType == ColliderStartPosType.MousePos)
                        {
                            Log.Error("立刻结算类型,必须指定目标");
                            return;
                        }
                        else
                        {
                            Log.Info("目标未指定,或触发体类型不存在");
                            return;
                        }
                    }
                    else
                    {
                        Log.Error("碰撞体类型未处理"+collider.ColliderType);
                        return;
                    }
                    #endregion
                    
                    #region 添加触发器

                    if (collider.ColliderShape == SkillColliderShapeType.None)
                    {
                        return;
                    }
                    else if (collider.ColliderShape==SkillColliderShapeType.Sphere)
                    {
                        var skillAOIUnit = unit.GetComponent<AOIUnitComponent>();
                        skillAOIUnit.AddSphereTrigger(collider.ColliderPara[0], AOITriggerType.Enter, (o,e) =>
                        {
                            OnColliderIn(aoiUnit, o,e,para, collider);
                        },false,UnitType.Monster);
                        if (collider.BuffIds != null&&collider.IsExitRemove!=null)
                        {
                            for (int i = 0; i < collider.IsExitRemove.Length; i++)
                            {
                                var item = collider.IsExitRemove[i];
                                if (item == 1)
                                {
                                    skillAOIUnit.AddSphereTrigger(collider.ColliderPara[0], AOITriggerType.Exit, (o,e) =>
                                    {
                                        OnColliderOut(aoiUnit, o,e,para, collider);
                                    },false,UnitType.Monster);
                                    break;
                                }
                            }
                        }
                    }
                    else if (collider.ColliderShape == SkillColliderShapeType.OBB)
                    {
                        var skillAOIUnit = unit.GetComponent<AOIUnitComponent>();
                        Vector3 par = new Vector3(collider.ColliderPara[0], collider.ColliderPara[1],
                            collider.ColliderPara[2]);
                        skillAOIUnit.AddOBBTrigger(par, AOITriggerType.Enter, (o,e) =>
                        {
                            OnColliderIn(aoiUnit, o,e,para,collider);
                        },false,UnitType.Monster);
                        if (collider.BuffIds != null&&collider.IsExitRemove!=null)
                        {
                            for (int i = 0; i < collider.IsExitRemove.Length; i++)
                            {
                                var item = collider.IsExitRemove[i];
                                if (item == 1)
                                {
                                    skillAOIUnit.AddSphereTrigger(collider.ColliderPara[0], AOITriggerType.Exit, (o,e) =>
                                    {
                                        OnColliderOut(aoiUnit, o,e,para, collider);
                                    },false,UnitType.Monster);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        Log.Error("碰撞体形状未处理"+collider.ColliderType);
                        return;
                    }
                    
                    #endregion
                    
                    TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + collider.Time, TimerType.SkillColliderRemove, unit);
                    
                }
                
            }
#endif
        }
        /// <summary>
        /// 进入触发器
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="type"></param>
        /// <param name="para"></param>
        /// <param name="judge"></param>
        public void OnColliderIn(AOIUnitComponent from, AOIUnitComponent to,
            AOITriggerType type,SkillPara para, SkillJudgeConfig judge)
        {
            Log.Info("触发"+type.ToString()+to.Id+"  "+from.Id);
            Log.Info("触发"+type.ToString()+to.Position+" Dis: "+Vector3.Distance(to.Position,from.Position));
            if(judge.BuffIds!=null)
            {
                for (int i = 0; i < (judge.BuffIds==null?0:judge.BuffIds.Length); i++)
                {
                    if(judge.BuffTimes==null||judge.BuffTimes.Length<=i) return;
                    to.GetComponent<CombatUnitComponent>().GetComponent<BuffComponent>().AddBuff(judge.BuffIds[i],judge.BuffTimes[i]);
                }
            }

            FormulaConfig formula = FormulaConfigCategory.Instance.Get(judge.FormulaId);
            if (formula!=null)
            {
                for (int i = 0; i < (para.Cost!=null?para.Cost.Count:0); i++)
                {
                    formula.Formula = formula.Formula.Replace("SkillCost"+i, para.Cost[i].ToString());
                }
                FormulaStringFx fx = FormulaStringFx.GetInstance(formula.Formula);
                NumericComponent f = from.GetParent<Unit>().GetComponent<NumericComponent>();
                NumericComponent t = to?.GetParent<Unit>().GetComponent<NumericComponent>();
                float value = fx.GetData(f, t);
                if (para.Paras.Length>1&&float.TryParse(para.Paras[1].ToString(), out var percent))
                {
                    value *= percent;
                }
                ListComponent<int> realValues = ListComponent<int>.Create();
                int realValue = 0;
                
                float now = t.GetAsFloat(NumericType.HpBase);
                if (realValue > 0) //扣血
                {
                    if (now < realValue)
                    {
                        t.Set(NumericType.HpBase,0);
                    }
                    else
                    {
                        t.Set(NumericType.HpBase,now - realValue);
                    }
                    EventSystem.Instance.Publish(new EventType.AfterCombatUnitGetDamage()
                    {
                        CombatUnitComponent = to.GetComponent<CombatUnitComponent>()
                    });
                }
                else if (realValue < 0)//加血
                {
                    float max = t.GetAsFloat(NumericType.MaxHp);
                    if (now + realValue >= max)
                    {
                        t.Set(NumericType.HpBase,max);
                    }
                    else
                    {
                        t.Set(NumericType.HpBase,now + realValue);
                    }
                }
                realValues.Dispose();
            }
        }
        /// <summary>
        /// 离开触发器
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="type"></param>
        /// <param name="para"></param>
        /// <param name="judge"></param>
        public void OnColliderOut(AOIUnitComponent from, AOIUnitComponent to,
            AOITriggerType type, SkillPara para, SkillJudgeConfig judge)
        {
            Log.Info("触发"+type.ToString()+to.Id+"  "+from.Id);
            Log.Info("触发"+type.ToString()+to.Position+" Dis: "+Vector3.Distance(to.Position,from.Position));
            if(judge.BuffIds!=null)
            {
                for (int i = 0; i < (judge.BuffIds==null?0:judge.BuffIds.Length); i++)
                {
                    if (judge.IsExitRemove.Length > i && judge.IsExitRemove[i] == 1)
                    {
                        to.GetComponent<CombatUnitComponent>().GetComponent<BuffComponent>().RemoveByConfigId(judge.BuffIds[i]);
                    }
                }
            }
        }
    }
}