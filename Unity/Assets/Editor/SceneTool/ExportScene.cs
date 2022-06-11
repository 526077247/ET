using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;

public class ExportScene: Editor
{
    // [MenuItem("Assets/Export Scene To XML From Selection")]
    static void ExportXML()
    {
        string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "xml");
        if (path.Length != 0)
        {
            Object[] selectedAssetList = Selection.GetFiltered(typeof (Object), SelectionMode.DeepAssets);
            // 如果存在场景文件，删除
            if (File.Exists(path)) File.Delete(path);
            
            XmlDocument xmlDocument = new XmlDocument();
            // 创建XML属性
            XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDocument.AppendChild(xmlDeclaration);
            // 创建XML根标志
            XmlElement rootXmlElement = xmlDocument.CreateElement("root");
            //遍历所有的游戏对象
            foreach (Object selectObject in selectedAssetList)
            {
                // 场景名称
                string sceneName = selectObject.name;
                // 场景路径
                string scenePath = AssetDatabase.GetAssetPath(selectObject);
                // 打开这个关卡
                EditorApplication.OpenScene(scenePath);

                // 创建场景标志
                XmlElement sceneXmlElement = xmlDocument.CreateElement("scene");
                sceneXmlElement.SetAttribute("sceneName", sceneName);

                foreach (GameObject sceneObject in Object.FindObjectsOfType(typeof (GameObject)))
                {
                    // 如果对象是激活状态
                    if (sceneObject.transform.parent == null && sceneObject.activeSelf)
                    {
                        // 判断是否是预设
                        if (PrefabUtility.GetPrefabType(sceneObject) == PrefabType.PrefabInstance)
                        {
                            // 获取引用预设对象
                            Object prefabObject = EditorUtility.GetPrefabParent(sceneObject);
                            
                            if (prefabObject != null)
                            {
                                XmlElement gameObjectXmlElement = xmlDocument.CreateElement("gameObject");
                                gameObjectXmlElement.SetAttribute("objectName", sceneObject.name);
                                gameObjectXmlElement.SetAttribute("objectPath", AssetDatabase.GetAssetPath(prefabObject).Replace("Assets/AssetsPackage/",""));

                                XmlElement transformXmlElement = xmlDocument.CreateElement("transform");

                                // 位置信息
                                XmlElement positionXmlElement = xmlDocument.CreateElement("position");
                                positionXmlElement.SetAttribute("x", sceneObject.transform.position.x.ToString());
                                positionXmlElement.SetAttribute("y", sceneObject.transform.position.y.ToString());
                                positionXmlElement.SetAttribute("z", sceneObject.transform.position.z.ToString());

                                // 旋转信息
                                XmlElement rotationXmlElement = xmlDocument.CreateElement("rotation");
                                rotationXmlElement.SetAttribute("x", sceneObject.transform.rotation.eulerAngles.x.ToString());
                                rotationXmlElement.SetAttribute("y", sceneObject.transform.rotation.eulerAngles.y.ToString());
                                rotationXmlElement.SetAttribute("z", sceneObject.transform.rotation.eulerAngles.z.ToString());

                                // 缩放信息
                                XmlElement scaleXmlElement = xmlDocument.CreateElement("scale");
                                scaleXmlElement.SetAttribute("x", sceneObject.transform.localScale.x.ToString());
                                scaleXmlElement.SetAttribute("y", sceneObject.transform.localScale.y.ToString());
                                scaleXmlElement.SetAttribute("z", sceneObject.transform.localScale.z.ToString());

                               

                                transformXmlElement.AppendChild(positionXmlElement);
                                transformXmlElement.AppendChild(rotationXmlElement);
                                transformXmlElement.AppendChild(scaleXmlElement);

                                gameObjectXmlElement.AppendChild(transformXmlElement);
                                sceneXmlElement.AppendChild(gameObjectXmlElement);
                            }
                        }
                    }
                }

                rootXmlElement.AppendChild(sceneXmlElement);
                
            }
            xmlDocument.AppendChild(rootXmlElement);
            // 保存场景数据
            xmlDocument.Save(path);
            // 刷新Project视图
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("Assets/Export Scene To Json From Selection")]
    static void ExportJson()
    {
        string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "json");
        if (path.Length != 0)
        {
            Object[] selectedAssetList = Selection.GetFiltered(typeof (Object), SelectionMode.DeepAssets);
            // 如果存在场景文件，删除
            if (File.Exists(path)) File.Delete(path);
            List<Dictionary<string, object>> root = new List<Dictionary<string, object>>();

            //遍历所有的游戏对象
            foreach (Object selectObject in selectedAssetList)
            {
                // 场景名称
                string sceneName = selectObject.name;
                // 场景路径
                string scenePath = AssetDatabase.GetAssetPath(selectObject);
                // 打开这个关卡
                EditorApplication.OpenScene(scenePath);
                Dictionary<string, object> sceneRoot = new Dictionary<string, object>();
                root.Add(sceneRoot);
                sceneRoot.Add("sceneName",sceneName);
                List<Dictionary<string, object>> scene= new List<Dictionary<string, object>>();
                sceneRoot.Add("Objects",scene);
                foreach (GameObject sceneObject in Object.FindObjectsOfType(typeof (GameObject)))
                {
                    // 如果对象是激活状态
                    if (sceneObject.transform.parent == null && sceneObject.activeSelf)
                    {
                        // 判断是否是预设
                        if (PrefabUtility.GetPrefabType(sceneObject) == PrefabType.PrefabInstance)
                        {
                            // 获取引用预设对象
                            Object prefabObject = EditorUtility.GetPrefabParent(sceneObject);

                            if (prefabObject != null)
                            {
                                Dictionary<string, object> obj = new Dictionary<string, object>();
                                scene.Add(obj);
                                obj.Add("objectName", sceneObject.name);
                                obj.Add("objectPath", AssetDatabase.GetAssetPath(prefabObject).Replace("Assets/AssetsPackage/", ""));

                                Dictionary<string, float[]> transform = new Dictionary<string, float[]>();
                                

                                // 位置信息
                                transform.Add("position",
                                    new[] { sceneObject.transform.position.x, sceneObject.transform.position.y, sceneObject.transform.position.z });

                                // 旋转信息
                                transform.Add("rotation",
                                    new[]
                                    {
                                        sceneObject.transform.rotation.x, sceneObject.transform.rotation.y, sceneObject.transform.rotation.z,
                                        sceneObject.transform.rotation.w
                                    });

                                // 缩放信息
                                transform.Add("scale",
                                    new[]
                                    {
                                        sceneObject.transform.localScale.x, sceneObject.transform.localScale.y,
                                        sceneObject.transform.localScale.z
                                    });
                                obj.Add("transform", transform);
                            }
                        }
                    }
                }


            }

            // 保存场景数据
            File.WriteAllText(path, LitJson.JsonMapper.ToJson(root));
            // 刷新Project视图
            AssetDatabase.Refresh();
        }
    }
}