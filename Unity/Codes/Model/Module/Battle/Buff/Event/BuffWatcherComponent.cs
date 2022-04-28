using System;
using System.Collections.Generic;

namespace ET
{
    [ObjectSystem]
    public class BuffWatcherComponentAwakeSystem : AwakeSystem<BuffWatcherComponent>
    {
        public override void Awake(BuffWatcherComponent self)
        {
            BuffWatcherComponent.Instance = self;
            self.Awake();
        }
    }

    [ObjectSystem]
    public class BuffWatcherComponentLoadSystem : LoadSystem<BuffWatcherComponent>
    {
        public override void Load(BuffWatcherComponent self)
        {
            self.Load();
        }
    }

    /// <summary>
    /// 监视数值变化组件,分发监听
    /// </summary>
    public class BuffWatcherComponent : Entity, IAwake, ILoad
    {
        public static BuffWatcherComponent Instance { get; set; }
		
        private Dictionary<int, List<IBuffWatcher>> allWatchers;

        public void Awake()
        {
            this.Load();
        }

        public void Load()
        {
            this.allWatchers = new Dictionary<int, List<IBuffWatcher>>();

            var types = Game.EventSystem.GetTypes(typeof(BuffWatcherAttribute));
            for (int j = 0; j < types.Count; j++)
            {
                Type type = types[j];
                object[] attrs = type.GetCustomAttributes(typeof(BuffWatcherAttribute), false);

                for (int i = 0; i < attrs.Length; i++)
                {
                    BuffWatcherAttribute item = (BuffWatcherAttribute)attrs[i];
                    IBuffWatcher obj = (IBuffWatcher)Activator.CreateInstance(type);
                    var key = item.IsAdd ? item.BuffType : -item.BuffType;
                    if (!this.allWatchers.ContainsKey(key))
                    {
                        this.allWatchers.Add(key, new List<IBuffWatcher>());
                    }
                    this.allWatchers[key].Add(obj);
                }
            }
        }

        public void Run(int type,bool isAdd,Unit unit,Buff buff)
        {
            var key = isAdd ? type : -type;
            List<IBuffWatcher> list;
            if (!this.allWatchers.TryGetValue(key, out list))
            {
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                IBuffWatcher numericWatcher = list[i];
                numericWatcher.Run(unit,buff);
            }
        }
    }
}