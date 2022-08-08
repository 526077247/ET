using System.Collections.Generic;
using UnityEngine;

namespace ET
{
	[ActorMessageHandler]
	public class C2M_StopHandler : AMActorLocationHandler<Unit, C2M_Stop>
	{
		protected override async ETTask Run(Unit unit, C2M_Stop message)
		{
			unit.Stop(0);
			var ghost = unit.GetComponent<AOIUnitComponent>().GetComponent<GhostComponent>();

			await ETTask.CompletedTask;
		}
	}
}