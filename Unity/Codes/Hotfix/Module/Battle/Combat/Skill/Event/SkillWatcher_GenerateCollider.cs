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
    [FriendClass(typeof(SkillAbility))]
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
                        unit = UnitFactory.CreateSkillCollider(scene, colliderId, startPos,para.Rotation,para.From,collider.Speed);
                        unit.AddComponent<MoveComponent>();
                        Log.Info(startPos+" "+startPos+(para.Position-startPos).normalized*collider.Speed*collider.Time/1000f);
                        List<Vector3> target = new List<Vector3>();
                        target.Add(startPos);
                        target.Add(startPos+(para.Position-startPos).normalized*collider.Speed*collider.Time/1000f);
                        unit.MoveToAsync(target).Coroutine();
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
                            OnColliderTrigger(aoiUnit, aoiUnit,AOITriggerType.Enter,para, collider);
                        else if(collider.StartPosType == ColliderStartPosType.Aim&&para.To!=null)
                            OnColliderTrigger(aoiUnit, para.To.unit.GetComponent<AOIUnitComponent>()
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
                        skillAOIUnit.AddSphereTrigger(collider.ColliderPara[0], AOITriggerType.All, (o,e) =>
                        {
                            OnColliderTrigger(aoiUnit, o,e,para, collider);
                        },false,UnitType.ALL);//测试判断为所有人
                    }
                    else if (collider.ColliderShape == SkillColliderShapeType.OBB)
                    {
                        var skillAOIUnit = unit.GetComponent<AOIUnitComponent>();
                        Vector3 par = new Vector3(collider.ColliderPara[0], collider.ColliderPara[1],
                            collider.ColliderPara[2]);
                        skillAOIUnit.AddOBBTrigger(par, AOITriggerType.All, (o,e) =>
                        {
                            OnColliderTrigger(aoiUnit, o,e,para,collider);
                        },false,UnitType.ALL);//测试判断为所有人
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
        /// 当触发
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="type"></param>
        /// <param name="para"></param>
        /// <param name="judge"></param>
        public void OnColliderTrigger(AOIUnitComponent from, AOIUnitComponent to,
            AOITriggerType type, SkillPara para, SkillJudgeConfig judge)
        {
            if (type == AOITriggerType.Enter)
            {
                OnColliderIn(from, to, para, judge);
            }
            else if (type == AOITriggerType.Exit)
            {
                OnColliderOut(from, to, para, judge);
            }
        }
        /// <summary>
        /// 进入触发器
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="type"></param>
        /// <param name="para"></param>
        /// <param name="judge"></param>
        public void OnColliderIn(AOIUnitComponent from, AOIUnitComponent to, SkillPara para, SkillJudgeConfig judge)
        {
            // Log.Info("触发"+type.ToString()+to.Id+"  "+from.Id);
            // Log.Info("触发"+type.ToString()+to.Position+" Dis: "+Vector3.Distance(to.Position,from.Position));
            int formulaId = 0;//公式
            if (para.Paras.Length > 1)
            {
                int.TryParse(para.Paras[1].ToString(), out formulaId);
            }
            float percent = 1;//实际伤害百分比
            if (para.Paras.Length > 2)
            {
                float.TryParse(para.Paras[2].ToString(), out percent);
            }

            List<int[]> buffInfo = null;//添加的buff
            if (para.Paras.Length > 3)
            {
                buffInfo = para.Paras[3] as List<int[]>;
                if (buffInfo == null)
                {
                    string[] vs = para.Paras[3].ToString().Split(';');
                    buffInfo = new List<int[]>();
                    for (int i = 0; i < vs.Length; i++)
                    {
                        var data = vs[i].Split(',');
                        int[] temp = new int[data.Length];
                        for (int j = 0; j < data.Length; j++)
                        {
                            temp[j] = int.Parse(data[i]);
                        }
                        buffInfo.Add(temp);
                    }
                    para.Paras[3] = buffInfo;
                }
            }
            var combatU = to.Parent.GetComponent<CombatUnitComponent>();
            if(buffInfo!=null&&buffInfo.Count>0)
            {
                var buffC = combatU.GetComponent<BuffComponent>();
                for (int i = 0; i < buffInfo.Count; i++)
                {
                    buffC.AddBuff(buffInfo[i][0],TimeHelper.ServerNow() + buffInfo[i][1]);
                }
            }

            FormulaConfig formula = FormulaConfigCategory.Instance.Get(formulaId);
            if (formula!=null)
            {
                for (int i = 0; i < (para.Cost!=null?para.Cost.Count:0); i++)
                {
                    formula.Formula = formula.Formula.Replace("SkillCost"+i, para.Cost[i].ToString());
                }
                FormulaStringFx fx = FormulaStringFx.GetInstance(formula.Formula);
                NumericComponent f = from.GetParent<Unit>().GetComponent<NumericComponent>();
                NumericComponent t = to?.GetParent<Unit>().GetComponent<NumericComponent>();
                float value = fx.GetData(f, t)*percent;
                int realValue = (int)value;
                
                int now = t.GetAsInt(NumericType.HpBase);
                if (now < realValue)
                {
                    realValue = now;
                    t.Set(NumericType.HpBase,0);
                }
                else
                {
                    t.Set(NumericType.HpBase,now - realValue);
                }
                EventSystem.Instance.Publish(new EventType.AfterCombatUnitGetDamage()
                {
                    Unit = combatU,
                    From = from.Parent.GetComponent<CombatUnitComponent>(),
                    Value = realValue,
                    SkillId = para.Ability.ConfigId
                });
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
        public void OnColliderOut(AOIUnitComponent from, AOIUnitComponent to, SkillPara para, SkillJudgeConfig judge)
        {
            // Log.Info("触发"+type.ToString()+to.Id+"  "+from.Id);
            // Log.Info("触发"+type.ToString()+to.Position+" Dis: "+Vector3.Distance(to.Position,from.Position));
            if (para.Paras.Length > 3)
            {
                List<int[]> buffInfo = para.Paras[3] as List<int[]>;
                if (buffInfo != null&&buffInfo.Count>0)
                {
                    var buffC = to.Parent.GetComponent<CombatUnitComponent>().GetComponent<BuffComponent>();
                    for (int i = 0; i < buffInfo.Count; i++)
                    {
                        if (buffInfo[i][2] == 1)
                        {
                            buffC.RemoveByConfigId(buffInfo[i][0]);
                        }
                    }
                }
            }
        }
    }
}