using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
	using OneTypeSystems = UnOrderMultiMap<Type, object>;
	public sealed class UIEventSystem
    {
		public static UIEventSystem Instance;
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
			foreach (Type type in EventSystem.Instance.GetTypes(typeof(UISystemAttribute)))
			{
				object obj = Activator.CreateInstance(type);

				if (obj is ISystemType iSystemType)
				{
					OneTypeSystems oneTypeSystems = this.typeSystems.GetOrCreateOneTypeSystems(iSystemType.Type());
					oneTypeSystems.Add(iSystemType.SystemType(), obj);
				}
			}
		}
		#region OnCreate
		public void OnCreate(Entity component)
		{
			List<object> iOnCreateSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnCreateSystem));
			if (iOnCreateSystems == null)
			{
				return;
			}

			foreach (IOnCreateSystem aOnCreateSystem in iOnCreateSystems)
			{
				if (aOnCreateSystem == null)
				{
					continue;
				}

				try
				{
					aOnCreateSystem.Run(component);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}

		public void OnCreate<P1>(Entity component, P1 p1)
		{
			List<object> iOnCreateSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnCreateSystem<P1>));
			if (iOnCreateSystems == null)
			{
				return;
			}

			foreach (IOnCreateSystem<P1> aOnCreateSystem in iOnCreateSystems)
			{
				if (aOnCreateSystem == null)
				{
					continue;
				}

				try
				{
					aOnCreateSystem.Run(component, p1);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}

		public void OnCreate<P1, P2>(Entity component, P1 p1, P2 p2)
		{
			List<object> iOnCreateSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnCreateSystem<P1, P2>));
			if (iOnCreateSystems == null)
			{
				return;
			}

			foreach (IOnCreateSystem<P1, P2> aOnCreateSystem in iOnCreateSystems)
			{
				if (aOnCreateSystem == null)
				{
					continue;
				}

				try
				{
					aOnCreateSystem.Run(component, p1, p2);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}

		public void OnCreate<P1, P2, P3>(Entity component, P1 p1, P2 p2, P3 p3)
		{
			List<object> iOnCreateSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnCreateSystem<P1, P2, P3>));
			if (iOnCreateSystems == null)
			{
				return;
			}

			foreach (IOnCreateSystem<P1, P2, P3> aOnCreateSystem in iOnCreateSystems)
			{
				if (aOnCreateSystem == null)
				{
					continue;
				}

				try
				{
					aOnCreateSystem.Run(component, p1, p2, p3);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}

		public void OnCreate<P1, P2, P3, P4>(Entity component, P1 p1, P2 p2, P3 p3, P4 p4)
		{
			List<object> iOnCreateSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnCreateSystem<P1, P2, P3, P4>));
			if (iOnCreateSystems == null)
			{
				return;
			}

			foreach (IOnCreateSystem<P1, P2, P3, P4> aOnCreateSystem in iOnCreateSystems)
			{
				if (aOnCreateSystem == null)
				{
					continue;
				}

				try
				{
					aOnCreateSystem.Run(component, p1, p2, p3, p4);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}

		#endregion

		#region OnEnable
		public void OnEnable(Entity component)
		{
			
			List<object> iOnEnableSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnEnableSystem));
			if (iOnEnableSystems != null)
			{
				foreach (IOnEnableSystem aOnEnableSystem in iOnEnableSystems)
				{
					if (aOnEnableSystem == null)
					{
						continue;
					}

					try
					{
						aOnEnableSystem.Run(component);
					}
					catch (Exception e)
					{
						Log.Error(e);
					}
				}
			}
			(component as UIBaseContainer).AfterOnEnable();

		}

		public void OnEnable<P1>(Entity component, P1 p1)
		{
			List<object> iOnEnableSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnEnableSystem<P1>));
			if (iOnEnableSystems != null)
			{
				foreach (IOnEnableSystem<P1> aOnEnableSystem in iOnEnableSystems)
				{
					if (aOnEnableSystem == null)
					{
						continue;
					}

					try
					{
						aOnEnableSystem.Run(component, p1);
					}
					catch (Exception e)
					{
						Log.Error(e);
					}
				}
			}

			
			(component as UIBaseContainer).AfterOnEnable();
		}

		public void OnEnable<P1, P2>(Entity component, P1 p1, P2 p2)
		{
			List<object> iOnEnableSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnEnableSystem<P1, P2>));
			if (iOnEnableSystems != null)
			{
				foreach (IOnEnableSystem<P1, P2> aOnEnableSystem in iOnEnableSystems)
				{
					if (aOnEnableSystem == null)
					{
						continue;
					}

					try
					{
						aOnEnableSystem.Run(component, p1, p2);
					}
					catch (Exception e)
					{
						Log.Error(e);
					}
				}
			}

			
			(component as UIBaseContainer).AfterOnEnable();
		}

		public void OnEnable<P1, P2, P3>(Entity component, P1 p1, P2 p2, P3 p3)
		{
			List<object> iOnEnableSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnEnableSystem<P1, P2, P3>));
			if (iOnEnableSystems != null)
			{
				foreach (IOnEnableSystem<P1, P2, P3> aOnEnableSystem in iOnEnableSystems)
				{
					if (aOnEnableSystem == null)
					{
						continue;
					}

					try
					{
						aOnEnableSystem.Run(component, p1, p2, p3);
					}
					catch (Exception e)
					{
						Log.Error(e);
					}
				}
			}

			
			(component as UIBaseContainer).AfterOnEnable();
		}

		public void OnEnable<P1, P2, P3, P4>(Entity component, P1 p1, P2 p2, P3 p3, P4 p4)
		{
			List<object> iOnEnableSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnEnableSystem<P1, P2, P3, P4>));
			if (iOnEnableSystems != null)
			{
				foreach (IOnEnableSystem<P1, P2, P3, P4> aOnEnableSystem in iOnEnableSystems)
				{
					if (aOnEnableSystem == null)
					{
						continue;
					}

					try
					{
						aOnEnableSystem.Run(component, p1, p2, p3, p4);
					}
					catch (Exception e)
					{
						Log.Error(e);
					}
				}
			}

			
			(component as UIBaseContainer).AfterOnEnable();
		}

		#endregion

		#region OnDisable
		public void OnDisable(Entity component)
		{
			(component as UIBaseContainer).BeforeOnDisable();
			List<object> iOnDisableSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnDisableSystem));
			if (iOnDisableSystems == null)
			{
				return;
			}

			foreach (IOnDisableSystem aOnDisableSystem in iOnDisableSystems)
			{
				if (aOnDisableSystem == null)
				{
					continue;
				}

				try
				{
					aOnDisableSystem.Run(component);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}

		public void OnDisable<P1>(Entity component, P1 p1)
		{
			(component as UIBaseContainer).BeforeOnDisable();
			List<object> iOnDisableSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnDisableSystem<P1>));
			if (iOnDisableSystems == null)
			{
				return;
			}

			foreach (IOnDisableSystem<P1> aOnDisableSystem in iOnDisableSystems)
			{
				if (aOnDisableSystem == null)
				{
					continue;
				}

				try
				{
					aOnDisableSystem.Run(component, p1);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}

		public void OnDisable<P1, P2>(Entity component, P1 p1, P2 p2)
		{
			(component as UIBaseContainer).BeforeOnDisable();
			List<object> iOnDisableSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnDisableSystem<P1, P2>));
			if (iOnDisableSystems == null)
			{
				return;
			}

			foreach (IOnDisableSystem<P1, P2> aOnDisableSystem in iOnDisableSystems)
			{
				if (aOnDisableSystem == null)
				{
					continue;
				}

				try
				{
					aOnDisableSystem.Run(component, p1, p2);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}

		public void OnDisable<P1, P2, P3>(Entity component, P1 p1, P2 p2, P3 p3)
		{
			(component as UIBaseContainer).BeforeOnDisable();
			List<object> iOnDisableSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnDisableSystem<P1, P2, P3>));
			if (iOnDisableSystems == null)
			{
				return;
			}

			foreach (IOnDisableSystem<P1, P2, P3> aOnDisableSystem in iOnDisableSystems)
			{
				if (aOnDisableSystem == null)
				{
					continue;
				}

				try
				{
					aOnDisableSystem.Run(component, p1, p2, p3);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}

		public void OnDisable<P1, P2, P3, P4>(Entity component, P1 p1, P2 p2, P3 p3, P4 p4)
		{
			(component as UIBaseContainer).BeforeOnDisable();
			List<object> iOnDisableSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnDisableSystem<P1, P2, P3, P4>));
			if (iOnDisableSystems == null)
			{
				return;
			}

			foreach (IOnDisableSystem<P1, P2, P3, P4> aOnDisableSystem in iOnDisableSystems)
			{
				if (aOnDisableSystem == null)
				{
					continue;
				}

				try
				{
					aOnDisableSystem.Run(component, p1, p2, p3, p4);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}

		#endregion

		#region OnDestroy
		public void OnDestroy(Entity component)
		{
			(component as UIBaseContainer).BeforeOnDestroy();
			List<object> iOnDestroySystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnDestroySystem));
			if (iOnDestroySystems == null)
			{
				return;
			}

			foreach (IOnDestroySystem aOnDestroySystem in iOnDestroySystems)
			{
				if (aOnDestroySystem == null)
				{
					continue;
				}

				try
				{
					aOnDestroySystem.Run(component);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}

		#endregion

		public void OnLanguageChange(Entity component)
		{
			List<object> iI18NSystems = this.typeSystems.GetSystems(component.GetType(), typeof(II18NSystem));
			if (iI18NSystems == null)
			{
				return;
			}

			foreach (II18NSystem aI18NSystem in iI18NSystems)
			{
				if (aI18NSystem == null)
				{
					continue;
				}

				try
				{
					aI18NSystem.Run(component);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}
		
		public async ETTask OnViewInitializationSystem(Entity component)
		{
			List<object> iOnViewInitializationSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnViewInitializationSystem));
			if (iOnViewInitializationSystems == null)
			{
				return;
			}

			foreach (IOnViewInitializationSystem aOnViewInitializationSystem in iOnViewInitializationSystems)
			{
				if (aOnViewInitializationSystem == null)
				{
					continue;
				}

				try
				{
					await aOnViewInitializationSystem.Run(component);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}
	}
}
