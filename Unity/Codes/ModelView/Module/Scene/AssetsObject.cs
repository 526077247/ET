using UnityEngine;

namespace ET
{
    public class AssetsObject
    {
        public string Name;
        public string Type;

        public Vector3 Size;//网格长宽
        public AssetsTransform Transform;

        #region Type-Terrain
        
        public string TerrainPath;
        
        #endregion

        #region Type-Prefab

        public string PrefabPath;

        #endregion
        

    }
}