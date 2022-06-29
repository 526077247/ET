using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

namespace ET
{
    internal class MapExport2Nav: Editor
    {
        [MenuItem("Tools/NavMesh/导入场景mesh")]
        static void ImportSceneMesh()
        {
            string path = EditorUtility.OpenFilePanel("选择场景json文件", "", "json");
            if (path.Length != 0)
            {
                var jsonStr = File.ReadAllText(path);
                var data = LitJson.JsonMapper.ToObject<AssetsRoot>(jsonStr);
                EditorApplication.OpenScene("Assets/MapEditor/Map.unity");
                for (int i = 0; i < data.Scenes.Count; i++)
                {
                    var scene = data.Scenes[i];
                    GameObject sceneObj = GameObject.Find(scene.Name);
                    if(sceneObj!=null)
                        DestroyImmediate(sceneObj);
                    sceneObj = new GameObject(scene.Name);
                    for (int j = 0; j < scene.Objects.Count; j++)
                    {
                        var objInfo = scene.Objects[j];
                        GameObject obj = null;
                        string addressPath;
                        switch (objInfo.Type)
                        {
                            case "Prefab":
                                addressPath = "Assets/AssetsPackage/" + objInfo.PrefabPath;
                                var prefab = AssetDatabase.LoadAssetAtPath(addressPath, typeof (GameObject)) as GameObject;
                                if(prefab==null) continue;
                                obj = Instantiate(prefab,sceneObj.transform);
                                obj.name = objInfo.Name;
                                obj.transform.localPosition = objInfo.Transform.Position;
                                obj.transform.localRotation = objInfo.Transform.Rotation;
                                obj.transform.localScale = objInfo.Transform.Scale;
                                
                                break;
                            case "Terrain":
                                addressPath = "Assets/AssetsPackage/" + objInfo.TerrainPath;
                                var terrainData =  AssetDatabase.LoadAssetAtPath(addressPath, typeof(TerrainData)) as TerrainData;
                                if(terrainData==null) continue;
                                obj = new GameObject(objInfo.Name);
                                obj.transform.parent = sceneObj.transform;
                                obj.transform.localPosition = objInfo.Transform.Position;
                                obj.transform.localRotation = objInfo.Transform.Rotation;
                                obj.transform.localScale = objInfo.Transform.Scale;
                                
                                obj.AddComponent<MeshFilter>();
                                obj.AddComponent<MeshRenderer>();
                                var mesh = obj.AddComponent<ExportMesh>();
                                mesh.terrainData = terrainData;
                                mesh.Generic();
                                break;
                            default:
                                UnityEngine.Debug.Log("未处理的类型：" + objInfo.Type);
                                break;
                        }
                        if (obj != null)
                        {
                            var mesh = obj.GetComponentsInChildren<MeshFilter>();
                            for (int k = 0; k < mesh.Length; k++)
                            {
                                // if (mesh[k].GetComponent<Spine.Unity.SkeletonAnimation>() != null) continue;
                                mesh[k].gameObject.tag = "NavMesh";
                                mesh[k].gameObject.isStatic = true;
                            }
                        }
                    }
                }
            }
        }
        
        [MenuItem("Assets/导出场景到Josn数据")]
        static void ExportJson()
        {
            string path = EditorUtility.SaveFilePanel("Save Resource", "", "Map", "json");
            if (path.Length != 0)
            {
                Object[] selectedAssetList = Selection.GetFiltered(typeof (Object), SelectionMode.DeepAssets);
                // 如果存在场景文件，删除
                if (File.Exists(path)) File.Delete(path);
                AssetsRoot root = new AssetsRoot();
                root.Scenes = new List<AssetsScene>();
                
                //遍历所有的游戏对象
                foreach (Object selectObject in selectedAssetList)
                {
                    // 场景名称
                    string sceneName = selectObject.name;
                    // 场景路径
                    string scenePath = AssetDatabase.GetAssetPath(selectObject);
                    // 打开这个关卡
                    EditorApplication.OpenScene(scenePath);
                    AssetsScene sceneRoot = new AssetsScene();
                    root.Scenes.Add(sceneRoot);
                    sceneRoot.Name = sceneName;
                    sceneRoot.Objects = new List<AssetsObject>();
                    var scene = sceneRoot.Objects;
                    foreach (GameObject sceneObject in Object.FindObjectsOfType(typeof (GameObject)))
                    {
                        // 如果对象是激活状态
                        if (sceneObject.transform.parent == null && sceneObject.activeSelf)
                        {
                            ChangeObj2Data(sceneObject, scene);
                        }
                    }
                }

                // 保存场景数据
                File.WriteAllText(path, LitJson.JsonMapper.ToJson(root));
                // 刷新Project视图
                AssetDatabase.Refresh();
            }
        }

        public static void ChangeObj2Data(GameObject sceneObject,List<AssetsObject> scene)
        {
            var terrain = sceneObject.GetComponent<TerrainCollider>();
            if (terrain != null)
            {
                if(terrain.terrainData==null) return;
                AssetsObject obj = new AssetsObject();
                scene.Add(obj);
                obj.Name = sceneObject.name;
                obj.Type = "Terrain";
                string prefabObject = EditorUtility.GetAssetPath(terrain.terrainData);
                obj.TerrainPath = prefabObject.Replace("Assets/AssetsPackage/", "");
                obj.Size = terrain.bounds.size;
                AddTransformInfo(obj, sceneObject);
            }
            // 判断是否是预设
            else if (PrefabUtility.GetPrefabType(sceneObject) == PrefabType.PrefabInstance)
            {
                // 获取引用预设对象
                Object prefabObject = EditorUtility.GetPrefabParent(sceneObject);
            
                if (prefabObject != null)
                {
                    AssetsObject obj = new AssetsObject();
                    scene.Add(obj);
                    obj.Name = sceneObject.name;
                    obj.Type = "Prefab";
                    obj.PrefabPath = AssetDatabase.GetAssetPath(prefabObject).Replace("Assets/AssetsPackage/", "");
                    var mesh = sceneObject.GetComponentInChildren<Renderer>();
                    if (mesh != null)
                        obj.Size = mesh.bounds.size;
                    else
                        obj.Size = terrain.transform.localScale;
                    AddTransformInfo(obj, sceneObject);
                }
            }
            else
            {
                for (int i = 0; i < sceneObject.transform.childCount; i++)
                {
                    ChangeObj2Data(sceneObject.transform.GetChild(i).gameObject, scene);
                }
            }
        }

        public static void AddTransformInfo(AssetsObject obj,GameObject sceneObject)
        {
            AssetsTransform transform = new AssetsTransform();


            // 位置信息
            transform.Position = sceneObject.transform.position;

            // 旋转信息
            transform.Rotation = sceneObject.transform.rotation;

            // 缩放信息
            transform.Scale = sceneObject.transform.localScale;
            obj.Transform = transform;
        }
    }
}