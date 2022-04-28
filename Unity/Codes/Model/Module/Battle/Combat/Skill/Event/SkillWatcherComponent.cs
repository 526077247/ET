using System;
using System.Collections.Generic;

namespace ET
{
    [ObjectSystem]
    public class SkillWatcherComponentAwakeSystem : AwakeSystem<SkillWatcherComponent>
    {
        public override void Awake(SkillWatcherComponent self)
        {
            SkillWatcherComponent.Instance = self;
            self.Awake();
        }
    }

    [ObjectSystem]
    public class SkillWatcherComponentLoadSystem : LoadSystem<SkillWatcherComponent>
    {
        public override void Load(SkillWatcherComponent self)
        {
            self.Load();
        }
    }

    /// <summary>
    /// 监视数值变化组件,分发监听
    /// </summary>
    public class SkillWatcherComponent : Entity, IAwake, ILoad
    {
        public static SkillWatcherComponent Instance { get; set; }
		
        private Dictionary<int, List<ISkillWatcher>> allWatchers;

        public void Awake()
        {
            this.Load();
        }

        public void Load()
        {
            this.allWatchers = new Dictionary<int, List<ISkillWatcher>>();

            var types = Game.EventSystem.GetTypes(typeof(SkillWatcherAttribute));
            for (int j = 0; j < types.Count; j++)
            {
                Type type = types[j];
                object[] attrs = type.GetCustomAttributes(typeof(SkillWatcherAttribute), false);

                for (int i = 0; i < attrs.Length; i++)
                {
                    SkillWatcherAttribute numericWatcherAttribute = (SkillWatcherAttribute)attrs[i];
                    ISkillWatcher obj = (ISkillWatcher)Activator.CreateInstance(type);
                    if (!this.allWatchers.ContainsKey(numericWatcherAttribute.SkillStepType))
                    {
                        this.allWatchers.Add(numericWatcherAttribute.SkillStepType, new List<ISkillWatcher>());
                    }
                    this.allWatchers[numericWatcherAttribute.SkillStepType].Add(obj);
                }
            }
        }

        public void Run(int type,SkillPara para)
        {
            List<ISkillWatcher> list;
            if (!this.allWatchers.TryGetValue(type, out list))
            {
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                ISkillWatcher numericWatcher = list[i];
                numericWatcher.Run(para);
            }
        }
    }
}