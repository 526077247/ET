﻿using UnityEngine;
using System.Collections.Generic;

namespace ET
{
    public static class UnitFactory
    {
        public static Unit Create(Scene currentScene, UnitInfo unitInfo)
        {
	        UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
	        Unit unit = unitComponent.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId);
	        unitComponent.Add(unit);
	        
	        unit.Position = new Vector3(unitInfo.X, unitInfo.Y, unitInfo.Z);
	        unit.Forward = new Vector3(unitInfo.ForwardX, unitInfo.ForwardY, unitInfo.ForwardZ);
	        switch (unit.Type)
	        {
		        case UnitType.Player:
		        {
			        NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
			        for (int i = 0; i < unitInfo.Ks.Count; ++i)
			        {
				        numericComponent.Set(unitInfo.Ks[i], unitInfo.Vs[i]);
			        }

			        unit.AddComponent<MoveComponent>();
			        if (unitInfo.MoveInfo != null)
			        {
				        if (unitInfo.MoveInfo.X.Count > 0)
				        {
					        using (ListComponent<Vector3> list = ListComponent<Vector3>.Create())
					        {
						        list.Add(unit.Position);
						        for (int i = 0; i < unitInfo.MoveInfo.X.Count; ++i)
						        {
							        list.Add(new Vector3(unitInfo.MoveInfo.X[i], unitInfo.MoveInfo.Y[i], unitInfo.MoveInfo.Z[i]));
						        }

						        unit.MoveToAsync(list).Coroutine();
					        }
				        }
			        }
			        
			        if (unitInfo.SkillIds != null)
			        {
				        Log.Info("-----------------"+unit.Id);
				        unit.AddComponent<CombatUnitComponent,Unit,List<int>>(unit,unitInfo.SkillIds);
			        }
			        unit.AddComponent<ObjectWait>();

			        unit.AddComponent<XunLuoPathComponent>();
			        break;
		        }
		        case UnitType.Skill:
		        {
			        unit.AddComponent<MoveComponent>();
			        if (unitInfo.MoveInfo != null)
			        {
				        if (unitInfo.MoveInfo.X.Count > 0)
				        {
					        using (ListComponent<Vector3> list = ListComponent<Vector3>.Create())
					        {
						        list.Add(unit.Position);
						        for (int i = 0; i < unitInfo.MoveInfo.X.Count; ++i)
						        {
							        list.Add(new Vector3(unitInfo.MoveInfo.X[i], unitInfo.MoveInfo.Y[i], unitInfo.MoveInfo.Z[i]));
						        }

						        unit.MoveToAsync(list).Coroutine();
					        }
				        }
			        }

			        unit.AddComponent<ObjectWait>();
			        break;
		        }
			        
	        }
	        Game.EventSystem.PublishAsync(new EventType.AfterUnitCreate() {Unit = unit}).Coroutine();
            return unit;
        }

        /// <summary>
        /// 创建技能触发体（单机用）
        /// </summary>
        /// <param name="currentScene"></param>
        /// <param name="configId"></param>
        /// <param name="pos"></param>
        /// <param name="rota"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public static Unit CreateSkillCollider(Scene currentScene, int configId,Vector3 pos,Quaternion rota,CombatUnitComponent from)
        {
	        UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
	        Unit unit = unitComponent.AddChild<Unit,int>(configId);
        
	        unit.Position = pos;
	        unit.Rotation = rota;
	        unit.AddComponent<SkillColliderComponent, int,CombatUnitComponent>(configId,from);
	        unit.AddComponent<AOIUnitComponent,Vector3,Quaternion, CampType>(pos,rota,CampType.SkillCollider);
	        return unit;
        }

    }
}
