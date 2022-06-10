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
            public List<DynamicSceneObject> Objects;
            public Dictionary<long, List<DynamicSceneObject>> GridMapObjects;
        }
        public class DynamicSceneObject
        {
            public class Transform
            {
                public float[] position;
                public float[] rotation;
                public float[] scale;
                public Vector3 Position
                {
                    get
                    {
                        if (position != null && position.Length == 3)
                            return new Vector3(this.position[0], this.position[1], this.position[2]);
                        else
                        {
                            Log.Error("Position Error");
                            return Vector3.zero;
                        }
                    }
                }

                public Quaternion Rotation
                {
                    get
                    {
                        if (rotation != null && rotation.Length == 4)
                            return new Quaternion(this.rotation[0], this.rotation[1], this.rotation[2],this.rotation[3]);
                        else
                        {
                            Log.Error("Rotation Error");
                            return Quaternion.identity;
                        }
                    }
                }

                public Vector3 Scale
                {
                    get
                    {
                        if (this.scale != null && scale.Length == 3)
                            return new Vector3(this.scale[0], this.scale[1], this.scale[2]);
                        else
                        {
                            Log.Error("Scale Error");
                            return Vector3.one;
                        }
                    }
                }
            }

            public string objectName;
            public string objectPath;
            public Transform transform;
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
        public Dictionary<DynamicSceneObject, int> DynamicSceneObjectMapCount;
        
        /// <summary>
        /// 当前已加载的场景物体
        /// </summary>
        public Dictionary<DynamicSceneObject, DynamicSceneViewObj> DynamicSceneObjectMapObj;
        public int? LastGridX = null;
        public int? LastGridY = null;
    }
}