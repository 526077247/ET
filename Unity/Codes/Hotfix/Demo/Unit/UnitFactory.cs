using UnityEngine;
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
	        var pos = new Vector3(unitInfo.X, unitInfo.Y, unitInfo.Z);
	        
	        unit.Forward = new Vector3(unitInfo.ForwardX, unitInfo.ForwardY, unitInfo.ForwardZ);
	        switch (unit.Type)
	        {
		        case UnitType.Monster:
		        case UnitType.Player:
		        {
			        NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
			        for (int i = 0; i < unitInfo.Ks.Count; ++i)
			        {
				        if(unitInfo.Ks[i]>NumericType.Max)//不需要同步最终值
							numericComponent.Set(unitInfo.Ks[i], unitInfo.Vs[i],true);
			        }

			        unit.AddComponent<MoveComponent>();
			        if (unitInfo.MoveInfo != null)
			        {
				        if (unitInfo.MoveInfo.X.Count > 0)
				        {
					        using (ListComponent<Vector3> list = ListComponent<Vector3>.Create())
					        {
						        list.Add(pos);
						        for (int i = 0; i < unitInfo.MoveInfo.X.Count; ++i)
						        {
							        list.Add(new Vector3(unitInfo.MoveInfo.X[i], unitInfo.MoveInfo.Y[i], unitInfo.MoveInfo.Z[i]));
						        }

						        unit.MoveToAsync(list).Coroutine();
					        }
				        }
			        }
			        unit.AddComponent<AOIUnitComponent,Vector3,Quaternion, UnitType>(pos,unit.Rotation,unit.Type);
			        CombatUnitComponent combatU;
			        if (unitInfo.SkillIds != null)
			        {
				        combatU = unit.AddComponent<CombatUnitComponent,List<int>>(unitInfo.SkillIds);
				        
			        }
			        else
			        {
				        combatU = unit.AddComponent<CombatUnitComponent>();
			        }

			        if (unitInfo.BuffIds != null&&unitInfo.BuffIds.Count>0)
			        {
				        var buffC = combatU.GetComponent<BuffComponent>();
				        buffC.Init(unitInfo.BuffIds, unitInfo.BuffTimestamp);

			        }
			       
			        unit.AddComponent<ObjectWait>();

			        unit.AddComponent<XunLuoPathComponent>();
			        break;
		        }
		        case UnitType.Skill:
		        {
			        NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
			        if (unitInfo.Ks != null && unitInfo.Ks.Count > 0)
			        {
				        for (int i = 0; i < unitInfo.Ks.Count; ++i)
				        {
					        if (unitInfo.Ks[i] > NumericType.Max) //不需要同步最终值
						        numericComponent.Set(unitInfo.Ks[i], unitInfo.Vs[i], true);
				        }
			        }
			        unit.AddComponent<MoveComponent>();
			        if (unitInfo.MoveInfo != null&&unitInfo.MoveInfo.X.Count > 0)
			        {
				        using (ListComponent<Vector3> list = ListComponent<Vector3>.Create())
				        {
					        list.Add(pos);
					        for (int i = 0; i < unitInfo.MoveInfo.X.Count; ++i)
					        {
						        list.Add(new Vector3(unitInfo.MoveInfo.X[i], unitInfo.MoveInfo.Y[i], unitInfo.MoveInfo.Z[i]));
					        }

					        unit.MoveToAsync(list).Coroutine();
				        }
			        }
			        unit.AddComponent<AOIUnitComponent,Vector3,Quaternion, UnitType>(pos,unit.Rotation,unit.Type);
			        unit.AddComponent<ObjectWait>();
			        break;
		        }
		        default:
		        {
			        Log.Error("没有处理 "+unit.Type);
			        break;
		        }
	        }

	        unit.Position = pos;
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
        /// <param name="para"></param>
        /// <returns></returns>
        public static Unit CreateSkillCollider(Scene currentScene,int configId, Vector3 pos,Quaternion rota,SkillPara para)
        {
	        UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
	        Unit unit = unitComponent.AddChild<Unit,int>(configId);
        
	        unit.Position = pos;
	        unit.Rotation = rota;
	        unit.AddComponent<SkillColliderComponent, SkillPara>(para);
	        unit.AddComponent<AOIUnitComponent,Vector3,Quaternion, UnitType>(pos,rota,UnitType.Skill);
	        return unit;
        }

    }
}
