using UnityEngine;

namespace ET
{
    [FriendClass(typeof(GuidanceComponent))]
    public static class GuidanceComponentSystem
    {
        public class AwakeSystem:AwakeSystem<GuidanceComponent>
        {
            public override void Awake(GuidanceComponent self)
            {
                GuidanceComponent.Instance = self;
                self.CheckGroupStart();
            }
        }

        /// <summary>
        /// 检查是否有可开启引导，进游戏检查一次，登录检查一次，完成一个引导检查一次
        /// </summary>
        /// <param name="self"></param>
        public static void CheckGroupStart(this GuidanceComponent self)
        {
            if(self.Group!=0) return;
            for (int i = 0; i < GuidanceConfigCategory.Instance.GetAllGroupList().Count; i++)
            {
                var item = GuidanceConfigCategory.Instance.GetAllGroupList()[i];
                var val = 0;
                if (item.Share != 0)
                {
                    val = PlayerPrefs.GetInt(CacheKeys.Guidance + "_" + item.Group, 0);
                }
                else if(!string.IsNullOrEmpty(GlobalComponent.Instance.Account))
                {
                    val = PlayerPrefs.GetInt(CacheKeys.Guidance+"_"+item.Group+"_"+GlobalComponent.Instance.Account,0);
                }

                if (val == 0)
                {
                    self.StartGuide(item.Group);
                    return;
                }
            }
        }
        
        
        /// <summary>
        /// 开始引导 todo:登录后获取服务端数据
        /// </summary>
        /// <param name="self"></param>
        /// <param name="group"></param>
        public static void StartGuide(this GuidanceComponent self,int group)
        {
            if(self.Group==group) return;
            if (self.Group != 0)
            {
                if (self.Config.Grouporder < GuidanceConfigCategory.Instance.GetGroup(group).Grouporder)
                {
                    return;
                }
            }

            if (GuidanceConfigCategory.Instance.GetGroup(group) == null)
            {
                Log.Error("引导不存在 "+group);
                return;
            }
            Log.Info("开启引导 "+group);
            self.Group = group;
            for (int i = self.Config.Steps.Count-1; i >=0 ; i--)
            {
                if (self.CheckStepCanRunning(self.Config.Steps[i].Id))
                {
                    self.RunStep(i);
                    return;
                }
            }
        }

        /// <summary>
        /// 完成一个事件
        /// </summary>
        /// <param name="self"></param>
        /// <param name="evt"></param>
        public static void NoticeEvent(this GuidanceComponent self, string evt)
        {
            if (self.StepConfig != null && self.StepConfig.Event == evt)
            {
                self.OnStepOver(self.StepConfig.Id);
            }
        }
        
        /// <summary>
        /// 检查是否可以执行次步骤
        /// </summary>
        /// <param name="self"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private static bool CheckStepCanRunning(this GuidanceComponent self, int id)
        {
            var step = GuidanceConfigCategory.Instance.Get(id);
            if (step.Steptype == GuidanceStepType.UIRouter)
            {
                if (UIManagerComponent.Instance.GetWindow(step.Value1, 1) != null)
                {
                    return false;
                }
                return true;
            }
            if (step.Steptype == GuidanceStepType.FocuGameObejct)
            {
                if (UIManagerComponent.Instance.GetWindow(step.Value1, 1) != null)
                {
                    return true;
                }
            }
            //todo: othertype
            else
            {
                Log.Error("未处理的类型 Steptype="+step.Steptype);
            }
            return false;
        }
        
        /// <summary>
        /// 完成一个步骤
        /// </summary>
        /// <param name="self"></param>
        /// <param name="id"></param>
        private static void OnStepOver(this GuidanceComponent self, int id)
        {
            if (self.StepConfig != null && self.StepConfig.Id == id)
            {
                if (self.StepConfig.KeyStep == 1)//关键步骤
                {
                    if (self.Config.Share != 0)
                    {
                        PlayerPrefs.SetInt(CacheKeys.Guidance+"_"+self.Config.Group+"_"+GlobalComponent.Instance.Account,1);
                    }
                    else
                    {
                        PlayerPrefs.SetInt(CacheKeys.Guidance+"_"+self.Config.Group,1);
                    }
                    PlayerPrefs.Save();
                }
                var index = self.CurIndex+1;
                if (index >= self.Config.Steps.Count)
                {
                    self.Stop();
                    return;
                }

                self.RunStep(index);
            }
        }

        /// <summary>
        /// 进行一个步骤
        /// </summary>
        /// <param name="self"></param>
        /// <param name="index"></param>
        private static void RunStep(this GuidanceComponent self,int index)
        {
            if (self.StepConfig != null && index!=self.CurIndex)
            {
                self.CurIndex = index;
                if (self.StepConfig.Steptype == GuidanceStepType.UIRouter)
                {
                    
                }
                else if (self.StepConfig.Steptype == GuidanceStepType.FocuGameObejct)
                {
                    var win = UIManagerComponent.Instance.GetWindow(self.StepConfig.Value1, 1);
                    if (win != null)
                    {
                        EventSystem.Instance.Publish(new UIEventType.FocuGameObejct()
                        {
                            Win = win,
                            Path = self.StepConfig.Value2,
                            Type = int.Parse(self.StepConfig.Value3)
                        });
                        return;
                    }
                }
                EventSystem.Instance.Publish(new UIEventType.FocuGameObejct());
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="self"></param>
        private static void Stop(this GuidanceComponent self)
        {
            Log.Info(self.Group + "  引导完成");
            self.CurIndex = -1;
            self.Group = 0;
            self.CheckGroupStart();
        }
    }
}