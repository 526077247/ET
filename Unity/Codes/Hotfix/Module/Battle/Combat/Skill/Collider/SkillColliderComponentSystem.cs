using System;
using UnityEngine;
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
    [Timer(TimerType.GenerateSkillCollider)]
    public class GenerateSkillCollider: ATimer<SkillColliderComponent>
    {
        public override void Run(SkillColliderComponent self)
        {
            try
            {
                self.GenerateSkillCollider();
            }
            catch (Exception e)
            {
                Log.Error($"move timer error: {self.Id}\n{e}");
            }
        }
    }
    
    public class SkillColliderAwakeSystem : AwakeSystem<SkillColliderComponent, SkillPara>
    {
        public override void Awake(SkillColliderComponent self, SkillPara para)
        {
            int curIndex = para.CurIndex;
            var stepPara = para.StepPara[curIndex];
            self.Cost = para.Cost;
            self.CostId = para.CostId;
            self.SkillConfigId = para.Ability.SkillConfig.Id;
            self.CreateTime = TimeHelper.ServerNow();
            self.FromId = para.From.Id;
            self.Para = stepPara;
            if (int.TryParse(stepPara.Paras[0].ToString(), out var colliderId))
            {
                self.ConfigId = colliderId;
                
                #region 添加触发器

                int deltaTime = 0;
                if (stepPara.Paras.Length >= 6)
                {
                    int.TryParse(stepPara.Paras[5].ToString(), out deltaTime);
                }
                if (deltaTime <= 0)
                {
                    deltaTime = 1;//等下一帧
                }
                if (self.Config.ColliderShape == SkillColliderShapeType.None)
                {
                    return;
                }
                else if (self.Config.ColliderShape == SkillColliderShapeType.Sphere||
                         self.Config.ColliderShape == SkillColliderShapeType.OBB)
                {
                    
                    TimerComponent.Instance.NewOnceTimer(self.CreateTime + deltaTime,
                        TimerType.GenerateSkillCollider, self);
                }
                else
                {
                    Log.Error("碰撞体形状未处理" + self.Config.ColliderType);
                    return;
                }

                #endregion

                TimerComponent.Instance.NewOnceTimer(self.CreateTime + self.Config.Time,
                    TimerType.SkillColliderRemove, self.Unit);
            }
        }
    }
    [FriendClass(typeof(SkillColliderComponent))]
    public static class SkillColliderComponentSystem
    {
        public static void GenerateSkillCollider(this SkillColliderComponent self)
        {
            var aoiUnit = self.Unit.Parent.GetChild<Unit>(self.FromId).GetComponent<AOIUnitComponent>();
            var skillAOIUnit = self.Unit.GetComponent<AOIUnitComponent>();
            if (skillAOIUnit == null||skillAOIUnit.IsDisposed)
            {
                return;
            }
            if (self.Config.ColliderShape == SkillColliderShapeType.OBB)
            {
                Vector3 par = new Vector3(self.Config.ColliderPara[0], self.Config.ColliderPara[1],
                    self.Config.ColliderPara[2]);
                skillAOIUnit.AddOBBTrigger(par, AOITriggerType.All,
                    (o, e) =>
                    {
                        EventSystem.Instance.Publish(new EventType.OnSkillTrigger
                        {
                            From = aoiUnit,
                            To = o,
                            Para = self.Para,
                            Config = self.SkillConfig,
                            Cost = self.Cost,
                            CostId = self.CostId,
                            Type = e
                        });
                    }, false, UnitType.ALL);//测试为所有
            }
            else if (self.Config.ColliderShape == SkillColliderShapeType.Sphere)
            {
                skillAOIUnit.AddSphereTrigger(self.Config.ColliderPara[0], AOITriggerType.All,
                    (o, e) =>
                    {
                        EventSystem.Instance.Publish(new EventType.OnSkillTrigger
                        {
                            From = aoiUnit,
                            To = o,
                            Para = self.Para,
                            Config = self.SkillConfig,
                            Cost = self.Cost,
                            CostId = self.CostId,
                            Type = e
                        });
                    }, false, UnitType.ALL);//测试为所有
            }
            else
            {
                Log.Error("碰撞体形状未处理" + self.Config.ColliderType);
                return;
            }
        }
    }
}