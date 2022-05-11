using System;
using System.Collections.Generic;

namespace ET
{
    [FriendClass(typeof(BuffWatcherComponent))]
    public static class BuffWatcherComponentSystem
    {
        [ObjectSystem]
        public class BuffWatcherComponentAwakeSystem : AwakeSystem<BuffWatcherComponent>
        {
            public override void Awake(BuffWatcherComponent self)
            {
                BuffWatcherComponent.Instance = self;
                self.Init();
            }
        }

	
        public class BuffWatcherComponentLoadSystem : LoadSystem<BuffWatcherComponent>
        {
            public override void Load(BuffWatcherComponent self)
            {
                self.Init();
            }
        }

        private static void Init(this BuffWatcherComponent self)
        {
            self.allWatchers = new Dictionary<int, List<IBuffWatcher>>();

            List<Type> types = Game.EventSystem.GetTypes(typeof(BuffWatcherAttribute));
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(BuffWatcherAttribute), false);

                for (int i = 0; i < attrs.Length; i++)
                {
                    BuffWatcherAttribute item = (BuffWatcherAttribute)attrs[i];
                    IBuffWatcher obj = (IBuffWatcher)Activator.CreateInstance(type);
                    var key = item.IsAdd ? item.BuffType : -item.BuffType;
                    if (!self.allWatchers.ContainsKey(key))
                    {
                        self.allWatchers.Add(key, new List<IBuffWatcher>());
                    }
                    self.allWatchers[key].Add(obj);
                }
            }
        }

        public static void Run(this BuffWatcherComponent self, int type,bool isAdd,Unit unit)
        {
            var key = isAdd ? type : -type;
            List<IBuffWatcher> list;
            if (!self.allWatchers.TryGetValue(key, out list))
            {
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                IBuffWatcher numericWatcher = list[i];
                numericWatcher.Run(unit);
            }
        }
    }
}