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
        public SceneNames Name;
        static Dictionary<SceneNames, SceneConfig> SceneConfigs;
        static SceneConfig()
        {
            SceneConfigs = new Dictionary<SceneNames, SceneConfig>(SceneNamesCompare.Instance);
            SceneConfigs.Add(InitScene.Name, InitScene);
            SceneConfigs.Add(LoadingScene.Name, LoadingScene);
            SceneConfigs.Add(MapScene.Name, MapScene);
            SceneConfigs.Add(LoginScene.Name, LoginScene);
        }

        public static SceneConfig InitScene = new SceneConfig
        {
            SceneAddress = "Scenes/InitScene/Init.unity",
            Name = SceneNames.Init,
        };
        public static SceneConfig LoadingScene = new SceneConfig
        {
            SceneAddress = "Scenes/LoadingScene/Loading.unity",
            Name = SceneNames.Loading,
        };
        public static SceneConfig LoginScene = new SceneConfig
        {
            SceneAddress = "Scenes/LoginScene/Login.unity",
            Name = SceneNames.Login,
        };
        public static SceneConfig MapScene = new SceneConfig
        {
            SceneAddress = "Scenes/MapScene/Map.unity",
            Name = SceneNames.Map,
        };

        public static SceneConfig GetSceneConfigByName(SceneNames name)
        {
            if(SceneConfigs.TryGetValue(name,out var res))
            {
                return res;
            }
            return null;
        }

        public static SceneConfig GetSceneConfigById(int id)
        {
            SceneNames name = (SceneNames)id;
            if (SceneConfigs.TryGetValue(name, out var res))
            {
                return res;
            }
            return null;
        }
    }
}
