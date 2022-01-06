using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public class SceneManagerComponent:Entity,IAwake,IDestroy
    {
        public List<string> ScenesChangeIgnoreClean ;
        public List<string> DestroyWindowExceptNames;
        public static SceneManagerComponent Instance;

        public Dictionary<SceneNames, SceneConfig> SceneConfigs;
        //当前场景
        public SceneNames current_scene;
        //是否忙
        public bool busing = false;

    }
}
