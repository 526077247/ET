using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    [ObjectSystem]
    public class AOIGridAwakeSystem : AwakeSystem<AOIGrid>
    {
        public override void Awake(AOIGrid self)
        {
            for (int i = 0; i < (int)CampType.MAX; i++)
            {
                self.idUnits.Add((CampType)i, new Dictionary<long, AOIUnitComponent>());
            }
            self.Triggers = ListComponent<AOITriggerComponent>.Create();
            self.ListenerUnits = ListComponent<AOIUnitComponent>.Create();
        }
    }
    [ObjectSystem]
    public class AOIGridDestroySystem : DestroySystem<AOIGrid>
    {
        public override void Destroy(AOIGrid self)
        {
            self.idUnits.Clear();
            self.Triggers.Dispose();
            self.ListenerUnits.Dispose();
        }
    }
    [FriendClass(typeof(AOIGrid))]
    [FriendClass(typeof(AOITriggerComponent))]
    [FriendClass(typeof(AOIUnitComponent))]
    public static class AOIGridSystem
    {
        /// <summary>
        /// 获取与球形碰撞器的关系：-1无关 0相交或包括碰撞器 1在碰撞器内部
        /// </summary>
        /// <param name="self"></param>
        /// <param name="trigger"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static int GetRelationshipWithSphereTrigger(this AOIGrid self, AOITriggerComponent trigger,
            Vector3 position)
        {
            Vector2 center = new Vector2(position.x,position.z) ;
            // Log.Info(center.x+" "+self.posx+" "+self.xMin+" "+self.xMax);
            // Log.Info(center.y+" "+self.posy+" "+self.yMin+" "+self.yMax);
            //圆心在格子外 0或-1
            if (center.x >= self.xMax) //圆心在格子右方
            {
                // Log.Info("center.x >= self.xMax");
                if (center.y > self.yMax) //圆心在格子右上方
                {
                    if (Vector2.Distance(center, new Vector2(self.xMax, self.yMax)) > trigger.Radius)
                        return -1;
                }
                else if (center.y < self.yMin) //圆心在格子右下方
                {
                    if (Vector2.Distance(center, new Vector2(self.xMax, self.yMin)) > trigger.Radius)
                        return -1;
                }
                else //圆心在格子右侧方
                {
                    if ((center.x - self.xMax) > trigger.Radius)
                        return -1;
                }
            }
            else if (center.x <= self.xMin) //圆心在格子左方
            {
                // Log.Info("center.x <= self.xMin");
                if (center.y >= self.yMax) //圆心在格子左上方
                {
                    if (Vector2.Distance(center, new Vector2(self.xMin, self.yMax)) > trigger.Radius)
                        return -1;
                }
                else if (center.y <= self.yMin) //圆心在格子左下方
                {
                    if (Vector2.Distance(center, new Vector2(self.xMin, self.yMin)) > trigger.Radius)
                        return -1;
                }
                else //圆心在格子左侧方
                {
                    if ((self.xMin - center.x) > trigger.Radius)
                        return -1;
                }
            }
            else if (center.y > self.yMax) //圆心在格子上方
            {
                // Log.Info("center.y > self.yMax");
                if (center.x > self.xMin && center.x < self.xMax) //圆心在格子上侧方
                    if ((center.x - self.yMax) > trigger.Radius)
                        return -1;
            }
            else if (center.y < self.yMin) //圆心在格子下方
            {
                // Log.Info("center.y < self.yMin");
                if (center.x > self.xMin && center.x < self.xMax) //圆心在格子下侧方
                    if ((self.yMin - center.y) > trigger.Radius)
                        return -1;
            }
            //圆心在格子内 0或1
            else if (center.x > self.posx && center.y > self.posy) //圆心在格子内右上方
            {
                // Log.Info("圆心在格子内右上方");
                if (Vector2.Distance(center,new Vector2(self.xMin,self.yMin))<trigger.Radius)
                    return 1;
            }
            else if (center.x > self.posx && center.y < self.posy) //圆心在格子内右下方
            {
                // Log.Info("圆心在格子内右下方");
                if (Vector2.Distance(center,new Vector2(self.xMin,self.yMax))<trigger.Radius)
                    return 1;
            }
            else if (center.x < self.posx && center.y > self.posy) //圆心在格子内左上方
            {
                // Log.Info("圆心在格子内左上方");
                if (Vector2.Distance(center,new Vector2(self.xMax,self.yMin))<trigger.Radius)
                    return 1;
            }
            else if (center.x < self.posx && center.y < self.posy) //圆心在格子内左下方
            {
                // Log.Info("圆心在格子内左下方");
                if (Vector2.Distance(center,new Vector2(self.xMax,self.yMax))<trigger.Radius)
                    return 1;
            }
            //圆心在格子内中心 0或1
            else
            {
                Log.Info("圆心x:" + center.x + " y:" + center.y + " -- 格子x:" + self.posx + " 格子y:" + self.posy);
                if (self.halfDiagonal < trigger.Radius) return 1;
            }
            // Log.Info("0");
            return 0;
        }
        /// <summary>
        /// 获取与碰撞器的关系：-1无关 0相交或包括碰撞器 1在碰撞器内部
        /// </summary>
        /// <param name="self"></param>
        /// <param name="trigger"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static int GetRelationshipWithTrigger(this AOIGrid self, AOITriggerComponent trigger,Vector3? position = null,Quaternion? rotation = null)
        {
            var unit = trigger.GetParent<AOIUnitComponent>();
            Vector3 tempPos;
            if (position != null)
            {
                tempPos = (Vector3) position;
            }
            else
            {
                tempPos = trigger.GetRealPos();
            }
            Quaternion tempRot;
            if (rotation != null)
            {
                tempRot = (Quaternion) rotation;
            }
            else
            {
                tempRot = unit.Rotation;
            }
            var res = self.GetRelationshipWithSphereTrigger(trigger, tempPos);
            if (trigger.TriggerType==TriggerShapeType.Cube&&res>=0)
            {
                var obb = trigger.GetComponent<OBBComponent>();
                //判断格子4个顶点是否在碰撞体内
                if (!obb.IsPointInTrigger(new Vector3(self.xMin, tempPos.y, self.yMin),tempPos,tempRot))
                {
                    return 0;
                }
                if (!obb.IsPointInTrigger(new Vector3(self.xMax, tempPos.y, self.yMin),tempPos,tempRot))
                {
                    return 0;
                }
                if (!obb.IsPointInTrigger(new Vector3(self.xMin, tempPos.y, self.yMax),tempPos,tempRot))
                {
                    return 0;
                }
                if (!obb.IsPointInTrigger(new Vector3(self.xMax, tempPos.y, self.yMax),tempPos,tempRot))
                {
                    return 0;
                }
                Log.Info("tempPos"+tempPos+"  trigger"+trigger.GetRealPos());
                return 1;
            }

            return res;
        }

        /// <summary>
        /// 添加触发器监视
        /// </summary>
        /// <param name="self"></param>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public static void AddTriggerListener(this AOIGrid self, AOITriggerComponent trigger)
        {
            if (Define.Debug)
            {
                if (!trigger.DebugMap.ContainsKey(self)) trigger.DebugMap[self] = 0;
                trigger.DebugMap[self]++;
                trigger.LogInfo.Add("AddTriggerListener "+self.posx+","+self.posy+"  "+DateTime.Now+"\r\n"+new StackTrace());
            }
            self.Triggers.Add(trigger);
        }
        /// <summary>
        /// 移除触发器监视
        /// </summary>
        /// <param name="self"></param>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public static void RemoveTriggerListener(this AOIGrid self, AOITriggerComponent trigger)
        {
            if (Define.Debug)
            {
                if (!trigger.DebugMap.ContainsKey(self)) trigger.DebugMap[self] = 0;
                trigger.DebugMap[self]--;
                trigger.LogInfo.Add("RemoveTriggerListener "+self.posx+","+self.posy+"  "+DateTime.Now+"\r\n"+new StackTrace());
            }
            self.Triggers.Remove(trigger);
        }
        /// <summary>
        /// 添加监视
        /// </summary>
        /// <param name="self"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static void AddListener(this AOIGrid self, AOIUnitComponent unit)
        {
            self.ListenerUnits.Add(unit);
        }
        /// <summary>
        /// 移除监视
        /// </summary>
        /// <param name="self"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static void RemoveListener(this AOIGrid self, AOIUnitComponent unit)
        {
            self.ListenerUnits.Remove(unit);
        }
        /// <summary>
        /// 获取一个AOIUnit
        /// </summary>
        /// <param name="self"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static AOIUnitComponent Get(this AOIGrid self, long id)
        {
            foreach (var item in self.idUnits)
                if(item.Value.TryGetValue(id, out AOIUnitComponent unit))
                    return unit;
            return null;
        }
        /// <summary>
        /// 进入格子
        /// </summary>
        /// <param name="self"></param>
        /// <param name="unit"></param>
        public static void Add(this AOIGrid self, AOIUnitComponent unit)
        {
            unit.Grid = self;
            self.idUnits[unit.Type][unit.Id]= unit;
            for (int i = 0; i < self.ListenerUnits.Count; i++)
            {
                var item = self.ListenerUnits[i];
                if (item.Type == CampType.Player)
                {
                    Game.EventSystem.Publish(new EventType.AOIRegisterUnit()
                    {
                        Receive = item,
                        Unit = unit
                    });
                }
            }
        }

        /// <summary>
        /// 离开
        /// </summary>
        /// <param name="self"></param>
        /// <param name="unit"></param>
        public static void Remove(this AOIGrid self, AOIUnitComponent unit)
        {
            if (self.idUnits.ContainsKey(unit.Type))
            {
                for (int i = 0; i < self.ListenerUnits.Count; i++)
                {
                    var item = self.ListenerUnits[i];
                    if (item.Type == CampType.Player)
                    {
                        Game.EventSystem.Publish(new EventType.AOIRemoveUnit()
                        {
                            Receive = item,
                            Unit = unit
                        });
                    }
                }
                self.idUnits[unit.Type].Remove(unit.Id);
                unit.Grid = null;
            }
        }
        /// <summary>
        /// 离开
        /// </summary>
        /// <param name="self"></param>
        /// <param name="id"></param>
        public static void Remove(this AOIGrid self, long id)
        {
            foreach (var item in self.idUnits)
            {
                if (item.Value.TryGetValue(id, out AOIUnitComponent unit))
                {
                    for (int i = 0; i < self.ListenerUnits.Count; i++)
                    {
                        var temp = self.ListenerUnits[i];
                        if (temp.Type == CampType.Player)
                        {
                            Game.EventSystem.Publish(new EventType.AOIRemoveUnit()
                            {
                                Receive = temp,
                                Unit = unit
                            });
                        }
                    }
                    item.Value.Remove(unit.Id);
                    unit.Grid = null;
                    break;
                }
            }
        }

        /// <summary>
        /// 获取所有指定类型单位
        /// </summary>
        /// <param name="self"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ListComponent<AOIUnitComponent> GetAllUnit(this AOIGrid self, CampType type = CampType.ALL)
        {
            var res = ListComponent<AOIUnitComponent>.Create();
            if (type == CampType.ALL)
            {
                foreach (var item in self.idUnits)
                    res.AddRange(item.Value.Values.ToList());
            }
            else if (self.idUnits.ContainsKey(type))
            {
                res.AddRange(self.idUnits[type].Values.ToList());
            }
            return res;
        }
        /// <summary>
        /// 获取所有指定类型单位
        /// </summary>
        /// <param name="self"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static ListComponent<AOIUnitComponent> GetAllUnit(this AOIGrid self, List<CampType> types)
        {
            var res = ListComponent<AOIUnitComponent>.Create();
            var isAll = types.Contains(CampType.ALL);
            foreach (var item in self.idUnits)
                if (types.Contains(item.Key) || isAll)
                {
                    // Log.Info("GetAllUnit key:"+item.Key);
                    res.AddRange(item.Value.Values.ToList());
                }
            return res;
        }
        /// <summary>
        /// 获取所有指定类型单位的碰撞器
        /// </summary>
        /// <param name="self"></param>
        /// <param name="types"></param>
        /// <param name="except"></param>
        /// <returns></returns>
        public static ListComponent<AOITriggerComponent> GetAllCollider(this AOIGrid self, List<CampType> types,AOITriggerComponent except)
        {
            var res = ListComponent<AOITriggerComponent>.Create();
            var isAll = types.Contains(CampType.ALL);
            for (int i = 0; i < self.Triggers.Count; i++)
            {
                var item = self.Triggers[i];
                if(!item.IsCollider||item==except) continue;
                
                if (isAll||types.Contains(item.GetParent<AOIUnitComponent>().Type))
                {
                    // Log.Info("GetAllUnit key:"+item.Key);
                    res.Add(item);
                }
            }
            return res;
        }
        
        /// <summary>
        /// 获取自身为中心指定圈数的所有格子
        /// </summary>
        /// <param name="self"></param>
        /// <param name="turnNum">圈数</param>
        /// <returns></returns>
        public static ListComponent<AOIGrid> GetNearbyGrid(this AOIGrid self,int turnNum)
        {
            var scene = self.DomainScene().GetComponent<AOISceneComponent>();
            return scene.GetNearbyGrid(turnNum, self.posx, self.posy);
        }

        /// <summary>
        /// 获取所有指定类型单位
        /// </summary>
        /// <param name="self"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ListComponent<AOIUnitComponent> GetAllUnit(this ListComponent<AOIGrid> self, CampType type = CampType.ALL)
        {
            var res = ListComponent<AOIUnitComponent>.Create();
            for (int i = 0; i < self.Count; i++)
            {
                if (type == CampType.ALL)
                    foreach (var item in self[i].idUnits)
                        res.AddRange(item.Value.Values.ToList());
                else if (self[i].idUnits.ContainsKey(type))
                    res.AddRange(self[i].idUnits[type].Values.ToList());
            }
            return res;
        }

        /// <summary>
        /// 获取自身为中心指定圈数的所有格子的所有指定类型单位
        /// </summary>
        /// <param name="self"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ListComponent<AOIUnitComponent> GetNearbyUnit(this AOIGrid self, int turnNum, CampType type = CampType.ALL)
        {
            var grid = self.GetNearbyGrid(turnNum);
            var res = grid.GetAllUnit(type);
            grid.Dispose();
            return res;
        }
    }
}
