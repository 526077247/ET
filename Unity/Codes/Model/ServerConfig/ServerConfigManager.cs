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
		readonly string ServerKey = "ServerId";
		private readonly int defaultServer = 1;
		ServerConfig cur_config;
		public static ServerConfigManagerComponent Instance;
		public void Awake()
		{
			Instance = this;
			cur_config = ServerConfigCategory.Instance.Get(PlayerPrefs.GetInt(ServerKey, defaultServer));
            if (cur_config == null)
            {
	            cur_config = ServerConfigCategory.Instance.GetOne();
			}
		}

		public ServerConfig GetCurConfig()
        {
			return cur_config;

		}

		public ServerConfig ChangeEnv(int id)
        {
	        var conf = ServerConfigCategory.Instance.Get(id);
			if(conf!=null)
            {
				cur_config = conf;
				PlayerPrefs.SetInt(ServerKey, id);
			}
			return cur_config;

		}
		//获取正式环境更新列表cdn地址
		public string GetUpdateListCdnUrl()
        {
			return cur_config.UpdateListUrl;
		}

		// 获取客户端资源cdn地址
		public string GetResCdnUrl()
		{
			return cur_config.ResUrl;
		}
		//获取测试环境更新列表cdn地址
		public string GetTestUpdateListCdnUrl()
        {
			return cur_config.TestUpdateListUrl;
		}

		public int GetEnvId()
        {
			return cur_config.EnvId;
		}
	}
}
