using System;
using System.Collections.Generic;

namespace ET
{
	[ObjectSystem]
	public class NumericWatcherComponentAwakeSystem : AwakeSystem<NumericWatcherComponent>
	{
		public override void Awake(NumericWatcherComponent self)
		{
			NumericWatcherComponent.Instance = self;
			self.Awake();
		}
	}

	[ObjectSystem]
	public class NumericWatcherComponentLoadSystem : LoadSystem<NumericWatcherComponent>
	{
		public override void Load(NumericWatcherComponent self)
		{
			self.Load();
		}
	}

	/// <summary>
	/// 监视数值变化组件,分发监听
	/// </summary>
	public class NumericWatcherComponent : Entity, IAwake, ILoad
	{
		public static NumericWatcherComponent Instance { get; set; }
		
		public Dictionary<int, List<INumericWatcher>> allWatchers;
	}
}