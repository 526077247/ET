using System;
using System.Collections.Generic;

namespace ET
{
    [ObjectSystem]
    public class CommandWatcherComponentAwakeSystem : AwakeSystem<CommandWatcherComponent>
    {
        public override void Awake(CommandWatcherComponent self)
        {
            CommandWatcherComponent.Instance = self;
            self.Awake();
        }
    }

    [ObjectSystem]
    public class CommandWatcherComponentLoadSystem : LoadSystem<CommandWatcherComponent>
    {
        public override void Load(CommandWatcherComponent self)
        {
            self.Load();
        }
    }

    /// <summary>
    /// 监视数值变化组件,分发监听
    /// </summary>
    public class CommandWatcherComponent : Entity,IAwake,ILoad
    {
        public static CommandWatcherComponent Instance { get; set; }
		
        private Dictionary<string, List<ICommandWatcher>> allWatchers;

        public void Awake()
        {
            this.Load();
        }

        public void Load()
        {
            this.allWatchers = new Dictionary<string, List<ICommandWatcher>>();

            List<Type> types = Game.EventSystem.GetTypes(typeof(CommandWatcherAttribute));
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(CommandWatcherAttribute), false);

                for (int i = 0; i < attrs.Length; i++)
                {
                    CommandWatcherAttribute numericWatcherAttribute = (CommandWatcherAttribute)attrs[i];
                    ICommandWatcher obj = (ICommandWatcher)Activator.CreateInstance(type);
                    if (!this.allWatchers.ContainsKey(numericWatcherAttribute.Command))
                    {
                        this.allWatchers.Add(numericWatcherAttribute.Command, new List<ICommandWatcher>());
                    }
                    this.allWatchers[numericWatcherAttribute.Command].Add(obj);
                }
            }
        }

        public async ETTask Run(string command, GalGameEngineComponent engine, GalGameEnginePara para)
        {
            List<ICommandWatcher> list;
            if (!this.allWatchers.TryGetValue(command, out list))
            {
                return;
            }

            for (int i = 0; i < list.Count; i++)
            {
                ICommandWatcher watcher = list[i];
                await watcher.Run(engine, para);
            }
        }
    }
}
