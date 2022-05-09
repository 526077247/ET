using System;
using UnityEngine;
using System.Collections.Generic;
using OfficeOpenXml.Drawing.Style.Coloring;

namespace ET
{
    public static class UnitFactory
    {
        public static Unit Create(Scene scene, long id, UnitType unitType)
        {
            UnitComponent unitComponent = scene.GetComponent<UnitComponent>();
            switch (unitType)
            {
                case UnitType.Player:
                {
                    Unit unit = unitComponent.AddChildWithId<Unit, int>(id, 1);
                    //ChildType测试代码 取消注释 编译Server.hotfix 可发现报错
                    //unitComponent.AddChild<Player, string>("Player");
                    unit.AddComponent<MoveComponent>();
                    unit.Position = new Vector3(-10, 0, -10);
			
                    NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
                    numericComponent.Set(NumericType.SpeedBase, 6f); // 速度是6米每秒
                    numericComponent.Set(NumericType.AOIBase, 2); // 视野2格
                    numericComponent.Set(NumericType.HpBase, 1000); // 生命1000
                    numericComponent.Set(NumericType.MaxHpBase, 1000); // 最大生命1000
                    numericComponent.Set(NumericType.LvBase,1); //1级
                    numericComponent.Set(NumericType.ATKBase,100); //100攻击
                    numericComponent.Set(NumericType.DEFBase,500); //500防御
                    var SkillIds = new List<int>(){1001,1002,1003,1004};//初始技能
                    unit.AddComponent<CombatUnitComponent,List<int>>(SkillIds);
                    unitComponent.Add(unit);
                    // 进入地图再加入aoi
                    
                    return unit;
                }
                default:
                    throw new Exception($"not such unit type: {unitType}");
            }
        }
        
        public static Unit CreateSkillCollider(Scene currentScene, int configId,Vector3 pos,Quaternion rota,CombatUnitComponent from,float speed = 0)
        {
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            Unit unit = unitComponent.AddChild<Unit,int>(configId);
        
            unit.Position = pos;
            unit.Rotation = rota;
            if (speed > 0)
            {
                var numc = unit.AddComponent<NumericComponent>();
                numc.Set(NumericType.SpeedBase, speed);
            }

            unit.AddComponent<SkillColliderComponent, int,CombatUnitComponent>(configId,from);
            unit.AddComponent<AOIUnitComponent,Vector3,Quaternion, UnitType>(pos,rota,unit.Type);
            return unit;
        }
    }
}