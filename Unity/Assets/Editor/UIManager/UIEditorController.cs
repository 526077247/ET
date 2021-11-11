using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using System.IO;
using ET;
using UnityEngine;
using UnityEngine.UI;

public class UIScriptController
{
    static string addressable_path = "Assets/AssetsPackage/";

    public static bool AllowGenerate(GameObject go, string path)
    {
        if (!go.name.StartsWith("UI"))
        {
            Log.Error(go.name + "没有以UI开头");
            return false;
        }
        if (!go.name.EndsWith("View") && !go.name.EndsWith("Win") && !go.name.EndsWith("Panel"))
        {
            Log.Error(go.name + "没有以View、Win、Panel结尾");
            return false;
        }
        return path.Contains(addressable_path);
    }
    public static void GenerateUICode(GameObject go, string path)
    {
        Log.Debug(path);
        GenerateEntityCode(go, path);
        GenerateSystemCode(go, path);
        AssetDatabase.Refresh();
    }
    static Dictionary<Type, string> WidgetInterfaceList;
    static UIScriptController()
    {
        WidgetInterfaceList = new Dictionary<Type, string>();
        WidgetInterfaceList.Add(typeof(Button), "UIButton");
        WidgetInterfaceList.Add(typeof(Text), "UIText");
        WidgetInterfaceList.Add(typeof(InputField), "UIInput");
        WidgetInterfaceList.Add(typeof(Slider), "UISlider");
        WidgetInterfaceList.Add(typeof(Image), "UIImage");
        WidgetInterfaceList.Add(typeof(RawImage), "UIRawImage");
        WidgetInterfaceList.Add(typeof(TMPro.TMP_Text), "UITextmesh");
        WidgetInterfaceList.Add(typeof(TMPro.TMP_InputField), "UIInputTextmesh");
        WidgetInterfaceList.Add(typeof(PointerClick), "UIPointerClick");
    }

    static void GenerateEntityCode(GameObject go, string path)
    {
        string name = go.name;
        var temp = new List<string>(path.Split('/'));
        int index = temp.IndexOf("AssetsPackage");
        var dirPath = $"Assets/ModelView/Demo/{temp[index + 1]}/{temp[index + 2]}";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        var csPath = $"{dirPath}/{name}.cs";
        if (File.Exists(csPath))
        {
            Log.Error("已存在 " + csPath + ",将不会再次生成。");
            return;
        }
        StreamWriter sw = new StreamWriter(csPath, false, Encoding.UTF8);
        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine("using System.Collections;")
                  .AppendLine("using System.Collections.Generic;")
                  .AppendLine("using System;")
                  .AppendLine("using UnityEngine;")
                  .AppendLine("using UnityEngine.UI;\r\n");

        strBuilder.AppendLine("namespace ET");
        strBuilder.AppendLine("{");

        strBuilder.AppendFormat("\tpublic class {0} : UIBaseView\r\n", name);
        strBuilder.AppendLine("\t{");
        strBuilder.AppendFormat("\t\tpublic override string PrefabPath => \"{0}\";", path.Replace(addressable_path, ""))
            .AppendLine();
        GenerateEntityChildCode(go.transform, "", strBuilder);
        strBuilder.AppendLine("\t\t \r\n");

        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("}");
        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
    }
    public static void GenerateEntityChildCode(Transform trans, string strPath, StringBuilder strBuilder)
    {
        if (null == trans)
        {
            return;
        }
        for (int nIndex = 0; nIndex < trans.childCount; ++nIndex)
        {
            Transform child = trans.GetChild(nIndex);
            string strTemp = strPath + "/" + child.name;

            if (child.GetComponent<UIScriptCreator>()!=null && child.GetComponent<UIScriptCreator>().isMarked)
            {
                foreach (var uiComponent in WidgetInterfaceList)
                {
                    Component component = child.GetComponent(uiComponent.Key);
                    if (null != component)
                    {
                        strBuilder.AppendFormat("\t\tpublic {0} {1};", uiComponent.Value, child.name)
                            .AppendLine();
                        break;
                    }
                }
            }

            GenerateEntityChildCode(child, strTemp, strBuilder);
        }
    }
    static void GenerateSystemCode(GameObject go, string path)
    {
        string name = go.name;
        var temp = new List<string>(path.Split('/'));
        int index = temp.IndexOf("AssetsPackage");
        var dirPath = $"Assets/HotfixView/Demo/{temp[index + 1]}/{temp[index + 2]}";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        var csPath = $"{dirPath}/{name}System.cs";
        if (File.Exists(csPath))
        {
            Log.Error("已存在 " + csPath + ",将不会再次生成。");
            return;
        }
        StreamWriter sw = new StreamWriter(csPath, false, Encoding.UTF8);
        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine("using System.Collections;")
                  .AppendLine("using System.Collections.Generic;")
                  .AppendLine("using System;")
                  .AppendLine("using UnityEngine;")
                  .AppendLine("using UnityEngine.UI;\r\n");

        strBuilder.AppendLine("namespace ET");
        strBuilder.AppendLine("{");

        strBuilder.AppendFormat("\tpublic class {0}OnCreateSystem : OnCreateSystem<{1}>\r\n", name, name);
        strBuilder.AppendLine("\t{");
        strBuilder.AppendLine("");


        strBuilder.AppendFormat("\t\tpublic override void OnCreate({0} self)\n", name)
               .AppendLine("\t\t{");
        GenerateSystemChildCode(go.transform, "", strBuilder);

        strBuilder.AppendLine("\t\t}");
        strBuilder.AppendLine("");
        strBuilder.AppendLine("\t}");

        strBuilder.AppendFormat("\tpublic static class {0}System\r\n", name);
        strBuilder.AppendLine("\t{");
        strBuilder.AppendLine("");
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("");

        strBuilder.AppendLine("}");
        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
    }

    public static void GenerateSystemChildCode(Transform trans, string strPath, StringBuilder strBuilder)
    {
        if (null == trans)
        {
            return;
        }
        for (int nIndex = 0; nIndex < trans.childCount; ++nIndex)
        {
            Transform child = trans.GetChild(nIndex);
            string strTemp = strPath==""? child.name: (strPath + "/" + child.name);

            if (child.GetComponent<UIScriptCreator>() != null && child.GetComponent<UIScriptCreator>().isMarked)
            {
                foreach (var uiComponent in WidgetInterfaceList)
                {
                    Component component = child.GetComponent(uiComponent.Key);
                    if (null != component)
                    {
                        strBuilder.AppendFormat("\t\t\tself.{0} = self.AddComponent<{1}>(\"{2}\");", child.name, uiComponent.Value, strTemp)
                            .AppendLine();
                        break;
                    }
                }
            }

            GenerateSystemChildCode(child, strTemp, strBuilder);
        }
    }
}

