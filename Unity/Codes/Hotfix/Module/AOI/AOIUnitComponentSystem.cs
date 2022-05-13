﻿using System;
using System.Collections.Generic;
using System.Linq;
using ET.EventType;
using UnityEngine;

namespace ET
{
    [ObjectSystem]
    public class AOIUnitComponentAwakeSystem : AwakeSystem<AOIUnitComponent,Vector3,Quaternion, UnitType,int>
    {
        public override void Awake(AOIUnitComponent self,Vector3 pos,Quaternion rota, UnitType type,int range)
        {
            self.Position = pos;
            self.Rotation = rota;
            self.Type = type;
            self.Range = range;
            self.DomainScene().GetComponent<AOISceneComponent>().RegisterUnit(self);
        }
    }
    [ObjectSystem]
    public class AOIUnitComponentAwakeSystem2 : AwakeSystem<AOIUnitComponent,Vector3,Quaternion, UnitType>
    {
        public override void Awake(AOIUnitComponent self,Vector3 pos,Quaternion rota, UnitType type)
        {
            self.Position = pos;
            self.Rotation = rota;
            self.Type = type;
            self.Range = 1;
            self.DomainScene().GetComponent<AOISceneComponent>().RegisterUnit(self);
        }
    }
    [ObjectSystem]
    public class AOIUnitComponentDestroySystem : DestroySystem<AOIUnitComponent>
    {
        public override void Destroy(AOIUnitComponent self)
        {
            // Log.Info("RemoveUnit "+self.Id);
            self.Scene.RemoveUnit(self);
        }
    }
    [FriendClass(typeof(AOIUnitComponent))]
    [FriendClass(typeof(AOICell))]
    public static class AOIUnitComponentSystem
    {

        /// <summary>
        /// 获取周围指定圈数指定类型AOI对象
        /// </summary>
        /// <param name="self"></param>
        /// <param name="turnNum"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ListComponent<AOIUnitComponent> GetNearbyUnit(this AOIUnitComponent self,int turnNum= 1, UnitType type = UnitType.ALL)
        {
            if (turnNum < 0) turnNum = self.Range;
            if (self.Cell!=null)
                return self.Cell.GetNearbyUnit(turnNum, type);
            return ListComponent<AOIUnitComponent>.Create();
        }
        /// <summary>
        /// 获取周围指定圈数指定类型AOI对象
        /// </summary>
        /// <param name="self"></param>
        /// <param name="turnNum"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ListComponent<AOICell> GetNearbyGrid(this AOIUnitComponent self,int turnNum= 1)
        {
            if (turnNum < 0) turnNum = self.Range;
            if (self.Cell!=null)
                return self.Cell.GetNearbyGrid(turnNum);
            return ListComponent<AOICell>.Create();
        }
        /// <summary>
        /// 移动一个 AOI 对象, 设置新的 (2D / 3D) 坐标
        /// </summary>
        /// <param name="self"></param>
        /// <param name="position"></param>
        public static void Move(this AOIUnitComponent self,Vector3 position)
        {
            var oldpos = self.Position;
            self.Position = position;
            AOICell cell = self.Scene.GetAOIGrid(position);
            var oldgrid = self.Cell;
            if (cell != oldgrid)//跨格子了：AOI刷新
            {
                self.ChangeTo(cell);
            }
            //触发器刷新 自己进入或离开别人的
            if (self.Collider != null)
            {
                self.Collider.AfterChangeBroadcastToOther(self.Collider.GetRealPos(oldpos),self.Collider.GetRealRot());
            }
            
            //触发器刷新 别人进入或离开自己的
            for (int i = 0; i < self.SphereTriggers.Count; i++)
            {
                var item = self.SphereTriggers[i];
                item.AfterChangePosition(self.Collider.GetRealPos(oldpos));
            }
        }
        /// <summary>
        /// 旋转一个 AOI 对象, 设置新的 (2D / 3D) 方向
        /// </summary>
        /// <param name="self"></param>
        /// <param name="rotation"></param>
        public static void Turn(this AOIUnitComponent self, Quaternion rotation)
        {
            var oldRotation = self.Rotation;
            self.Rotation = rotation;
            //触发器刷新 自己的
            for (int i = 0; i < self.SphereTriggers.Count; i++)
            {
                var item = self.SphereTriggers[i];
                item.AfterChangeRotation(oldRotation);
            }
        }

        /// <summary>
        /// 刷新 AOI 对象，半径范围的其它物体，重新触发 AOI 消息
        /// </summary>
        /// <param name="self"></param>
        public static void RefreshUnit(this AOIUnitComponent self)
        {
            if (self.Type == UnitType.Player)
            {
                // 把周围的人通知给自己
                var units = self.GetNearbyUnit(self.Range);
                for (int i = 0; i < units.Count; i++)
                {
                    var item = units[i];
                    Game.EventSystem.Publish(new AOIRegisterUnit(){Receive = self,Unit = item});
                }
                units.Dispose();
            }
        }
        
        /// <summary>
        /// 改变格子
        /// </summary>
        /// <param name="self"></param>
        /// <param name="newgrid"></param>
        public static void ChangeTo(this AOIUnitComponent self,AOICell newgrid)
        {
            AOICell oldgrid = self.Cell;
            Log.Info(self.Id+"From: "+"  grid x:"+ oldgrid.posx+",y:"+ oldgrid.posy+ "  ChangeTo:grid x:"+ newgrid.posx+",y:"+ newgrid.posy);
            #region 广播给别人
            using (DictionaryComponent<AOIUnitComponent, int> dic = DictionaryComponent<AOIUnitComponent, int>.Create())
            {
                //Remove
                if (oldgrid.idUnits.ContainsKey(self.Type))
                {
                    for (int i = 0; i < oldgrid.ListenerUnits.Count; i++)
                    {
                        var item = oldgrid.ListenerUnits[i];
                        if (item.Type == UnitType.Player&&item!=self)
                        {
                            dic.Add(item, -1);
                        }
                    }

                    oldgrid.idUnits[self.Type].Remove(self);
                    self.Cell = null;
                }
                else
                {
                    Log.Error("unit.Type=" + self.Type + "未添加就删除");
                }

                //Add
                self.Cell = newgrid;
                if (Define.Debug && newgrid.idUnits[self.Type].Contains(self))
                {
                    Log.Error("newgrid.idUnits[self.Type].Contains(self)");
                }
                newgrid.idUnits[self.Type].Add(self);
                for (int i = 0; i < newgrid.ListenerUnits.Count; i++)
                {
                    var item = newgrid.ListenerUnits[i];
                    if (item.Type == UnitType.Player&&item!=self)
                    {
                        if (dic.ContainsKey(item))
                            dic[item] += 1;
                        else
                            dic.Add(item, 1);
                    }
                }

                foreach (var item in dic)
                {
                    if (item.Value > 0)
                        Game.EventSystem.Publish(new EventType.AOIRegisterUnit()
                        {
                            Receive = item.Key,
                            Unit = self,
                        });
                    else if (item.Value < 0)
                        Game.EventSystem.Publish(new EventType.AOIRemoveUnit()
                        {
                            Receive = item.Key,
                            Unit = self
                        });
                }
            }
            #endregion

            #region 广播给自己 && 刷新监听
            var older = oldgrid.GetNearbyGrid(self.Range);
            var newer = newgrid.GetNearbyGrid(self.Range);
            DictionaryComponent<AOICell, int> temp = DictionaryComponent<AOICell, int>.Create();
            for (int i = 0; i < older.Count; i++)
            {
                var item = older[i];
                temp[item] = -1;
            }
            for (int i = 0; i < newer.Count; i++)
            {
                var item = newer[i];
                if (temp.ContainsKey(item))
                    temp[item] = 0;
                else
                    temp[item] = 1;
            }
            ListComponent<AOIUnitComponent> adder = ListComponent<AOIUnitComponent>.Create();
            ListComponent<AOIUnitComponent> remover = ListComponent<AOIUnitComponent>.Create();
            foreach (var item in temp)
            {
                if (item.Value > 0)
                {
                    item.Key.AddListener(self);
                    adder.AddRange(item.Key.GetAllUnit());
                }
                else if (item.Value < 0)
                {
                    item.Key.RemoveListener(self);
                    remover.AddRange(item.Key.GetAllUnit());
                }
            }
            if (self.Type == UnitType.Player)
            {
                for (int i = 0; i < adder.Count; i++)
                {
                    var item = adder[i];
                    if (item == self) continue;
                    Log.Info("AOIRegisterUnit"+item.Id);
                    Game.EventSystem.Publish(new EventType.AOIRegisterUnit
                    {
                        Receive = self,
                        Unit = item
                    });
                }
                for (int i = 0; i < remover.Count; i++)
                {
                    var item = remover[i];
                    if (item == self) continue;
                    Log.Info("AOIRemoveUnit"+item.Id);
                    Game.EventSystem.Publish(new EventType.AOIRemoveUnit()
                    {
                        Receive = self,
                        Unit = item
                    });
                }
            }
            temp.Dispose();
            newer.Dispose();
            older.Dispose();
            adder.Dispose();
            remover.Dispose();
            #endregion

            
        }
        /// <summary>
        /// 获取自己能被谁看到
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static List<AOIUnitComponent> GetBeSeeUnits(this AOIUnitComponent self)
        {
            return self.Cell.ListenerUnits;
        }
    }
    
}