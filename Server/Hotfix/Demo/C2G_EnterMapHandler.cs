using System;


namespace ET
{
	[FriendClass(typeof(GateMapComponent))]
	[MessageHandler]
	public class C2G_EnterMapHandler : AMRpcHandler<C2G_EnterMap, G2C_EnterMap>
	{
		protected override async ETTask Run(Session session, C2G_EnterMap request, G2C_EnterMap response, Action reply)
		{
			Player player = session.GetComponent<SessionPlayerComponent>().GetMyPlayer();

			// 在Gate上动态创建一个Map Scene，把Unit从DB中加载放进来，然后传送到真正的Map中，这样登陆跟传送的逻辑就完全一样了
			GateMapComponent gateMapComponent = player.AddComponent<GateMapComponent>();
			gateMapComponent.Scene = await SceneFactory.Create(gateMapComponent, "GateMap", SceneType.Map);

			Scene scene = gateMapComponent.Scene;
			
			// 这里可以从DB中加载Unit
			Unit unit = UnitFactory.Create(scene, player.Id, UnitType.Player);
			unit.AddComponent<UnitGateComponent, long>(session.InstanceId);
			string toMapArea = "Map1AreaConfigCategory";//玩在上次所在区域，需要存db或者有个坐标转区域的方法，新号需要读出生地配置
			var cellId = AOIHelper.CreateCellId(unit.Position, Define.CellLen);
			var area = AreaConfigComponent.Instance.Get(toMapArea).Get(cellId);
			StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.Get(area.SceneId);
			MapSceneConfig mapSceneConfig = MapSceneConfigCategory.Instance.Get(startSceneConfig.Id);
			response.MyId = player.Id;
			reply();
			
			// 开始传送
			await TransferHelper.Transfer(unit, startSceneConfig.InstanceId, mapSceneConfig.Name);
		}
	}
}