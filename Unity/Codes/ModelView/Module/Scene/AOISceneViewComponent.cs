using System.Collections.Generic;
using UnityEngine;
namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class AOISceneViewComponent:Entity,IAwake<int>
    {
        public static AOISceneViewComponent Instance;
        public class DynamicScene
        {
            public string sceneName;
            public List<AssetsObject> Objects;
            public Dictionary<long, List<AssetsObject>> GridMapObjects;
        }
        public class DynamicSceneViewObj
        {
            public GameObject Obj;
            public bool IsLoading;
        }
        public Dictionary<string, DynamicScene> DynamicSceneMap;
        
        /// <summary>
        /// 格子边长
        /// </summary>
        public int GridLen;

        /// <summary>
        /// 当前地图
        /// </summary>
        public string CurMap;

        /// <summary>
        /// 当前需加载的场景物体
        /// </summary>
        public Dictionary<AssetsObject, int> DynamicSceneObjectMapCount;
        
        /// <summary>
        /// 当前已加载的场景物体
        /// </summary>
        public Dictionary<AssetsObject, DynamicSceneViewObj> DynamicSceneObjectMapObj;
        public int? LastGridX = null;
        public int? LastGridY = null;
    }
}