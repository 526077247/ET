using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [ObjectSystem]
    public class BaseSceneAwakeSystem : AwakeSystem<BaseScene,SceneConfig>
    {
        public override void Awake(BaseScene self, SceneConfig config)
        {
            self.Awake(config);
        }
    }
}
