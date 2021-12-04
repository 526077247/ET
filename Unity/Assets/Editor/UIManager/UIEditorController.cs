﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UIScriptController
{
    static string addressable_path = "Assets/AssetsPackage/";
    static string generate_path = "Demo";
    static bool forced_coverage = false;//是否强制覆盖
    public static bool AllowGenerate(GameObject go, string path)
    {
        if (!go.name.StartsWith("UI"))
        {
            UnityEngine.Debug.LogError(go.name + "没有以UI开头");
            return false;
        }
        if (!go.name.EndsWith("View") && !go.name.EndsWith("Win") && !go.name.EndsWith("Panel"))
        {
            UnityEngine.Debug.LogError(go.name + "没有以View、Win、Panel结尾");
            return false;
        }
        return path.Contains(addressable_path);
    }
    public static void GenerateUICode(GameObject go, string path)
    {
        UnityEngine.Debug.Log(path);
        GenerateEntityCode(go, path);
        GenerateSystemCode(go, path);
        //AssetDatabase.Refresh();
    }
    static Dictionary<Type, string> WidgetInterfaceList;
    static UIScriptController()//优先生成的排前面
    {
        WidgetInterfaceList = new Dictionary<Type, string>();
        WidgetInterfaceList.Add(typeof(SuperScrollView.LoopListView2), "UILoopListView2");
        WidgetInterfaceList.Add(typeof(SuperScrollView.LoopGridView), "UILoopGridView");
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
        var dirPath = $"Codes/ModelView/{generate_path}/{temp[index + 1]}/{temp[index + 2]}";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        var csPath = $"{dirPath}/{name}.cs";
        if (!forced_coverage && File.Exists(csPath))
        {
            UnityEngine.Debug.LogError("已存在 " + csPath + ",将不会再次生成。");
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

        strBuilder.AppendFormat("\tpublic class {0} : UIBaseContainer\r\n", name);
        strBuilder.AppendLine("\t{");
        strBuilder.AppendFormat("\t\tpublic static string PrefabPath => \"{0}\";", path.Replace(addressable_path, ""))
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
            var uisc = child.GetComponent<UIScriptCreator>();
            if (uisc != null && uisc.isMarked)
            {
                bool had = false;
                foreach (var uiComponent in WidgetInterfaceList)
                {
                    Component component = child.GetComponent(uiComponent.Key);
                    if (null != component)
                    {
                        had = true;
                        strBuilder.AppendFormat("\t\tpublic {0} {1};", uiComponent.Value, uisc.GetModuleName())
                            .AppendLine();
                        break;
                    }
                }

                if (!had)
                {
                    strBuilder.AppendFormat("\t\tpublic UIBaseContainer {0};", uisc.GetModuleName())
                            .AppendLine();
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
        var dirPath = $"Codes/HotfixView/{generate_path}/{temp[index + 1]}/{temp[index + 2]}";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        var csPath = $"{dirPath}/{name}System.cs";
        if (!forced_coverage && File.Exists(csPath))
        {
            UnityEngine.Debug.LogError("已存在 " + csPath + ",将不会再次生成。");
            return;
        }
        StreamWriter sw = new StreamWriter(csPath, false, Encoding.UTF8);
        StringBuilder strBuilder = new StringBuilder();
        StringBuilder tempBuilder = new StringBuilder();
        strBuilder.AppendLine("using System.Collections;")
                  .AppendLine("using System.Collections.Generic;")
                  .AppendLine("using System;")
                  .AppendLine("using UnityEngine;")
                  .AppendLine("using UnityEngine.UI;")
                  .AppendLine("using SuperScrollView;");

        strBuilder.AppendLine("namespace ET");
        strBuilder.AppendLine("{");
        strBuilder.AppendLine("\t[UISystem]");
        strBuilder.AppendFormat("\tpublic class {0}OnCreateSystem : OnCreateSystem<{1}>\r\n", name, name);
        strBuilder.AppendLine("\t{");
        strBuilder.AppendLine("");


        strBuilder.AppendFormat("\t\tpublic override void OnCreate({0} self)\n", name)
               .AppendLine("\t\t{");
        GenerateSystemChildCode(go.transform, "", strBuilder, tempBuilder, name);

        strBuilder.AppendLine("\t\t}");
        strBuilder.AppendLine("");
        strBuilder.AppendLine("\t}");

        strBuilder.AppendFormat("\tpublic static class {0}System\r\n", name);
        strBuilder.AppendLine("\t{");
        strBuilder.Append(tempBuilder);
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("");

        strBuilder.AppendLine("}");
        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
    }

    public static void GenerateSystemChildCode(Transform trans, string strPath, StringBuilder strBuilder, StringBuilder tempBuilder, string name)
    {
        if (null == trans)
        {
            return;
        }
        for (int nIndex = 0; nIndex < trans.childCount; ++nIndex)
        {
            Transform child = trans.GetChild(nIndex);
            string strTemp = strPath == "" ? child.name : (strPath + "/" + child.name);

            var uisc = child.GetComponent<UIScriptCreator>();
            if (uisc != null && uisc.isMarked)
            {
                bool had = false;
                foreach (var uiComponent in WidgetInterfaceList)
                {
                    Component component = child.GetComponent(uiComponent.Key);
                    if (null != component)
                    {
                        had = true;
                        strBuilder.AppendFormat("\t\t\tself.{0} = self.AddUIComponent<{1}>(\"{2}\");", uisc.GetModuleName(), uiComponent.Value, strTemp)
                            .AppendLine();
                        if (uiComponent.Key == typeof(Button))
                        {
                            strBuilder.AppendFormat("\t\t\tself.{0}.SetOnClick(()=>{{self.OnClick{1}();}});", uisc.GetModuleName(), uisc.GetModuleName())
                                    .AppendLine();
                            tempBuilder.AppendFormat("\t\tpublic static void OnClick{0}(this {1} self)", uisc.GetModuleName(), name)
                                    .AppendLine();
                            tempBuilder.AppendLine("\t\t{").AppendLine();
                            tempBuilder.AppendLine("\t\t}");
                        }
                        else if (uiComponent.Key == typeof(SuperScrollView.LoopListView2))
                        {
                            strBuilder.AppendFormat("\t\t\tself.{0}.InitListView(0,(a,b)=>{{return self.Get{1}ItemByIndex(a,b);}});", uisc.GetModuleName(), uisc.GetModuleName())
                                    .AppendLine();
                            tempBuilder.AppendFormat("\t\tpublic static LoopListViewItem2 Get{0}ItemByIndex(this {1} self,LoopListView2 listView, int index)", uisc.GetModuleName(), name)
                                    .AppendLine();
                            tempBuilder.AppendLine("\t\t{");
                            tempBuilder.AppendLine("\t\t\treturn null;");
                            tempBuilder.AppendLine("\t\t}");
                        }
                        else if (uiComponent.Key == typeof(SuperScrollView.LoopGridView))
                        {
                            strBuilder.AppendFormat("\t\t\tself.{0}.InitGridView(0,(a,b,c,d)=>{{return self.Get{1}ItemByIndex(a,b,c,d);}});", uisc.GetModuleName(), uisc.GetModuleName())
                                    .AppendLine();
                            tempBuilder.AppendFormat("\t\tpublic static LoopGridViewItem Get{0}ItemByIndex(this {1} self,LoopGridView gridview,int index, int row, int column)", uisc.GetModuleName(), name)
                                    .AppendLine();
                            tempBuilder.AppendLine("\t\t{");
                            tempBuilder.AppendLine("\t\t\treturn null;");
                            tempBuilder.AppendLine("\t\t}");
                        }
                        break;
                    }
                }

                if (!had)
                {
                    strBuilder.AppendFormat("\t\t\tself.{0} = self.AddUIComponent<UIBaseContainer>(\"{1}\");", uisc.GetModuleName(), strTemp)
                            .AppendLine();
                }
            }

            GenerateSystemChildCode(child, strTemp, strBuilder, tempBuilder, name);
        }
    }
}

