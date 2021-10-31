using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public partial class SceneConfig
    {
        public string SceneAddress;
        public string Name;
        static Dictionary<string, SceneConfig> SceneConfigs;
        static SceneConfig()
        {
            SceneConfigs = new Dictionary<string, SceneConfig>();
            SceneConfigs.Add(InitScene.Name, InitScene);
            SceneConfigs.Add(LoadingScene.Name, LoadingScene);
            SceneConfigs.Add(MapScene.Name, MapScene);
            SceneConfigs.Add(WorldScene.Name, WorldScene);
            SceneConfigs.Add(LoginScene.Name, LoginScene);
        }

        public static SceneConfig InitScene = new SceneConfig
        {
            SceneAddress = "Scenes/InitScene/Init.unity",
            Name = "Init",
        };
        public static SceneConfig LoadingScene = new SceneConfig
        {
            SceneAddress = "Scenes/LoadingScene/Loading.unity",
            Name = "Loading",
        };
        public static SceneConfig LoginScene = new SceneConfig
        {
            SceneAddress = "Scenes/LoginScene/Login.unity",
            Name = "Login",
        };
        public static SceneConfig MapScene = new SceneConfig
        {
            SceneAddress = "Scenes/MapScene/Map.unity",
            Name = "Map",
        };
        public static SceneConfig WorldScene = new SceneConfig
        {
            SceneAddress = "Scenes/MapScene/Map.unity",
            Name = "World",
        };

        public static SceneConfig GetSceneConfigByName(string name)
        {
            if(SceneConfigs.TryGetValue(name,out var res))
            {
                return res;
            }
            return null;
        }
    }
}
