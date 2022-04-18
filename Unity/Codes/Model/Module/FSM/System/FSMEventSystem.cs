using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
	using OneTypeSystems = UnOrderMultiMap<Type, object>;
	public sealed class FSMEventSystem
	{
		public static FSMEventSystem Instance;
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

		private TypeSystems typeSystems = new TypeSystems();
		public void Awake()
		{
			foreach (Type type in EventSystem.Instance.GetTypes(typeof(FSMSystemAttribute)))
			{
				object obj = Activator.CreateInstance(type);

				if (obj is ISystemType iSystemType)
				{
					OneTypeSystems oneTypeSystems = this.typeSystems.GetOrCreateOneTypeSystems(iSystemType.Type());
					oneTypeSystems.Add(iSystemType.SystemType(), obj);
				}
			}
		}
		#region FSMOnEnter
		public async ETTask FSMOnEnter(Entity component)
		{
			List<object> iFSMOnEnterSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IFSMOnEnterSystem));
			if (iFSMOnEnterSystems == null)
			{
				return;
			}

			for (int i = 0; i < iFSMOnEnterSystems.Count; i++)
			{
				IFSMOnEnterSystem aFSMOnEnterSystem = (IFSMOnEnterSystem)iFSMOnEnterSystems[i];
				if (aFSMOnEnterSystem == null)
				{
					continue;
				}

				try
				{
					await aFSMOnEnterSystem.Run(component);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}

		public async ETTask FSMOnEnter<P1>(Entity component, P1 p1)
		{
			List<object> iFSMOnEnterSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IFSMOnEnterSystem<P1>));
			if (iFSMOnEnterSystems == null)
			{
				return;
			}

			for (int i = 0; i < iFSMOnEnterSystems.Count; i++)
			{
				IFSMOnEnterSystem<P1> aFSMOnEnterSystem = (IFSMOnEnterSystem<P1>)iFSMOnEnterSystems[i];
				if (aFSMOnEnterSystem == null)
				{
					continue;
				}

				try
				{
					await aFSMOnEnterSystem.Run(component, p1);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}

		public async ETTask FSMOnEnter<P1, P2>(Entity component, P1 p1, P2 p2)
		{
			List<object> iFSMOnEnterSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IFSMOnEnterSystem<P1, P2>));
			if (iFSMOnEnterSystems == null)
			{
				return;
			}

			for (int i = 0; i < iFSMOnEnterSystems.Count; i++)
			{
				IFSMOnEnterSystem<P1, P2> aFSMOnEnterSystem = (IFSMOnEnterSystem<P1, P2>)iFSMOnEnterSystems[i];
				if (aFSMOnEnterSystem == null)
				{
					continue;
				}

				try
				{
					await aFSMOnEnterSystem.Run(component, p1, p2);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}

		public async ETTask FSMOnEnter<P1, P2, P3>(Entity component, P1 p1, P2 p2, P3 p3)
		{
			List<object> iFSMOnEnterSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IFSMOnEnterSystem<P1, P2, P3>));
			if (iFSMOnEnterSystems == null)
			{
				return;
			}

			for (int i = 0; i < iFSMOnEnterSystems.Count; i++)
			{
				IFSMOnEnterSystem<P1, P2, P3> aFSMOnEnterSystem = (IFSMOnEnterSystem<P1, P2, P3>)iFSMOnEnterSystems[i];
				if (aFSMOnEnterSystem == null)
				{
					continue;
				}

				try
				{
					await aFSMOnEnterSystem.Run(component, p1, p2, p3);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}

		public async ETTask FSMOnEnter<P1, P2, P3, P4>(Entity component, P1 p1, P2 p2, P3 p3, P4 p4)
		{
			List<object> iFSMOnEnterSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IFSMOnEnterSystem<P1, P2, P3, P4>));
			if (iFSMOnEnterSystems == null)
			{
				return;
			}

			for (int i = 0; i < iFSMOnEnterSystems.Count; i++)
			{
				IFSMOnEnterSystem<P1, P2, P3, P4> aFSMOnEnterSystem = (IFSMOnEnterSystem<P1, P2, P3, P4>)iFSMOnEnterSystems[i];
				if (aFSMOnEnterSystem == null)
				{
					continue;
				}

				try
				{
					await aFSMOnEnterSystem.Run(component, p1, p2, p3, p4);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}

		#endregion

		#region FSMOnExit
		public async ETTask FSMOnExit(Entity component)
		{

			List<object> iFSMOnExitSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IFSMOnExitSystem));
			if (iFSMOnExitSystems != null)
			{
				for (int i = 0; i < iFSMOnExitSystems.Count; i++)
				{
					IFSMOnExitSystem aFSMOnExitSystem = (IFSMOnExitSystem)iFSMOnExitSystems[i];
					if (aFSMOnExitSystem == null)
					{
						continue;
					}

					try
					{
						await aFSMOnExitSystem.Run(component);
					}
					catch (Exception e)
					{
						Log.Error(e);
					}
				}
			}
		}

		#endregion


	}
}
