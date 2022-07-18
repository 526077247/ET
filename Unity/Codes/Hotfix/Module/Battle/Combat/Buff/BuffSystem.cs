namespace ET
{
    [Timer(TimerType.RemoveBuff)]
    public class RemoveBuff: ATimer<Buff>
    {
        public override void Run(Buff self)
        {
            try
            {
                if(self==null||self.IsDisposed) return;
                self.GetParent<BuffComponent>().Remove(self.Id);
            }
            catch (System.Exception e)
            {
                Log.Error($"move timer error: {self.Id}\n{e}");
            }
        }
    }

    [FriendClass(typeof(BuffComponent))]
    [ObjectSystem]
    public class BuffAwakeSystem : AwakeSystem<Buff,int,long,long>
    {
        public override void Awake(Buff self,int id,long timestamp,long sourceId)
        {
            Log.Info("添加BUFF id="+id);
            self.ConfigId = id;
            self.Timestamp = timestamp;
            self.FromUnitId = sourceId;
            var buffComp = self.GetParent<BuffComponent>();
            var unit = buffComp.unit;
            if ((self.Config.Type&BuffSubType.Attribute)!=0 )
                self.AddBuffAttrValue(unit);
            if ((self.Config.Type&BuffSubType.ActionControl)!=0 )
               self.AddBuffActionControl(unit);
            if(timestamp>=0)
                self.TimerId = TimerComponent.Instance.NewOnceTimer(timestamp, TimerType.RemoveBuff, self);
        }
    }
    [FriendClass(typeof(BuffComponent))]
    [ObjectSystem]
    public class BuffAwakeSystem1 : AwakeSystem<Buff,int,long,bool,long>
    {
        public override void Awake(Buff self,int id,long timestamp,bool ignoreAddAttr,long sourceId)
        {
            Log.Info("添加BUFF id="+id);
            self.ConfigId = id;
            self.Timestamp = timestamp;
            self.FromUnitId = sourceId;
            var buffComp = self.GetParent<BuffComponent>();
            var unit = buffComp.unit;
            if(!ignoreAddAttr && (self.Config.Type&BuffSubType.Attribute)!=0 )
                self.AddBuffAttrValue(unit);
            if ((self.Config.Type&BuffSubType.ActionControl)!=0 )
                self.AddBuffActionControl(unit);
            if(timestamp>=0)
                self.TimerId = TimerComponent.Instance.NewOnceTimer(timestamp, TimerType.RemoveBuff, self);
        }
    }
    [FriendClass(typeof(BuffComponent))]
    [ObjectSystem]
    public class BuffDestroySystem : DestroySystem<Buff>
    {
        public override void Destroy(Buff self)
        {
            TimerComponent.Instance.Remove(ref self.TimerId);
            Log.Info("移除BUFF id="+self.ConfigId);
            var buffComp = self.GetParent<BuffComponent>();
            var unit = buffComp.unit;
            if ((self.Config.Type&BuffSubType.Attribute)!=0 && self.AttrConfig.IsRemove == 0) //结束后是否移除加成（0:是）
                self.RemoveBuffAttrValue(unit);
            if ((self.Config.Type & BuffSubType.ActionControl) != 0)
                self.RemoveBuffActionControl(unit);

        }
    }

    [FriendClass(typeof(Buff))]
    [FriendClass(typeof(BuffComponent))]
    public static class BuffSystem
    {
        /// <summary>
        /// 添加BUFF属性加成
        /// </summary>
        /// <param name="self"></param>
        /// <param name="unit"></param>
        public static void AddBuffAttrValue(this Buff self, Unit unit)
        {
            if (self.AttrConfig.AttributeType != null)
            {
                var numc = unit.GetComponent<NumericComponent>();
                if (numc != null)
                {
                    for (int i = 0; i < self.AttrConfig.AttributeType.Length; i++)
                    {
                        if (NumericType.Map.TryGetValue(self.AttrConfig.AttributeType[i], out var attr))
                        {
                            if (self.AttrConfig.AttributeAdd != null && self.AttrConfig.AttributeAdd.Length > i)
                                numc.Set(attr * 10 + 2, numc.GetAsInt(attr * 10 + 2) + self.AttrConfig.AttributeAdd[i]);
                            if (self.AttrConfig.AttributePct != null && self.AttrConfig.AttributePct.Length > i)
                                numc.Set(attr * 10 + 3, numc.GetAsInt(attr * 10 + 3) + self.AttrConfig.AttributePct[i]);
                            if (self.AttrConfig.AttributeFinalAdd != null && self.AttrConfig.AttributeFinalAdd.Length > i)
                                numc.Set(attr * 10 + 4,
                                    numc.GetAsInt(attr * 10 + 4) + self.AttrConfig.AttributeFinalAdd[i]);
                            if (self.AttrConfig.AttributeFinalPct != null && self.AttrConfig.AttributeFinalPct.Length > i)
                                numc.Set(attr * 10 + 5,
                                    numc.GetAsInt(attr * 10 + 5) + self.AttrConfig.AttributeFinalPct[i]);
                        }
                        else
                        {
                            Log.Info("BuffConfig属性没找到 【" + self.AttrConfig.AttributeType[i]+"】");
                        }
                    }
                }
                else
                {
                    Log.Error("添加BUFF id= " + unit.Id + " 时没找到 NumericComponent 组件");
                }
            }
        }
        /// <summary>
        /// 移除BUFF属性加成
        /// </summary>
        /// <param name="self"></param>
        /// <param name="unit"></param>
        public static void RemoveBuffAttrValue(this Buff self,Unit unit)
        {
            if (self.AttrConfig.AttributeType != null)
            {
                var numc = unit.GetComponent<NumericComponent>();
                if (numc != null)
                {
                    for (int i = 0; i < self.AttrConfig.AttributeType.Length; i++)
                    {
                        if (NumericType.Map.TryGetValue(self.AttrConfig.AttributeType[i], out var attr))
                        {
                            if (self.AttrConfig.AttributeAdd != null && self.AttrConfig.AttributeAdd.Length > i)
                                numc.Set(attr * 10 + 2, numc.GetAsInt(attr * 10 + 2) - self.AttrConfig.AttributeAdd[i]);
                            if (self.AttrConfig.AttributePct != null && self.AttrConfig.AttributePct.Length > i)
                                numc.Set(attr * 10 + 3, numc.GetAsInt(attr * 10 + 3) - self.AttrConfig.AttributePct[i]);
                            if (self.AttrConfig.AttributeFinalAdd != null && self.AttrConfig.AttributeFinalAdd.Length > i)
                                numc.Set(attr * 10 + 4,
                                    numc.GetAsInt(attr * 10 + 4) - self.AttrConfig.AttributeFinalAdd[i]);
                            if (self.AttrConfig.AttributeFinalPct != null && self.AttrConfig.AttributeFinalPct.Length > i)
                                numc.Set(attr * 10 + 5,
                                    numc.GetAsInt(attr * 10 + 5) - self.AttrConfig.AttributeFinalPct[i]);
                        }
                        else
                        {
                            Log.Info("BuffConfig属性没找到 【" + self.AttrConfig.AttributeType[i]+"】");
                        }
                    }
                }
                else
                {
                    Log.Error("移除BUFF id= " + self.ConfigId + " 时没找到 NumericComponent 组件");
                }
                
            }
        }


        /// <summary>
        /// 添加行为禁制
        /// </summary>
        /// <param name="self"></param>
        /// <param name="unit"></param>
        public static void AddBuffActionControl(this Buff self, Unit unit)
        {
            var buffComp = self.GetParent<BuffComponent>();
            if (self.ActionControlConfig.ActionControl != null)
            {
                for (int i = 0; i < self.ActionControlConfig.ActionControl.Length; i++)
                {
                    var type = self.ActionControlConfig.ActionControl[i];
                    if (!buffComp.ActionControls.ContainsKey(type)||buffComp.ActionControls[type]==0)
                    {
                        buffComp.ActionControls[type] = 1;
                        // Log.Info("BuffWatcherComponent");
                        BuffWatcherComponent.Instance.SetActionControlActive(type,true,unit);
                    }
                    else
                    {
                        buffComp.ActionControls[type]++;
                    }
                }
            }
        }

        /// <summary>
        /// 移除行为禁制
        /// </summary>
        /// <param name="self"></param>
        /// <param name="unit"></param>
        public static void RemoveBuffActionControl(this Buff self, Unit unit)
        {
            var buffComp = self.GetParent<BuffComponent>();
            if (self.ActionControlConfig.ActionControl != null)
            {
                for (int i = 0; i < self.ActionControlConfig.ActionControl.Length; i++)
                {
                    var type = self.ActionControlConfig.ActionControl[i];
                    if (buffComp.ActionControls.ContainsKey(type)&&buffComp.ActionControls[type]>0)
                    {
                        buffComp.ActionControls[type]--;
                        if (buffComp.ActionControls[type] == 0)
                        {
                            // Log.Info("BuffWatcherComponent");
                            BuffWatcherComponent.Instance.SetActionControlActive(type,false,unit);
                        }
                    }
                }
            }
        }
    }
}