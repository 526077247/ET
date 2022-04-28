using System;
using System.Collections.Generic;
namespace ET
{
	using OneTypeSystems = UnOrderMultiMap<Type, object>;

	public sealed class SelectEventSystem
	{
		private static SelectEventSystem __Instance;
		public static SelectEventSystem Instance
		{
			get
			{
				if (__Instance == null)
				{
					__Instance = new SelectEventSystem();
				}

				return __Instance;
			}
		}
		
		private SelectEventSystem()
		{
			typeSystems = new TypeSystems();
			foreach (Type type in EventSystem.Instance.GetTypes(typeof(SelectSystemAttribute)))
			{
				object obj = Activator.CreateInstance(type);
				if (obj is ISystemType iSystemType)
				{
					OneTypeSystems oneTypeSystems = this.typeSystems.GetOrCreateOneTypeSystems(iSystemType.Type());
					oneTypeSystems.Add(iSystemType.SystemType(), obj);
				}
			}
		}

		private class TypeSystems
		{
			private readonly Dictionary<Type, OneTypeSystems> typeSystemsMap = new Dictionary<Type, OneTypeSystems>();

			public OneTypeSystems GetOrCreateOneTypeSystems(Type type)
			{
				OneTypeSystems systems = null;
				this.typeSystemsMap.TryGetValue(type, out systems);
				if (systems != null)
				{
					return systems;
				}

				systems = new OneTypeSystems();
				this.typeSystemsMap.Add(type, systems);
				return systems;
			}

			public OneTypeSystems GetOneTypeSystems(Type type)
			{
				OneTypeSystems systems = null;
				this.typeSystemsMap.TryGetValue(type, out systems);
				return systems;
			}

			public List<object> GetSystems(Type type, Type systemType)
			{
				OneTypeSystems oneTypeSystems = null;
				if (!this.typeSystemsMap.TryGetValue(type, out oneTypeSystems))
				{
					return null;
				}

				if (!oneTypeSystems.TryGetValue(systemType, out List<object> systems))
				{
					return null;
				}

				return systems;
			}
		}

		private TypeSystems typeSystems;

		public async ETTask Show(Entity component)
		{
			List<object> iShowSelectSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IShowSelectSystem));
			if (iShowSelectSystems == null)
			{
				return;
			}

			for (int i = 0; i < iShowSelectSystems.Count; i++)
			{
				IShowSelectSystem aShowSelectSystem = (IShowSelectSystem)iShowSelectSystems[i];
				if (aShowSelectSystem == null)
				{
					continue;
				}

				try
				{
					await aShowSelectSystem.Show(component);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}
		
		public async ETTask Show<T>(Entity component,T t)
		{
			List<object> iShowSelectSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IShowSelectSystem<T>));
			if (iShowSelectSystems == null)
			{
				return;
			}

			for (int i = 0; i < iShowSelectSystems.Count; i++)
			{
				IShowSelectSystem<T> aShowSelectSystem = (IShowSelectSystem<T>)iShowSelectSystems[i];
				if (aShowSelectSystem == null)
				{
					continue;
				}

				try
				{
					await aShowSelectSystem.Show(component,t);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}
		
		public async ETTask Show<T,V>(Entity component,T t,V v)
		{
			List<object> iShowSelectSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IShowSelectSystem<T,V>));
			if (iShowSelectSystems == null)
			{
				return;
			}

			for (int i = 0; i < iShowSelectSystems.Count; i++)
			{
				IShowSelectSystem<T,V> aShowSelectSystem = (IShowSelectSystem<T,V>)iShowSelectSystems[i];
				if (aShowSelectSystem == null)
				{
					continue;
				}

				try
				{
					await aShowSelectSystem.Show(component,t,v);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}
		
		public void Hide(Entity component)
		{
			List<object> iShowSelectSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IHideSelectSystem));
			if (iShowSelectSystems == null)
			{
				return;
			}

			for (int i = 0; i < iShowSelectSystems.Count; i++)
			{
				IHideSelectSystem aShowSelectSystem = (IHideSelectSystem)iShowSelectSystems[i];
				if (aShowSelectSystem == null)
				{
					continue;
				}

				try
				{
					aShowSelectSystem.Hide(component);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}
	}
}