using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ET
{
    [ObjectSystem]
    public class AOITriggerComponentAwakeSystem : AwakeSystem<AOITriggerComponent,float,Action<AOIUnitComponent, AOITriggerType>>
    {
        public override void Awake(AOITriggerComponent self,float a,Action<AOIUnitComponent, AOITriggerType> b)
        {
            if (Define.Debug)
            {
                self.DebugMap = DictionaryComponent<AOIGrid, int>.Create();
                self.LogInfo = ListComponent<string>.Create();
            }
            self.Radius = a;
            self.Handler = b;
        }
    }
    [ObjectSystem]
    [FriendClass(typeof(AOIGrid))]
    public class AOITriggerComponentDestroySystem : DestroySystem<AOITriggerComponent>
    {
        public override void Destroy(AOITriggerComponent self)
        {
            Log.Info("RemoverTrigger"+self.Id);
            if(self.TriggerType!=TriggerShapeType.Cube)//OBB的在子组件处理
                self.GetParent<AOIUnitComponent>().RemoverTrigger(self);
            self.Handler=null;
            
            if (Define.Debug)
            {
                bool hasErr = false;
                foreach (var item in self.DebugMap)
                {
                    if (item.Value != 0)
                    {
                        hasErr = true;
                        Log.Error("碰撞器没完全移除 "+item.Value+"       "+item.Key.posx+","+
                                  item.Key.posy+item.Key.Triggers.Contains(self));
                    }
                }

                if (hasErr)
                {
                    for (int i = 0; i < self.LogInfo.Count; i++)
                    {
                        var item = self.LogInfo[i];
                        Log.Info(item);
                    }
                }
                self.DebugMap.Dispose();
                self.LogInfo.Dispose();
            }
        }
    }
    [FriendClass(typeof(OBBComponent))]
    [FriendClass(typeof(AOITriggerComponent))]
    [FriendClass(typeof(AOIUnitComponent))]
    [FriendClass(typeof(AOISceneComponent))]
    [FriendClass(typeof(AOIGrid))]
    public static class AOITriggerComponentSystem
    {
        public static void OnTrigger(this AOITriggerComponent self, AOITriggerComponent other, AOITriggerType type)
        {
            Log.Info("OnTrigger"+type);
            self.Handler?.Invoke(other.GetParent<AOIUnitComponent>(),type);
        }
        /// <summary>
        /// 获取偏移后的位置
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Vector3 GetRealPos(this AOITriggerComponent self)
        {
            return self.GetParent<AOIUnitComponent>().Position + self.Offset;
        }
        /// <summary>
        /// 获取偏移后的位置
        /// </summary>
        /// <param name="self"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static Vector3 GetRealPos(this AOITriggerComponent self, Vector3 pos)
        {
            return pos + self.Offset;
        }
        /// <summary>
        /// 获取偏移后的位置
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Quaternion GetRealRot(this AOITriggerComponent self)
        {
            return self.GetParent<AOIUnitComponent>().Rotation;
        }
        /// <summary>
        /// 初始化碰撞器数据
        /// </summary>
        /// <param name="self"></param>
        /// <param name="radius"></param>
        /// <param name="type"></param>
        /// <param name="handler"></param>
        /// <param name="isCollider"></param>
        /// <param name="selecter"></param>
        /// <returns></returns>
        static AOITriggerComponent AddTrigger(this AOIUnitComponent self, float radius, AOITriggerType type,
            Action<AOIUnitComponent, AOITriggerType> handler, bool isCollider, params UnitType[] selecter)
        {
            AOITriggerComponent trigger = self.AddChild<AOITriggerComponent,float,Action<AOIUnitComponent, AOITriggerType>>(radius,handler);
            trigger.Flag = type;
            trigger.Selecter = new List<UnitType>(selecter);
            trigger.TriggerType=TriggerShapeType.Sphere;
            trigger.IsCollider = isCollider;
            self.SphereTriggers.Add(trigger);
            return trigger;
        }
        /// <summary>
        /// 添加监听事件，并判断触发进入触发器
        /// </summary>
        /// <param name="self"></param>
        /// <param name="trigger"></param>
        /// <param name="type"></param>
        static void AddTriggerListener(this AOIUnitComponent self,AOITriggerComponent trigger,AOITriggerType type)
        {
            var len = self.Scene.gridLen;
            int count = (int)Math.Ceiling((double)trigger.Radius / len);
            if (count > 2) Log.Info("检测范围超过2格，触发半径："+ trigger.Radius);
            using (var grids = self.GetNearbyGrid(count))
            {
                HashSetComponent<AOITriggerComponent> temp1 = HashSetComponent<AOITriggerComponent>.Create();
                HashSetComponent<AOITriggerComponent> temp2 = HashSetComponent<AOITriggerComponent>.Create();
                for (int i = 0; i < grids.Count; i++)
                {
                    var item = grids[i];
                    var flag = item.GetRelationshipWithTrigger(trigger);
                    // Log.Info("grids pos "+item.posx+" "+item.posy+" flag"+flag);
                    if (flag >= 0)//格子在范围有重叠部分
                    {
                        item.AddTriggerListener(trigger);
                        //别人进入自己
                        if (type == AOITriggerType.All || type == AOITriggerType.Enter)//注意不能放前面判断
                        {
                            using (var colliders = item.GetAllCollider(trigger.Selecter,trigger))
                            {
                                for (int j = 0; j < colliders.Count; j++)
                                {
                                    var collider = colliders[j];
                                    if(collider==trigger) continue;
                                    if (!temp1.Contains(collider)&&trigger.IsInTrigger(collider,trigger.GetRealPos(),
                                            trigger.GetRealRot(),collider.GetRealPos(),collider.GetRealRot()))
                                    {
                                        Log.Info("grids pos "+item.posx+" "+item.posy);
                                        temp1.Add(collider);
                                    }
                                }
                            }
                        }
                        //自己进入别人
                        if (trigger.IsCollider)
                        {
                            for (int j = 0; j < item.Triggers.Count; j++)
                            {
                                var collider = item.Triggers[j];
                                if(collider==trigger) continue;
                                if(collider.Flag!=AOITriggerType.Enter&&collider.Flag!=AOITriggerType.All) continue;
                                if (!temp2.Contains(collider)&&collider.IsInTrigger(trigger,collider.GetRealPos(),
                                        collider.GetRealRot(),trigger.GetRealPos(), trigger.GetRealRot()))
                                {
                                    Log.Info("grids pos "+item.posx+" "+item.posy);
                                    temp2.Add(collider);
                                }
                            }
                        }
                    }
                }

                foreach (var item in temp1)
                {
                    trigger.OnTrigger(item,AOITriggerType.Enter);
                }
                foreach (var item in temp2)
                {
                    item.OnTrigger(trigger,AOITriggerType.Enter);
                }
                temp1.Dispose();
                temp2.Dispose();
            }
        }
        /// <summary>
        /// 添加球形碰撞器
        /// </summary>
        /// <param name="self"></param>
        /// <param name="radius">半径</param>a
        /// <param name="flag">监听进出类型</param>
        /// <param name="handler">当触发发生事件</param>
        /// <param name="isCollider"></param>
        /// <param name="selecter">筛选AOI类型</param>
        /// <returns></returns>
        public static AOITriggerComponent AddSphereTrigger(this AOIUnitComponent self, float radius, AOITriggerType flag, 
            Action<AOIUnitComponent, AOITriggerType> handler,bool isCollider, params UnitType[] selecter)
        {
            if (isCollider && self.Collider != null)
            {
                Log.Error("添加Collider时，Collider已存在");
                return null;
            }
            #region 数据初始化
            var trigger = self.AddTrigger(radius, flag, handler,isCollider, selecter);
            #endregion

            #region 添加监听事件，并判断触发进入触发器

            self.AddTriggerListener(trigger, flag);
            #endregion
            return trigger;
        }
        /// <summary>
        /// 添加立方体碰撞器
        /// </summary>
        /// <param name="self"></param>
        /// <param name="scale">长宽高</param>
        /// <param name="flag">监听进出类型</param>
        /// <param name="handler">当触发发生事件</param>
        /// <param name="isCollider"></param>
        /// <param name="selecter">筛选AOI类型</param>
        /// <returns></returns>
        public static AOITriggerComponent AddOBBTrigger(this AOIUnitComponent self, Vector3 scale, AOITriggerType flag,
            Action<AOIUnitComponent, AOITriggerType> handler,bool isCollider, params UnitType[] selecter)
        {
            if (isCollider && self.Collider != null)
            {
                Log.Error("添加Collider时，Collider已存在");
                return null;
            }
            float radius = Mathf.Sqrt(scale.x*scale.x+scale.y*scale.y+scale.z*scale.z)/2;
            var trigger = self.AddTrigger(radius, flag, handler,isCollider, selecter);
            trigger.AddComponent<OBBComponent, Vector3>(scale);
            trigger.TriggerType=TriggerShapeType.Cube;
            #region 添加监听事件，并判断触发进入触发器

            self.AddTriggerListener(trigger, flag);
            
            #endregion
            return trigger;
        }
        
        
        
        /// <summary>
        /// 移除碰撞器
        /// </summary>
        /// <param name="self"></param>
        /// <param name="trigger"></param>
        public static void RemoverTrigger(this AOIUnitComponent self, AOITriggerComponent trigger)
        {
            self.SphereTriggers.Remove(trigger);
            #region 添加监听事件，并判断触发离开触发器
            var len = self.Scene.gridLen;
            int count = (int)Math.Ceiling((double)trigger.Radius / len);
            if (count > 2) Log.Info("检测范围超过2格，触发半径："+ trigger.Radius);
            using (var grids = self.GetNearbyGrid(count))
            {
                HashSetComponent<AOITriggerComponent> temp = HashSetComponent<AOITriggerComponent>.Create();
                for (int i = 0; i < grids.Count; i++)
                {
                    var item = grids[i];
                    var flag = item.GetRelationshipWithTrigger(trigger);
                    if (flag >= 0)//格子与范围不完全重叠
                    {
                        item.RemoveTriggerListener(trigger);
                        //离开触发器
                        if (trigger.Flag == AOITriggerType.All || trigger.Flag == AOITriggerType.Exit)//注意不能放前面判断
                        {
                            using (var colliders = item.GetAllCollider(trigger.Selecter,trigger))
                            {
                                for (int j = 0; j < colliders.Count; j++)
                                {
                                    var collider = colliders[j];
                                    if (trigger.IsInTrigger(collider,trigger.GetRealPos(),
                                            trigger.GetRealRot(),collider.GetRealPos(),collider.GetRealRot()))
                                    {
                                        if(collider==trigger) continue;
                                        temp.Add(collider);
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var item in temp)
                {
                    trigger.OnTrigger(item,AOITriggerType.Exit);
                }

                temp.Dispose();
            }
            trigger.Dispose();
            #endregion
        }
        

        /// <summary>
        /// 自己改变坐标后，看别人有没有进来或离开
        /// </summary>
        /// <param name="self"></param>
        /// <param name="before"></param>
        public static void AfterChangePosition(this AOITriggerComponent self,Vector3 before)
        {
            self.AfterChangeBroadcastToMe(before,self.GetRealRot());
        }

        /// <summary>
        /// 自己改变方向后，看别人有没有进来或离开
        /// </summary>
        /// <param name="self"></param>
        /// <param name="before"></param>
        public static void AfterChangeRotation(this AOITriggerComponent self,Quaternion before)
        {
            if (self.TriggerType!=TriggerShapeType.Cube) return;
            self.AfterChangeBroadcastToMe(self.GetRealPos(), before);
        }
        /// <summary>
        /// 自己坐标方向改变后，看别人有没有进来或离开
        /// </summary>
        /// <param name="self"></param>
        /// <param name="beforePosition"></param>
        /// <param name="beforeRotation"></param>
        public static void AfterChangeBroadcastToMe(this AOITriggerComponent self,Vector3 beforePosition,Quaternion beforeRotation)
        {
            var unit = self.GetParent<AOIUnitComponent>();
            var len = unit.Scene.gridLen;
            int count = (int)Math.Ceiling((double)self.Radius / len);
            if (count > 2) Log.Info("检测范围超过2格，触发半径："+ self.Radius);
            DictionaryComponent<AOIGrid,int> triggers = DictionaryComponent<AOIGrid, int>.Create();
            DictionaryComponent<AOITriggerComponent,int> colliderDic = DictionaryComponent<AOITriggerComponent, int>.Create();

            using (var grids = unit.Scene.GetNearbyGrid(count, beforePosition))
            {
                for (int i = 0; i < grids.Count; i++)
                {
                    var item = grids[i];
                    //旧的
                    var flag = item.GetRelationshipWithTrigger(self,beforePosition,beforeRotation);
                    if (flag >= 0) //格子在范围内部
                    {
                        triggers.Add(item, -1);
                    }
                    // Log.Info("old "+flag+" "+ item.posx+","+item.posy);
                }
            }

            using (var grids = unit.Scene.GetNearbyGrid(count, unit.Position))
            {
                //新的
                for (int i = 0; i < grids.Count; i++)
                {
                    var item = grids[i];
                    var flag = item.GetRelationshipWithTrigger(self);
                    if ( flag>= 0)//格子在范围内部
                    {
                        if (triggers.ContainsKey(item))
                            triggers[item]++;
                        else
                            triggers.Add(item,1);
                    }
                    // Log.Info("new "+flag+" "+ item.posx+","+item.posy);
                }
            }

            #region 筛选格子里的单位
            HashSetComponent<AOITriggerComponent> pre = HashSetComponent<AOITriggerComponent>.Create();//之前有的
            HashSetComponent<AOITriggerComponent> after = HashSetComponent<AOITriggerComponent>.Create();//现在有的
            //不完全包围的格子需要逐个计算
            foreach (var item in triggers)
            {
                if (item.Value > 0)//之前无现在有
                {
                    item.Key.AddTriggerListener(self);
                    using (var colliders = item.Key.GetAllCollider(self.Selecter,self))
                    {
                        for (int i = 0; i < colliders.Count; i++)
                        {
                            var collider = colliders[i];
                            if (!pre.Contains(collider)&&self.IsInTrigger(collider,self.GetRealPos(),
                                    self.GetRealRot(),collider.GetRealPos(),collider.GetRealRot()))
                            {
                                pre.Add(collider);
                            }
                            
                        }
                    }
                }
                else if (item.Value < 0)//之前有现在无
                {
                    item.Key.RemoveTriggerListener(self);
                    using (var colliders = item.Key.GetAllCollider(self.Selecter,self))
                    {
                        for (int i = 0; i < colliders.Count; i++)
                        {
                            var collider = colliders[i];
                            if (!after.Contains(collider)&&self.IsInTrigger(collider,self.GetRealPos(beforePosition),
                                    beforeRotation,collider.GetRealPos(),collider.GetRealRot()))
                            {
                                after.Add(collider);
                            }
                            
                        }
                    }
                }
                else//之前有现在有，但坐标变了
                {
                    using (var colliders = item.Key.GetAllCollider(self.Selecter,self))
                    {
                        for (int i = 0; i < colliders.Count; i++)
                        {
                            var collider = colliders[i];
                            if (!after.Contains(collider)&&self.IsInTrigger(collider,self.GetRealPos(beforePosition),
                                    beforeRotation,collider.GetRealPos(),collider.GetRealRot()))
                            {
                                after.Add(collider);
                            }
                            if (!pre.Contains(collider)&&self.IsInTrigger(collider,self.GetRealPos(),
                                    self.GetRealRot(),collider.GetRealPos(),collider.GetRealRot()))
                            {
                                pre.Add(collider);
                            }
                        }
                    }
                }

                
            }
            foreach (var collider in pre)
            {
                colliderDic.Add(collider,1);
            }
            foreach (var collider in after)
            {
                if (colliderDic.ContainsKey(collider))
                {
                    colliderDic[collider]--;
                }
                else
                {
                    colliderDic.Add(collider,-1);
                }
                
            }
            pre.Dispose();
            after.Dispose();
            #endregion
            //判断事件
            foreach (var item in colliderDic)
            {
                if(self==item.Key) continue;
                if (item.Value <0 &&(self.Flag == AOITriggerType.All || self.Flag == AOITriggerType.Exit))//离开
                {
                    self.OnTrigger(item.Key,AOITriggerType.Exit);
                }
                else if (item.Value >0 &&(self.Flag == AOITriggerType.All || self.Flag == AOITriggerType.Enter))//进入
                {
                    self.OnTrigger(item.Key,AOITriggerType.Enter);
                }
 
            }
            
            triggers.Dispose();
            colliderDic.Dispose();
        }
        /// <summary>
        /// 自己坐标方向改变后，有没有进来或离开别人
        /// </summary>
        /// <param name="self"></param>
        /// <param name="beforePosition"></param>
        /// <param name="beforeRotation"></param>
        public static void AfterChangeBroadcastToOther(this AOITriggerComponent self,Vector3 beforePosition,Quaternion beforeRotation)
        {
            if(!self.IsCollider) return;
            var unit = self.GetParent<AOIUnitComponent>();
            var len = unit.Scene.gridLen;
            int count = (int)Math.Ceiling((double)self.Radius / len);
            if (count > 2) Log.Info("检测范围超过2格，触发半径："+ self.Radius);
            DictionaryComponent<AOIGrid,int> triggers = DictionaryComponent<AOIGrid, int>.Create();
            DictionaryComponent<AOITriggerComponent,int> colliderDic = DictionaryComponent<AOITriggerComponent, int>.Create();

            using (var grids = unit.Scene.GetNearbyGrid(count, beforePosition))
            {
                for (int i = 0; i < grids.Count; i++)
                {
                    var item = grids[i];
                    //旧的
                    var flag = item.GetRelationshipWithTrigger(self,beforePosition,beforeRotation);
                    if (flag >= 0) //格子在范围内部
                    {
                        triggers.Add(item, -1);
                    }
                    // Log.Info("old "+flag+" "+ item.posx+","+item.posy);
                }
            }

            using (var grids = unit.Scene.GetNearbyGrid(count, unit.Position))
            {
                //新的
                for (int i = 0; i < grids.Count; i++)
                {
                    var item = grids[i];
                    var flag = item.GetRelationshipWithTrigger(self);
                    if ( flag>= 0)//格子在范围内部
                    {
                        if (triggers.ContainsKey(item))
                            triggers[item]++;
                        else
                            triggers.Add(item,1);
                    }
                    // Log.Info("new "+flag+" "+ item.posx+","+item.posy);
                }
            }

            #region 筛选格子里的单位
            HashSetComponent<AOITriggerComponent> pre = HashSetComponent<AOITriggerComponent>.Create();//之前有的
            HashSetComponent<AOITriggerComponent> after = HashSetComponent<AOITriggerComponent>.Create();//现在有的
            //不完全包围的格子需要逐个计算
            foreach (var item in triggers)
            {
                if (item.Value > 0)//之前无现在有
                {
                    // item.Key.AddTriggerListener(self);
                    for (int i = 0; i < item.Key.Triggers.Count; i++)
                    {
                        var collider = item.Key.Triggers[i];
                        if (!pre.Contains(collider)&&self.IsInTrigger(collider,self.GetRealPos(),
                                self.GetRealRot(),collider.GetRealPos(),collider.GetRealRot()))
                        {
                            pre.Add(collider);
                        }
                            
                    }
                }
                else if (item.Value < 0)//之前有现在无
                {
                    // item.Key.RemoveTriggerListener(self);
                    for (int i = 0; i < item.Key.Triggers.Count; i++)
                    {
                        var collider = item.Key.Triggers[i];
                        if (!after.Contains(collider)&&self.IsInTrigger(collider,self.GetRealPos(beforePosition),
                                beforeRotation,collider.GetRealPos(),collider.GetRealRot()))
                        {
                            after.Add(collider);
                        }
                            
                    }
                }
                else//之前有现在有，但坐标变了
                {
                    for (int i = 0; i < item.Key.Triggers.Count; i++)
                    {
                        var collider = item.Key.Triggers[i];
                        if (!after.Contains(collider)&&self.IsInTrigger(collider,self.GetRealPos(beforePosition),
                                beforeRotation,collider.GetRealPos(),collider.GetRealRot()))
                        {
                            after.Add(collider);
                        }
                        if (!pre.Contains(collider)&&self.IsInTrigger(collider,self.GetRealPos(),
                                self.GetRealRot(),collider.GetRealPos(),collider.GetRealRot()))
                        {
                            pre.Add(collider);
                        }
                    }
                }

                
            }
            foreach (var collider in pre)
            {
                colliderDic.Add(collider,1);
            }
            foreach (var collider in after)
            {
                if (colliderDic.ContainsKey(collider))
                {
                    colliderDic[collider]--;
                }
                else
                {
                    colliderDic.Add(collider,-1);
                }
                
            }
            pre.Dispose();
            after.Dispose();
            #endregion
            //判断事件
            foreach (var item in colliderDic)
            {
                if(self==item.Key) continue;
                if (item.Value <0 &&(self.Flag == AOITriggerType.All || self.Flag == AOITriggerType.Exit))//离开
                {
                    item.Key.OnTrigger(self,AOITriggerType.Exit);
                }
                else if (item.Value >0 &&(self.Flag == AOITriggerType.All || self.Flag == AOITriggerType.Enter))//进入
                {
                    item.Key.OnTrigger(self,AOITriggerType.Enter);
                }
 
            }
            
            triggers.Dispose();
            colliderDic.Dispose();
        }
        
        
        /// <summary>
        /// 自己坐标方向改变后，别人or别人自己都判断有没有进来或离开
        /// </summary>
        /// <param name="self"></param>
        /// <param name="beforePosition"></param>
        /// <param name="beforeRotation"></param>
        public static void AfterChange(this AOITriggerComponent self,Vector3 beforePosition,Quaternion beforeRotation)
        {
            var unit = self.GetParent<AOIUnitComponent>();
            var len = unit.Scene.gridLen;
            int count = (int)Math.Ceiling((double)self.Radius / len);
            if (count > 2) Log.Info("检测范围超过2格，触发半径："+ self.Radius);
            DictionaryComponent<AOIGrid,int> triggers = DictionaryComponent<AOIGrid, int>.Create();
            DictionaryComponent<AOITriggerComponent,int> colliderDic = DictionaryComponent<AOITriggerComponent, int>.Create();

            using (var grids = unit.Scene.GetNearbyGrid(count, beforePosition))
            {
                for (int i = 0; i < grids.Count; i++)
                {
                    var item = grids[i];
                    //旧的
                    var flag = item.GetRelationshipWithTrigger(self,beforePosition,beforeRotation);
                    if (flag >= 0) //格子在范围内部
                    {
                        triggers.Add(item, -1);
                    }
                    // Log.Info("old "+flag+" "+ item.posx+","+item.posy);
                }
            }

            using (var grids = unit.Scene.GetNearbyGrid(count, unit.Position))
            {
                //新的
                for (int i = 0; i < grids.Count; i++)
                {
                    var item = grids[i];
                    var flag = item.GetRelationshipWithTrigger(self);
                    if ( flag>= 0)//格子在范围内部
                    {
                        if (triggers.ContainsKey(item))
                            triggers[item]++;
                        else
                            triggers.Add(item,1);
                    }
                    // Log.Info("new "+flag+" "+ item.posx+","+item.posy);
                }
            }

            #region 筛选格子里的单位
            HashSetComponent<AOITriggerComponent> pre = HashSetComponent<AOITriggerComponent>.Create();//之前有的
            HashSetComponent<AOITriggerComponent> after = HashSetComponent<AOITriggerComponent>.Create();//现在有的
            //完全包围的格子不需要逐个计算
            foreach (var item in triggers)
            {
                if (item.Value > 0)
                {
                    item.Key.AddTriggerListener(self);
                    for (int i = 0; i < item.Key.Triggers.Count; i++)
                    {
                        var collider = item.Key.Triggers[i];
                        pre.Add(collider);
                    }
                }
                else if (item.Value < 0)
                {
                    item.Key.RemoveTriggerListener(self);
                    for (int i = 0; i < item.Key.Triggers.Count; i++)
                    {
                        var collider = item.Key.Triggers[i];
                        after.Add(collider);
                    }
                }
            }
            //不完全包围的格子需要逐个计算
            foreach (var item in triggers)
            {
                if (item.Value > 0)//之前无现在有
                {
                    item.Key.AddTriggerListener(self);
                    for (int i = 0; i < item.Key.Triggers.Count; i++)
                    {
                        var collider = item.Key.Triggers[i];
                        if (!pre.Contains(collider)&&self.IsInTrigger(collider,self.GetRealPos(),
                                self.GetRealRot(),collider.GetRealPos(),collider.GetRealRot()))
                        {
                            pre.Add(collider);
                        }
                            
                    }
                }
                else if (item.Value < 0)//之前有现在无
                {
                    item.Key.RemoveTriggerListener(self);
                    for (int i = 0; i < item.Key.Triggers.Count; i++)
                    {
                        var collider = item.Key.Triggers[i];
                        if (!after.Contains(collider)&&self.IsInTrigger(collider,self.GetRealPos(beforePosition),
                                beforeRotation,collider.GetRealPos(),collider.GetRealRot()))
                        {
                            after.Add(collider);
                        }
                            
                    }
                }
                else//之前有现在有，但坐标变了
                {
                    for (int i = 0; i < item.Key.Triggers.Count; i++)
                    {
                        var collider = item.Key.Triggers[i];
                        if (!after.Contains(collider)&&self.IsInTrigger(collider,self.GetRealPos(beforePosition),
                                beforeRotation,collider.GetRealPos(),collider.GetRealRot()))
                        {
                            after.Add(collider);
                        }
                        if (!pre.Contains(collider)&&self.IsInTrigger(collider,self.GetRealPos(),
                                self.GetRealRot(),collider.GetRealPos(),collider.GetRealRot()))
                        {
                            pre.Add(collider);
                        }
                    }
                }

                
            }
            foreach (var collider in pre)
            {
                colliderDic.Add(collider,-1);
            }
            foreach (var collider in after)
            {
                if (colliderDic.ContainsKey(collider))
                {
                    colliderDic[collider]++;
                }
                else
                {
                    colliderDic.Add(collider,1);
                }
                
            }
            pre.Dispose();
            after.Dispose();
            #endregion
            //判断事件
            foreach (var item in colliderDic)
            {
                if(self==item.Key) continue;
                if (item.Value <0 &&(self.Flag == AOITriggerType.All || self.Flag == AOITriggerType.Exit))//离开
                {
                    if(self.IsCollider)
                        item.Key.OnTrigger(self,AOITriggerType.Exit);
                    else if(item.Key.IsCollider)
                        self.OnTrigger(item.Key,AOITriggerType.Exit);
                }
                else if (item.Value >0 &&(self.Flag == AOITriggerType.All || self.Flag == AOITriggerType.Enter))//进入
                {
                    if(self.IsCollider)
                        item.Key.OnTrigger(self,AOITriggerType.Enter);
                    else if(item.Key.IsCollider)
                        self.OnTrigger(item.Key,AOITriggerType.Enter);
                }
 
            }
            
            triggers.Dispose();
            colliderDic.Dispose();
        }
        
        /// <summary>
        /// 判断是否触发
        /// </summary>
        /// <param name="trigger1"></param>
        /// <param name="trigger2"></param>
        /// <param name="position1"></param>
        /// <param name="rotation1"></param>
        /// <param name="position2"></param>
        /// <param name="rotation2"></param>
        /// <returns></returns>
        public static bool IsInTrigger(this AOITriggerComponent trigger1, AOITriggerComponent trigger2,
            Vector3 position1, Quaternion rotation1, Vector3 position2, Quaternion rotation2)
        {
            if(trigger1==null||trigger2==null)
            {
                return false;
            }
            if (!trigger1.IsCollider && !trigger2.IsCollider)//至少一个为碰撞器
            {
                return false;
            }
            // Log.Info("IsInTrigger");
            var pos1 = trigger1.GetRealPos(position1);
            var pos2 = trigger1.GetRealPos(position2);
            var dis = Vector3.Distance(pos1, pos2);
            // Log.Info("dis"+dis+"pos1"+pos1+"pos2"+pos2+"trigger1.Radius"+trigger1.Radius+"trigger2.Radius"+trigger2.Radius);
            bool isSphereTrigger = trigger1.Radius+trigger2.Radius > dis;
            if (trigger1.TriggerType == TriggerShapeType.Sphere && trigger2.TriggerType == TriggerShapeType.Sphere)//判断球触发
            {
                // Log.Info("判断球触发");
                return isSphereTrigger;
            }
            if (!isSphereTrigger) return false;//外接球不相交
            if (trigger1.TriggerType == TriggerShapeType.Cube && trigger2.TriggerType == TriggerShapeType.Cube)//判断OBB触发
            {
                // Log.Info("判断OBB触发");
                //一方有一个点在对方内部即为触发
                using (var list = trigger1.GetComponent<OBBComponent>().GetAllVertex())
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if(IsPointInTrigger(trigger2,list[i],pos2,rotation2))
                        {
                            return true;
                        }
                    }
                }
                using (var list = trigger2.GetComponent<OBBComponent>().GetAllVertex())
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if(IsPointInTrigger(trigger1,list[i],pos1,rotation1))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            else if(trigger1.TriggerType <= TriggerShapeType.Cube && trigger2.TriggerType <= TriggerShapeType.Cube)//判断OBB和球触发
            {
                // Log.Info("判断OBB和球触发");
                var triggerOBB = trigger1;
                var triggerSphere = trigger2;
                var posOBB = pos1;
                var posSp = pos2;
                var rotOBB = rotation1;
                if (trigger2.TriggerType == TriggerShapeType.Cube)
                {
                    triggerOBB = trigger2;
                    triggerSphere = trigger1;
                    posOBB = pos2;
                    posSp = pos1;
                    rotOBB = rotation2;
                }
                var obb = triggerOBB.GetComponent<OBBComponent>();
                Vector3 temp = Quaternion.Inverse(rotOBB)*(posSp - posOBB); //转换到触发器模型空间坐标
                var xMax = obb.Scale.x / 2;
                var yMax = obb.Scale.y / 2;
                var zMax = obb.Scale.z / 2;
                if (-xMax <= temp.x && temp.x <= xMax && -yMax <= temp.y && temp.y <= yMax &&
                    -zMax <= temp.z && temp.z <= zMax)//球心在立方体内
                {
                    // Log.Info("球心在立方体内");
                    return true;
                }

                if (-xMax - triggerSphere.Radius > temp.x || temp.x > xMax + triggerSphere.Radius ||
                    -yMax - triggerSphere.Radius > temp.y || temp.y > yMax + triggerSphere.Radius ||
                    -zMax - triggerSphere.Radius > temp.z || temp.z > zMax + triggerSphere.Radius)//球心离立方体超过半径
                {
                    // Log.Info("球心离立方体超过半径 xMax"+xMax+" yMax"+yMax+" zMax"+zMax+" Radius"+triggerSphere.Radius+" temp"+temp);
                    return false;
                }
                // Log.Info("一个轴出立方");
                //一个轴出立方
                if (-xMax <= temp.x && temp.x <= xMax && -yMax <= temp.y && temp.y <= yMax)//z方向不在立方体内
                {
                    return -zMax - triggerSphere.Radius <= temp.z && temp.z <= zMax + triggerSphere.Radius;
                }
                if (-yMax <= temp.y && temp.y <= yMax && -zMax <= temp.z && temp.z <= zMax)//x方向不在立方体内
                {
                    return -xMax - triggerSphere.Radius <= temp.x && temp.x <= xMax + triggerSphere.Radius;
                }
                if (-xMax <= temp.x && temp.x <= xMax && -zMax <= temp.z && temp.z <= zMax)//y方向不在立方体内
                {
                    return -yMax - triggerSphere.Radius <= temp.y && temp.y <= yMax + triggerSphere.Radius;
                }

                #region 两个轴出立方

                // Log.Info("两个轴出立方");
                //两个轴出立方
                if (-xMax <= temp.x && temp.x <= xMax)
                {
                    if (-yMax > temp.y&& -zMax > temp.z)
                    {
                        return Vector3.Distance(temp, new Vector3(temp.x, -yMax, -zMax)) <= triggerSphere.Radius;
                    }
                    if (-yMax > temp.y&& temp.z > zMax)
                    {
                        return Vector3.Distance(temp, new Vector3(temp.x, -yMax, zMax)) <= triggerSphere.Radius;
                    }
                    if (temp.y > yMax && -zMax > temp.z)
                    {
                        return Vector3.Distance(temp, new Vector3(temp.x, yMax, -zMax)) <= triggerSphere.Radius;
                    }
                    if (temp.y > yMax && temp.z > zMax)
                    {
                        return Vector3.Distance(temp, new Vector3(temp.x, yMax, zMax)) <= triggerSphere.Radius;
                    }
                }
                //两个轴出立方
                if (-yMax <= temp.y && temp.y <= yMax)
                {
                    if (-xMax > temp.x&& -zMax > temp.z)
                    {
                        return Vector3.Distance(temp, new Vector3(-xMax, temp.y, -zMax)) <= triggerSphere.Radius;
                    }
                    if (-xMax > temp.x&& temp.z > zMax)
                    {
                        return Vector3.Distance(temp, new Vector3(-xMax, temp.y, zMax)) <= triggerSphere.Radius;
                    }
                    if (temp.x > xMax && -zMax > temp.z)
                    {
                        return Vector3.Distance(temp, new Vector3(xMax, temp.y, -zMax)) <= triggerSphere.Radius;
                    }
                    if (temp.x > xMax && temp.z > zMax)
                    {
                        return Vector3.Distance(temp, new Vector3(xMax, temp.y, zMax)) <= triggerSphere.Radius;
                    }
                }
                //两个轴出立方
                if (-zMax <= temp.z && temp.z <= zMax)
                {
                    if (-yMax > temp.y&& -xMax > temp.x)
                    {
                        return Vector3.Distance(temp, new Vector3(-xMax, -yMax, temp.z)) <= triggerSphere.Radius;
                    }
                    if (-yMax > temp.y&& temp.x > xMax)
                    {
                        return Vector3.Distance(temp, new Vector3(xMax, -yMax, temp.z)) <= triggerSphere.Radius;
                    }
                    if (temp.y > yMax && -xMax > temp.x)
                    {
                        return Vector3.Distance(temp, new Vector3(-xMax, yMax, temp.z)) <= triggerSphere.Radius;
                    }
                    if (temp.y > yMax && temp.x > xMax)
                    {
                        return Vector3.Distance(temp, new Vector3(xMax, yMax, temp.z)) <= triggerSphere.Radius;
                    }
                }
                #endregion
                Log.Info("离8个角较近的位置");
                //离8个角较近的位置
                using (var points = obb.GetAllVertex())
                {
                    for (int i = 0; i < points.Count; i++)
                    {
                        if (Vector3.Distance(temp, points[i]) > triggerSphere.Radius)
                        {
                            return false;
                        }
                    }
                }
                return true;

            }
            else//未处理
            {
                Log.Error("未处理的触发器触发判断，类型 trigger1.TriggerType="+trigger1.TriggerType+"; trigger2.TriggerType="+trigger2.TriggerType);
            }

            return false;
        }
        
        /// <summary>
        /// 判断某个点是否在触发器移到指定位置后之内
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="position"></param>
        /// <param name="center"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static bool IsPointInTrigger(this AOITriggerComponent trigger, Vector3 position,Vector3 center,Quaternion rotation)
        {
            var dis = Vector3.Distance(center, position);
            if (trigger.Radius < dis) return false;
            if (trigger.TriggerType==TriggerShapeType.Cube)
            {
                return trigger.GetComponent<OBBComponent>().IsPointInTrigger(position,center,rotation);
            }
            return true;
        }


        /// <summary>
        /// 判断射线是否在触发器移到指定位置后之内
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="ray"></param>
        /// <param name="center"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static bool IsRayInTrigger(this AOITriggerComponent trigger,Ray ray, Vector3 center,Quaternion rotation)
        {
            //求点到直线的距离
            var dis = Math.Sqrt(Vector3.Cross(ray.Start - center, ray.Dir).sqrMagnitude / ray.Dir.sqrMagnitude);
            if (dis > trigger.Radius) return false;
            var disHitStartSqr = (ray.Start - center).sqrMagnitude - dis * dis;
            var hit = ray.Start + (float)(Math.Sqrt(disHitStartSqr)-Math.Sqrt(trigger.Radius * trigger.Radius - dis * dis))
                *ray.Dir;
            if (ray.Distance * ray.Distance< disHitStartSqr||Math.Abs(Vector3.Distance(hit, center) - trigger.Radius) > 0.1)
            {
                return false;//即使距离小于半径，由于是射线也要判断一下
            }
            if (trigger.TriggerType == TriggerShapeType.Cube)
            {
                return trigger.GetComponent<OBBComponent>().IsRayInTrigger(ray, center, rotation,out hit);
            }
            return true;
        }
        
        /// <summary>
        /// 判断射线是否在触发器移到指定位置后之内
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="ray"></param>
        /// <param name="center"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static bool IsRayInTrigger(this AOITriggerComponent trigger,Ray ray, Vector3 center,Quaternion rotation,out Vector3 hit)
        {
            hit = Vector3.zero;
            //求点到直线的距离
            var dis = Math.Sqrt(Vector3.Cross(ray.Start - center, ray.Dir).sqrMagnitude / ray.Dir.sqrMagnitude);
            if (dis > trigger.Radius) return false;
            var disHitStartSqr = (ray.Start - center).sqrMagnitude - dis * dis;
            hit = ray.Start + (float)(Math.Sqrt(disHitStartSqr)-Math.Sqrt(trigger.Radius * trigger.Radius - dis * dis))
                *ray.Dir;
            if (ray.Distance * ray.Distance< disHitStartSqr||Math.Abs(Vector3.Distance(hit, center) - trigger.Radius) > 0.1)
            {
                return false;//即使距离小于半径，由于是射线也要判断一下
            }
            if (trigger.TriggerType == TriggerShapeType.Cube)
            {
                return trigger.GetComponent<OBBComponent>().IsRayInTrigger(ray, center, rotation,out hit);
            }
            return true;
        }
    }
}