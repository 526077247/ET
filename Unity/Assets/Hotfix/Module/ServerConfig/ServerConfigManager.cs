using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{

	[ObjectSystem]
	public class ServerConfigManagerComponentAwakeSystem : AwakeSystem<ServerConfigManagerComponent>
	{
		public override void Awake(ServerConfigManagerComponent self)
		{
			self.Awake();
		}
	}
	public class ServerConfigManagerComponent: Entity
    {
		readonly string ServerKey = "Env";
		ServerConfig cur_config;
		public static ServerConfigManagerComponent Instance;
		public void Awake()
		{
			Instance = this;
			ServerConfig.config.TryGetValue(PlayerPrefs.GetString(ServerKey, ServerConfig.default_key),out cur_config);
            if (cur_config == null)
            {
				ServerConfig.config.TryGetValue(ServerConfig.default_key, out cur_config);
			}
		}

		public ServerConfig GetCurConfig()
        {
			return cur_config;

		}

		public ServerConfig ChangeEnv(string name)
        {
			if(ServerConfig.config.TryGetValue(name,out var conf))
            {
				cur_config = conf;
				PlayerPrefs.SetString(ServerKey, name);
			}
			return cur_config;

		}
		//获取正式环境更新列表cdn地址
		public string GetUpdateListCdnUrl()
        {
			return cur_config.update_list_cdn_url;
		}

		// 获取客户端资源cdn地址
		public string GetResCdnUrl()
		{
			return cur_config.res_cdn_url;
		}
		//获取测试环境更新列表cdn地址
		public string GetTestUpdateListCdnUrl()
        {
			return cur_config.test_update_list_cdn_url;
		}

		public ENV_ID GetEnvId()
        {
			return cur_config.env_id;
		}
	}
}
