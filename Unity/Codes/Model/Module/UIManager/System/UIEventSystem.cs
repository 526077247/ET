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
			RegisterI18N(component);
			List<object> iOnCreateSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnCreateSystem));
			if (iOnCreateSystems == null)
			{
				return;
			}

			for (int i = 0; i < iOnCreateSystems.Count; i++)
			{
				IOnCreateSystem aOnCreateSystem = (IOnCreateSystem)iOnCreateSystems[i];
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
			RegisterI18N(component);
			List<object> iOnCreateSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnCreateSystem<P1>));
			if (iOnCreateSystems == null)
			{
				return;
			}

			for (int i = 0; i < iOnCreateSystems.Count; i++)
			{
				IOnCreateSystem<P1> aOnCreateSystem = (IOnCreateSystem<P1>)iOnCreateSystems[i];
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
			RegisterI18N(component);
			List<object> iOnCreateSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnCreateSystem<P1, P2>));
			if (iOnCreateSystems == null)
			{
				return;
			}

			for (int i = 0; i < iOnCreateSystems.Count; i++)
			{
				IOnCreateSystem<P1, P2> aOnCreateSystem = (IOnCreateSystem<P1, P2>)iOnCreateSystems[i];
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
			RegisterI18N(component);
			List<object> iOnCreateSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnCreateSystem<P1, P2, P3>));
			if (iOnCreateSystems == null)
			{
				return;
			}

			for (int i = 0; i < iOnCreateSystems.Count; i++)
			{
				IOnCreateSystem<P1, P2, P3> aOnCreateSystem = (IOnCreateSystem<P1, P2, P3>)iOnCreateSystems[i];
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
			RegisterI18N(component);
			List<object> iOnCreateSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnCreateSystem<P1, P2, P3, P4>));
			if (iOnCreateSystems == null)
			{
				return;
			}

			for (int i = 0; i < iOnCreateSystems.Count; i++)
			{
				IOnCreateSystem<P1, P2, P3, P4> aOnCreateSystem = (IOnCreateSystem<P1, P2, P3, P4>)iOnCreateSystems[i];
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
				for (int i = 0; i < iOnEnableSystems.Count; i++)
				{
					IOnEnableSystem aOnEnableSystem = (IOnEnableSystem)iOnEnableSystems[i];
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
		}

		public void OnEnable<P1>(Entity component, P1 p1)
		{
			List<object> iOnEnableSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnEnableSystem<P1>));
			if (iOnEnableSystems != null)
			{
				for (int i = 0; i < iOnEnableSystems.Count; i++)
				{
					IOnEnableSystem<P1> aOnEnableSystem = (IOnEnableSystem<P1>)iOnEnableSystems[i];
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
		}

		public void OnEnable<P1, P2>(Entity component, P1 p1, P2 p2)
		{
			List<object> iOnEnableSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnEnableSystem<P1, P2>));
			if (iOnEnableSystems != null)
			{
				for (int i = 0; i < iOnEnableSystems.Count; i++)
				{
					IOnEnableSystem<P1, P2> aOnEnableSystem = (IOnEnableSystem<P1, P2>)iOnEnableSystems[i];
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
		}

		public void OnEnable<P1, P2, P3>(Entity component, P1 p1, P2 p2, P3 p3)
		{
			List<object> iOnEnableSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnEnableSystem<P1, P2, P3>));
			if (iOnEnableSystems != null)
			{
				for (int i = 0; i < iOnEnableSystems.Count; i++)
				{
					IOnEnableSystem<P1, P2, P3> aOnEnableSystem = (IOnEnableSystem<P1, P2, P3>)iOnEnableSystems[i];
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
		}

		public void OnEnable<P1, P2, P3, P4>(Entity component, P1 p1, P2 p2, P3 p3, P4 p4)
		{
			List<object> iOnEnableSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnEnableSystem<P1, P2, P3, P4>));
			if (iOnEnableSystems != null)
			{
				for (int i = 0; i < iOnEnableSystems.Count; i++)
				{
					IOnEnableSystem<P1, P2, P3, P4> aOnEnableSystem = (IOnEnableSystem<P1, P2, P3, P4>)iOnEnableSystems[i];
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
		}

		#endregion

		#region OnDisable
		public void OnDisable(Entity component)
		{
			List<object> iOnDisableSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnDisableSystem));
			if (iOnDisableSystems == null)
			{
				return;
			}

			for (int i = 0; i < iOnDisableSystems.Count; i++)
			{
				IOnDisableSystem aOnDisableSystem = (IOnDisableSystem)iOnDisableSystems[i];
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
			List<object> iOnDisableSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnDisableSystem<P1>));
			if (iOnDisableSystems == null)
			{
				return;
			}

			for (int i = 0; i < iOnDisableSystems.Count; i++)
			{
				IOnDisableSystem<P1> aOnDisableSystem = (IOnDisableSystem<P1>)iOnDisableSystems[i];
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
			List<object> iOnDisableSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnDisableSystem<P1, P2>));
			if (iOnDisableSystems == null)
			{
				return;
			}

			for (int i = 0; i < iOnDisableSystems.Count; i++)
			{
				IOnDisableSystem<P1, P2> aOnDisableSystem = (IOnDisableSystem<P1, P2>)iOnDisableSystems[i];
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
			List<object> iOnDisableSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnDisableSystem<P1, P2, P3>));
			if (iOnDisableSystems == null)
			{
				return;
			}

			for (int i = 0; i < iOnDisableSystems.Count; i++)
			{
				IOnDisableSystem<P1, P2, P3> aOnDisableSystem = (IOnDisableSystem<P1, P2, P3>)iOnDisableSystems[i];
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
			List<object> iOnDisableSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnDisableSystem<P1, P2, P3, P4>));
			if (iOnDisableSystems == null)
			{
				return;
			}

			for (int i = 0; i < iOnDisableSystems.Count; i++)
			{
				IOnDisableSystem<P1, P2, P3, P4> aOnDisableSystem = (IOnDisableSystem<P1, P2, P3, P4>)iOnDisableSystems[i];
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
			RemoveI18N(component);
			List<object> iOnDestroySystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnDestroySystem));
			if (iOnDestroySystems == null)
			{
				return;
			}

			for (int i = 0; i < iOnDestroySystems.Count; i++)
			{
				IOnDestroySystem aOnDestroySystem = (IOnDestroySystem)iOnDestroySystems[i];
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
		
		#region I18N

		private Dictionary<Type, bool> I18NCheckRes = new Dictionary<Type, bool>();

		public void RegisterI18N(Entity component)
		{
			if (CheckIsI18N(component))
			{
				EventSystem.Instance.Publish(new UIEventType.RegisterI18NEntity() {entity = component});
			}
		}
		public void RemoveI18N(Entity component)
		{
			if (CheckIsI18N(component))
			{
				EventSystem.Instance.Publish(new UIEventType.RemoveI18NEntity {entity = component});
			}
		}
		public bool CheckIsI18N(Entity component)
		{
			var type = component.GetType();
			if (I18NCheckRes.ContainsKey(type)) return I18NCheckRes[type];
			if (!(component is II18N))
			{
				I18NCheckRes[type] = false;
				return false;
			}
			List<object> iI18NSystems = this.typeSystems.GetSystems(type, typeof(II18NSystem));
			if (iI18NSystems == null)
			{
				I18NCheckRes[type] = false;
				return false;
			}
			for (int i = 0; i < iI18NSystems.Count; i++)
			{
				II18NSystem aI18NSystem = (II18NSystem)iI18NSystems[i];
				if (aI18NSystem != null)
				{
					I18NCheckRes[type] = true;
					return true;
				}
			}
			I18NCheckRes[type] = false;
			return false;
		}
		public void OnLanguageChange(Entity component)
		{
			List<object> iI18NSystems = this.typeSystems.GetSystems(component.GetType(), typeof(II18NSystem));
			if (iI18NSystems == null)
			{
				return;
			}

			for (int i = 0; i < iI18NSystems.Count; i++)
			{
				II18NSystem aI18NSystem = (II18NSystem)iI18NSystems[i];
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
		#endregion
		public async ETTask OnViewInitializationSystem(Entity component)
		{
			List<object> iOnViewInitializationSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnViewInitializationSystem));
			if (iOnViewInitializationSystems == null)
			{
				return;
			}

			for (int i = 0; i < iOnViewInitializationSystems.Count; i++)
			{
				IOnViewInitializationSystem aOnViewInitializationSystem = (IOnViewInitializationSystem)iOnViewInitializationSystems[i];
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
		
		public void OnChangeRedDotActive(Entity component,int count)
		{
			List<object> iRedDotSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IRedDotSystem));
			if (iRedDotSystems == null)
			{
				return;
			}

			for (int i = 0; i < iRedDotSystems.Count; i++)
			{
				IRedDotSystem aRedDotSystem = (IRedDotSystem)iRedDotSystems[i];
				if (aRedDotSystem == null)
				{
					continue;
				}

				try
				{
					aRedDotSystem.Run(component,count);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}
		
		public bool OnWidthPaddingChange(IOnWidthPaddingChange component)
		{
			
			List<object> iOnWidthPaddingChangeSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IOnWidthPaddingChangeSystem));
			if (iOnWidthPaddingChangeSystems == null)
			{
				return false;
			}
			bool res = false;
			for (int i = 0; i < iOnWidthPaddingChangeSystems.Count; i++)
			{
				IOnWidthPaddingChangeSystem aOnWidthPaddingChangeSystem = (IOnWidthPaddingChangeSystem)iOnWidthPaddingChangeSystems[i];
				if (aOnWidthPaddingChangeSystem == null)
				{
					continue;
				}

				try
				{
					res = true;
					aOnWidthPaddingChangeSystem.Run(component);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}

			return res;
		}
	}
}
