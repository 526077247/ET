using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using ET.EventType;
using UnityEngine;

namespace ET
{
    [ObjectSystem]
    public class AOISceneComponentAwakeSystem : AwakeSystem<AOISceneComponent, int>
    {
        public override void Awake(AOISceneComponent self, int gridLen)
        {
            self.gridLen = gridLen;
            self.halfDiagonal = self.gridLen*0.7072f;
            Log.Info("AOIScene StandBy! ");
        }
    }

    [ObjectSystem]
    public class AOISceneComponentDestroySystem : DestroySystem<AOISceneComponent>
    {
        public override void Destroy(AOISceneComponent self)
        {

        }
    }
    [FriendClass(typeof(AOISceneComponent))]
    [FriendClass(typeof(AOIUnitComponent))]
    [FriendClass(typeof(AOIGrid))]
    public static class AOISceneComponentSystem
    {
        /// <summary>
        /// 找到指定位置所在的Grid
        /// </summary>
        /// <param name="self"></param>
        /// <param name="pos"></param>
        /// <param name="create">没有是否创建</param>
        public static AOIGrid GetAOIGrid(this AOISceneComponent self,Vector3 pos,bool create = true)
        {
            int xIndex = (int)Math.Floor(pos.x / self.gridLen);
            int yIndex = (int)Math.Floor(pos.z / self.gridLen);
            
            return self.GetCell(xIndex,yIndex,create);
        }

        /// <summary>
        /// 注册一个 AOI 对象, 同时设置其默认 AOI 半径。注：每个对象都有一个默认的 AOI 半径，凡第一次进入半径范围的其它物体，都会触发 AOI 消息。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="unit"></param>
        public static void RegisterUnit(this AOISceneComponent self,AOIUnitComponent unit)
        {
            unit.Scene = self;
            AOIGrid grid = self.GetAOIGrid(unit.Position);
            grid.Add(unit);
            Log.Info("RegisterUnit:" + unit.Id + "  Position:" + unit.Position + "  grid x:"+ grid.posx+",y:"+ grid.posy+" type"+unit.Type);

            using (var ListenerGrids = grid.GetNearbyGrid(unit.Range))
            {
                for (int i = 0; i < ListenerGrids.Count; i++)
                {
                    var item = ListenerGrids[i];
                    item.AddListener(unit);
                    if (unit.Type == UnitType.Player)
                    {
                        using (var list = item.GetAllUnit())
                        {
                            for (int j = 0; j < list.Count; j++)
                            {
                                var t = list[j];
                                Game.EventSystem.Publish(new AOIRegisterUnit()
                                {
                                    Receive = unit,
                                    Unit = t
                                });
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 删除一个 AOI 对象 ,同时有可能触发它相对其它 AOI 对象的离开消息。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="unit"></param>
        public static void RemoveUnit(this AOISceneComponent self, AOIUnitComponent unit)
        {
            Log.Info("RemoveUnit:" + unit.Id);
            unit.Scene = null;
            if (unit.Grid != null)
            {
                using (var ListenerGrids = unit.Grid.GetNearbyGrid(unit.Range))
                {
                    for (int i = 0; i < ListenerGrids.Count; i++)
                    {
                        var item = ListenerGrids[i];
                        item.RemoveListener(unit);
                        if (unit.Type == UnitType.Player)
                        {
                            using (var list = item.GetAllUnit())
                            {
                                for (int j = 0; j < list.Count; j++)
                                {
                                    var t = list[j];
                                    Game.EventSystem.Publish(new AOIRemoveUnit()
                                    {
                                        Receive = unit,
                                        Unit = t
                                    });
                                }
                            }
                        }
                    }
                }
                unit.Grid.Remove(unit);
            }
            
        }
        
        /// <summary>
        /// 获取指定位置为中心指定圈数的所有格子
        /// </summary>
        /// <param name="self"></param>
        /// <param name="turnNum"></param>
        /// <param name="posx"></param>
        /// <param name="posy"></param>
        /// <returns></returns>
        public static ListComponent<AOIGrid> GetNearbyGrid(this AOISceneComponent self,int turnNum,int posx,int posy)
        {
            ListComponent<AOIGrid> res = ListComponent<AOIGrid>.Create();
            for (int i = 0; i <= turnNum*2+1; i++)
            {
                var x = posx - turnNum + i;
                for (int j = 0; j <= turnNum * 2 + 1; j++)
                {
                    var y = posy - turnNum + j;
                    res.Add(self.GetCell(x,y));
                }
            }
            return res;
        }
        
        private static AOIGrid GetCell(this AOISceneComponent self, int x,int y,bool create = true)
        {
            long cellId = AOIHelper.CreateCellId(x, y);
            AOIGrid grid = self.GetChild<AOIGrid>(cellId);
            if (grid == null && create)
            {
                grid = self.AddChildWithId<AOIGrid>(cellId);
                grid.xMin = x * self.gridLen;
                grid.xMax = grid.xMin + self.gridLen;
                grid.yMin = y * self.gridLen;
                grid.yMax = grid.yMin + self.gridLen;
                grid.posx = x;
                grid.posy = y;
                grid.halfDiagonal = self.halfDiagonal;
            }

            return grid;
        }
        /// <summary>
        /// 获取指定位置为中心指定圈数的所有格子
        /// </summary>
        /// <param name="self"></param>
        /// <param name="turnNum"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static ListComponent<AOIGrid> GetNearbyGrid(this AOISceneComponent self,int turnNum,Vector3 pos)
        {
            var grid = self.GetAOIGrid(pos);
            ListComponent<AOIGrid> res = ListComponent<AOIGrid>.Create();
            for (int i = 0; i <= turnNum*2+1; i++)
            {
                var x = grid.posx - turnNum + i;
                for (int j = 0; j <= turnNum * 2 + 1; j++)
                {
                    var y = grid.posy - turnNum + j;
                    res.Add(self.GetCell(x,y));
                }
            }
            return res;
        }
    }
}
