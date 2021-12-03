using AssetBundles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
namespace ET
{
    public class MapScene : BaseScene
    {
        public override void Awake(SceneConfig scene_config)
        {
            base.Awake(scene_config);
            this.PreLoadTask = ListComponent<ETTask>.Create();
            var role = UnitConfigCategory.Instance.GetAll();
            foreach (var item in role)
                this.PreLoadTask.Add(AddPreloadGameObject(item.Value.Perfab, 1));
        }
    }
}
