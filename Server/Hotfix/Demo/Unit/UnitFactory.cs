using System;
using UnityEngine;
using System.Collections.Generic;
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
                    numericComponent.Set(NumericType.Speed, 6f); // 速度是6米每秒
                    numericComponent.Set(NumericType.AOI, 2); // 视野2格
                    numericComponent.Set(NumericType.Hp, 1000); // 生命1000
                    numericComponent.Set(NumericType.MaxHp, 1000); // 最大生命1000
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
        
        public static Unit CreateSkillCollider(Scene currentScene, int configId,Vector3 pos,Quaternion rota,CombatUnitComponent from)
        {
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            Unit unit = unitComponent.AddChild<Unit,int>(configId);
        
            unit.Position = pos;
            unit.Rotation = rota;
            unit.AddComponent<SkillColliderComponent, int,CombatUnitComponent>(configId,from);
            unit.AddComponent<AOIUnitComponent,Vector3,Quaternion, UnitType>(pos,rota,unit.Type);
            return unit;
        }
    }
}