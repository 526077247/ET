using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    public class DownLoadInfo
    {
        public string name;
        public string hash;

    }

    public class WhiteConfig
    {
        public ENV_ID env_id;
        public string account;
    }
    public class Resver
    {
        public List<string> channel;
        public List<string> update_tailnumber;
        public int force_update;
    }
    public class AppConfig
    {
        public string app_url;
        public Dictionary<string, Resver> app_ver;
        public string jump_channel;
    }
    public class UpdateConfig
    {
        public Dictionary<string,Dictionary<string, Resver>> res_list;
        public Dictionary<string, AppConfig> app_list;
    }
    public class BootConfig : Singleton<BootConfig>
    {
        string m_update_list_cdn_url;
        string m_cdn_url;
        bool m_inWhiteList;
        Dictionary<string, Dictionary<string, Resver>> m_resUpdateList;
        Dictionary<string, AppConfig> m_appUpdateList;
        public BootConfig()
        {
            m_update_list_cdn_url = ServerConfigManagerComponent.Instance.GetUpdateListCdnUrl();
            m_cdn_url = ServerConfigManagerComponent.Instance.GetResCdnUrl();
            m_inWhiteList = false;
        }

        public override void Dispose()
        {
            Release();
            m_resUpdateList = null;
            m_appUpdateList = null;
        }

        #region 白名单
        //获取白名单下载地址
        public string GetWhiteListCdnUrl()
        {
            if (string.IsNullOrEmpty(m_update_list_cdn_url)) return m_update_list_cdn_url;
            return string.Format("{0}/white.list", m_update_list_cdn_url);
        }

        //设置白名单模式
        public void SetWhiteMode(bool whiteMode)
        {
            if (whiteMode)
            {
                m_update_list_cdn_url = ServerConfigManagerComponent.Instance.GetTestUpdateListCdnUrl();
            }
        }

        //设置白名单列表
        //格式为json格式
	    //{
		// "WhiteList":[
    	//	    {"env_id":1, "uid":11111}
    	//    ]
        //}
        public void SetWhiteList(List<WhiteConfig> info)
        {
            m_inWhiteList = false;
            var env_id = ServerConfigManagerComponent.Instance.GetEnvId();
            var account = PlayerPrefs.GetString(CacheKeys.Account);
            foreach (var item in info)
            {
                if (item.env_id == env_id && item.account == account)
                {
                    m_inWhiteList = true;
                    Log.Info(" user is in white list {0}".Fmt(account));
                    break;
                }
            }
        }
        //是否在白名单中
        public bool IsInWhiteList()
        {
            return m_inWhiteList;
        }
        #endregion

        //获取更新列表地址, 平台独立
        //格式为json格式
        //    {
        //        "res_list" : {
        //                "100": {
        //                       "1.0.0": {"channel": ["all"], "update_tailnumber": ["all"]},
        //                 }
        //        },
        //        "app_list" : { 
        //                 "googleplay": {
        //                      "app_url": "https://www.baidu.com/",
        //                       "app_ver": {
        //	                           "1.0.1": { "force_update": 1 }
        //                       }
        //                  }
        //         }
        //    }
        public string GetUpdateListCdnUrl()
        {
            var url = string.Format("{0}/update_{1}.list", m_update_list_cdn_url, PlatformUtil.GetStrPlatformIgnoreEditor());
            Log.Info("GetUpdateListUrl url = {0}".Fmt(url));
            return url;
        }

        //设置更新列表
        public void SetUpdateList(UpdateConfig info)
        {
            m_appUpdateList = info.app_list;
            m_resUpdateList = info.res_list;
        }

        //根据渠道获取app更新列表
        public AppConfig GetAppUpdateListByChannel(string channel)
        {
            if (m_appUpdateList == null) return null;
            if(m_appUpdateList.TryGetValue(channel,out var data))
            {
                if (!string.IsNullOrEmpty(data.jump_channel))
                    data = m_appUpdateList[data.jump_channel];
                return data;
            }
            return null;
        }
        //找到可以更新的最大app版本号
        public string FindMaxUpdateAppVer(string channel,string local_app_ver = "")
        {
            if (m_appUpdateList == null) return null;
            string last_ver = null;
            if (m_appUpdateList.TryGetValue(channel, out var data))
            {
                foreach (var item in data.app_ver)
                {
                    if (last_ver == null) last_ver = item.Key;
                    else
                    {
                        if(VersionCompare.Compare(item.Key, last_ver) > 0)
                        {
                            last_ver = item.Key;
                        }
                    }
                }
            }
            return last_ver;
        }

        //找到可以更新的最大资源版本号
        public string FindMaxUpdateResVer(string engine_ver, string channel)
        {
            if (string.IsNullOrEmpty(engine_ver) || m_resUpdateList == null || 
                !m_resUpdateList.TryGetValue(engine_ver, out var resVerList)) return null;
            if (resVerList == null) return null;
            var verList = new List<string>();
            foreach (var item in resVerList)
            {
                verList.Add(item.Key);
            }
            verList.Sort((a, b) => { return -VersionCompare.Compare(a, b); });
            string last_ver = "";
            for (int i = 0; i < verList.Count; i++)
            {
                var info = resVerList[verList[i]];
                if(IsStrInList(channel,info.channel)&& IsInTailNumber(info.update_tailnumber))
                {
                    last_ver = verList[i];
                    break;
                }
            }
            return last_ver;
        }
        //检测灰度更新，检测是否在更新尾号列表
        bool IsInTailNumber(List<string> list)
        {
            if (list == null) return false;
            var account = PlayerPrefs.GetString(CacheKeys.Account, "");
            var tail_number = "";
            if (!string.IsNullOrEmpty(account))
                tail_number = account[account.Length - 1].ToString();
            for (int i = 0; i < list.Count; i++)
                if (list[i] == "all" || tail_number == list[i])
                    return true;
            return false;
        }

        bool IsStrInList(string str,List<string> list)
        {
            if (list == null) return false;
            for (int i = 0; i < list.Count; i++)
                if (list[i] == "all" || str == list[i])
                    return true;
            return false;
        }

        //根据资源版本号获取在cdn上的资源地址
        public string GetUpdateCdnResUrlByVersion(string resver)
        {
            var platformStr = PlatformUtil.GetStrPlatformIgnoreEditor();
            var url = string.Format("{0}/{1}_{2}", m_cdn_url, resver, platformStr);
            Log.Info("GetUpdateCdnResUrl url = {0}".Fmt(url));
            return url;
        }
    }
}
