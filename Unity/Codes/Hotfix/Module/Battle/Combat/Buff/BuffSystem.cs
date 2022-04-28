namespace ET
{
    [FriendClass(typeof(BuffComponent))]
    [ObjectSystem]
    public class BuffAwakeSystem : AwakeSystem<Buff,int>
    {
        public override void Awake(Buff self,int id)
        {
            Log.Info("添加BUFF id="+id);
            self.ConfigId = id;
            var buffComp = self.GetParent<BuffComponent>();
            var unit = buffComp.unit;
            if (self.Config.AttributeType != null)
            {
                var numc = unit?.GetComponent<NumericComponent>();
                if (numc != null)
                {
                    for (int i = 0; i < self.Config.AttributeType.Length; i++)
                    {
                        if (NumericType.Map.TryGetValue(self.Config.AttributeType[i], out var attr))
                        {
                            if (self.Config.AttributeAdd != null && self.Config.AttributeAdd.Length > i)
                                numc.Set(attr * 10 + 2, numc.GetAsInt(attr * 10 + 2) + self.Config.AttributeAdd[i]);
                            if (self.Config.AttributePct != null && self.Config.AttributePct.Length > i)
                                numc.Set(attr * 10 + 3, numc.GetAsInt(attr * 10 + 3) + self.Config.AttributePct[i]);
                            if (self.Config.AttributeFinalAdd != null && self.Config.AttributeFinalAdd.Length > i)
                                numc.Set(attr * 10 + 4,
                                    numc.GetAsInt(attr * 10 + 4) + self.Config.AttributeFinalAdd[i]);
                            if (self.Config.AttributeFinalPct != null && self.Config.AttributeFinalPct.Length > i)
                                numc.Set(attr * 10 + 5,
                                    numc.GetAsInt(attr * 10 + 5) + self.Config.AttributeFinalPct[i]);
                        }
                        else
                        {
                            Log.Info("BuffConfig属性没找到 【" + self.Config.AttributeType[i]+"】");
                        }
                    }
                }
                else
                {
                    Log.Error("添加BUFF id= " + id + " 时没找到 NumericComponent 组件");
                }
            }

            if (self.Config.ActionControl != null)
            {
                for (int i = 0; i < self.Config.ActionControl.Length; i++)
                {
                    var type = self.Config.ActionControl[i];
                    if (!buffComp.ActionControls.ContainsKey(type)||buffComp.ActionControls[type]==0)
                    {
                        buffComp.ActionControls[type] = 1;
                        // Log.Info("BuffWatcherComponent");
                        BuffWatcherComponent.Instance.Run(type,true,unit,self);
                    }
                    else
                    {
                        buffComp.ActionControls[type]++;
                    }
                }
            }
        }
    }

    [FriendClass(typeof(BuffComponent))]
    [ObjectSystem]
    public class BuffDestroySystem : DestroySystem<Buff>
    {
        public override void Destroy(Buff self)
        {
            Log.Info("移除BUFF id="+self.ConfigId);
            var buffComp = self.GetParent<BuffComponent>();
            var unit = buffComp.unit;
            if (self.Config.IsRemove == 0)//结束后是否移除加成（0:是）
            {
                if (self.Config.AttributeType != null)
                {
                    var numc = unit?.GetComponent<NumericComponent>();
                    if (numc != null)
                    {
                        for (int i = 0; i < self.Config.AttributeType.Length; i++)
                        {
                            if (NumericType.Map.TryGetValue(self.Config.AttributeType[i], out var attr))
                            {
                                if (self.Config.AttributeAdd != null && self.Config.AttributeAdd.Length > i)
                                    numc.Set(attr * 10 + 2, numc.GetAsInt(attr * 10 + 2) - self.Config.AttributeAdd[i]);
                                if (self.Config.AttributePct != null && self.Config.AttributePct.Length > i)
                                    numc.Set(attr * 10 + 3, numc.GetAsInt(attr * 10 + 3) - self.Config.AttributePct[i]);
                                if (self.Config.AttributeFinalAdd != null && self.Config.AttributeFinalAdd.Length > i)
                                    numc.Set(attr * 10 + 4,
                                        numc.GetAsInt(attr * 10 + 4) - self.Config.AttributeFinalAdd[i]);
                                if (self.Config.AttributeFinalPct != null && self.Config.AttributeFinalPct.Length > i)
                                    numc.Set(attr * 10 + 5,
                                        numc.GetAsInt(attr * 10 + 5) - self.Config.AttributeFinalPct[i]);
                            }
                            else
                            {
                                Log.Info("BuffConfig属性没找到 【" + self.Config.AttributeType[i]+"】");
                            }
                        }
                    }
                    else
                    {
                        Log.Error("移除BUFF id= " + self.ConfigId + " 时没找到 NumericComponent 组件");
                    }
                }
            }
            if (self.Config.ActionControl != null)
            {
                for (int i = 0; i < self.Config.ActionControl.Length; i++)
                {
                    var type = self.Config.ActionControl[i];
                    if (buffComp.ActionControls.ContainsKey(type)&&buffComp.ActionControls[type]>0)
                    {
                        buffComp.ActionControls[type]--;
                        if (buffComp.ActionControls[type] == 0)
                        {
                            // Log.Info("BuffWatcherComponent");
                            BuffWatcherComponent.Instance.Run(type,false,unit,self);
                        }
                    }
                }
            }
        }
    }

    public static class BuffSystem
    {
        
    }
}