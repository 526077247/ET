using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ET
{
	public class ConfigAwakeSystem : AwakeSystem<ConfigComponent>
	{
		public override void Awake(ConfigComponent self)
		{
			ConfigComponent.Instance = self;
		}
	}

	public class ConfigDestroySystem : DestroySystem<ConfigComponent>
	{
		public override void Destroy(ConfigComponent self)
		{
			ConfigComponent.Instance = null;
		}
	}

	public static class ConfigComponentSystem
	{
		public static void LoadOneConfig(this ConfigComponent self, Type configType)
		{
			byte[] oneConfigBytes = ConfigComponent.GetOneConfigBytes(configType.FullName);

			object category = ProtobufHelper.FromBytes(configType, oneConfigBytes, 0, oneConfigBytes.Length);

			self.AllConfig[configType] = category;
		}

		public static async ETTask LoadAsync(this ConfigComponent self)
		{
			self.AllConfig.Clear();
			HashSet<Type> types = Game.EventSystem.GetTypes(typeof(ConfigAttribute));
			Dictionary<string, byte[]> configBytes = ConfigComponent.GetAllConfigBytes;

			foreach (Type type in types)
			{
				self.LoadOneInThread(type, configBytes);
			}
			await ETTask.CompletedTask;
		}

		private static void LoadOneInThread(this ConfigComponent self, Type configType, Dictionary<string, byte[]> configBytes)
		{
			if (!configBytes.TryGetValue(configType.Name, out byte[] oneConfigBytes))
			{
				Log.Error("Config Not Found, Key: " + configType.Name);
				return;
			}

			object category = ProtobufHelper.FromBytes(configType, oneConfigBytes, 0, oneConfigBytes.Length);

			lock (self)
			{
				self.AllConfig[configType] = category;
			}
		}
	}
}