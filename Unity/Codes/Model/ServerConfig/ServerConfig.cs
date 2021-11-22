using System.Collections.Generic;


namespace ET
{
    public enum ENV_ID:byte
    {
        DEVELOP = 1,//开发|测试服
        PUBLISH = 2, //预发布
        ONLINE = 3, //正式服
    }
    public class ServerConfig
    {
        public string[] iplist;
        public string update_list_cdn_url;
        public string res_cdn_url;
        public string test_update_list_cdn_url;
        public ENV_ID env_id;


        public static Dictionary<string, ServerConfig> config;
        public static string default_key;

        static ServerConfig()
        {
            config = new Dictionary<string, ServerConfig>()
            {
                {
                    "setting1",new ServerConfig{
                        iplist=new string[] { "172.22.213.58:10002" },
                        update_list_cdn_url = "http://172.22.213.58:8081/cdn",
                        res_cdn_url="http://172.22.213.58:8081/cdn",
                        test_update_list_cdn_url = "http://172.22.213.58:8081/cdn_test",
                        env_id = ENV_ID.DEVELOP,
                    } 
                },
                {
                    "setting2",new ServerConfig{
                        iplist=new string[] { "127.0.0.1:10002" },
                        update_list_cdn_url = "http://127.0.0.1:8081/cdn",
                        res_cdn_url = "http://127.0.0.1:8081/cdn",
                        test_update_list_cdn_url = "http://127.0.0.1:8081/cdn_test",
                        env_id = ENV_ID.DEVELOP,
                    }
                },
            };
            default_key = "setting1";
        }
    }
}
