using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
	[ActorMessageHandler]
	public class M2M_UnitTransferRequestHandler : AMActorRpcHandler<Scene, M2M_UnitTransferRequest, M2M_UnitTransferResponse>
	{
		protected override async ETTask Run(Scene scene, M2M_UnitTransferRequest request, M2M_UnitTransferResponse response, Action reply)
		{
			await ETTask.CompletedTask;
			UnitComponent unitComponent = scene.GetComponent<UnitComponent>();
			Unit unit = request.Unit;
			
			unitComponent.AddChild(unit);
			unitComponent.Add(unit);

			foreach (Entity entity in request.Entitys)
			{
				unit.AddComponent(entity);
			}
			
			unit.AddComponent<MoveComponent>();
			unit.AddComponent<PathfindingComponent, string>(scene.Name);
			unit.Position = new Vector3(-10, 0, -10);
			
			unit.AddComponent<MailBoxComponent>();
			
			// 通知客户端创建My Unit
			M2C_CreateMyUnit m2CCreateUnits = new M2C_CreateMyUnit();
			m2CCreateUnits.Unit = UnitHelper.CreateUnitInfo(unit);
			m2CCreateUnits.Unit.SkillIds = new List<int>(){1001,1002,1003,1004};//初始技能
			MessageHelper.SendToClient(unit, m2CCreateUnits);
			
			var numericComponent = unit.GetComponent<NumericComponent>();
			// 加入aoi
			var aoiu = unit.AddComponent<AOIUnitComponent,Vector3,Quaternion, CampType,int>
					(unit.Position,unit.Rotation,CampType.Player,numericComponent.GetAsInt(NumericType.AOI));
			unit.AddComponent<CombatUnitComponent,Unit,List<int>>(unit,m2CCreateUnits.Unit.SkillIds);
			aoiu.AddSphereTrigger(0.5f, AOITriggerType.None, null, true);
			response.NewInstanceId = unit.InstanceId;
			
			reply();
		}
	}
}