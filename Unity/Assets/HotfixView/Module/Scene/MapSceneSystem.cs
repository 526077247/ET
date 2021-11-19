using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [ObjectSystem]
    public class MapSceneAwakeSystem : AwakeSystem<MapScene, SceneConfig>
    {
        public override void Awake(MapScene self, SceneConfig config)
        {
            self.Awake(config);
        }
    }
}
