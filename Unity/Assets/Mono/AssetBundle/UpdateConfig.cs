using System.Collections.Generic;
namespace ET
{
    public enum ENV_ID:byte
    {
        DEVELOP = 1,//开发|测试服
        PUBLISH = 2, //预发布
        ONLINE = 3, //正式服
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
}