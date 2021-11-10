using UnityEngine;
using System.Collections;
using AssetBundles;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// added by wsh @ 2018.01.03
/// 功能：打包前的AB检测工作
/// </summary>

public static class CheckAssetBundles
{
    public static void SwitchChannel(string channelName)
    {
        var channelFolderPath = AssetBundleUtility.PackagePathToAssetsPath(AssetBundleConfig.ChannelFolderName);
        var guids = AssetDatabase.FindAssets("t:textAsset", new string[] { channelFolderPath });
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            GameUtility.SafeWriteAllText(path, channelName);
        }
        AssetDatabase.Refresh();
    }

    public static void ClearAllAssetBundles()
    {
        var assebundleNames = AssetDatabase.GetAllAssetBundleNames();
        var length = assebundleNames.Length;
        var count = 0;
        foreach (var assetbundleName in assebundleNames)
        {
            count++;
            EditorUtility.DisplayProgressBar("Remove assetbundle name :", assetbundleName, (float)count / length);
            AssetDatabase.RemoveAssetBundleName(assetbundleName, true);
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();

        assebundleNames = AssetDatabase.GetAllAssetBundleNames();
        if (assebundleNames.Length != 0)
        {
            Logger.LogError("Something wrong!!!");
        }
    }

    public static void RunAllCheckers()
    {
        var guids = AssetDatabase.FindAssets("t:AddressableDispatcherConfig", new string[] { AddressableInspectorUtils.DatabaseRoot });
        var length = guids.Length;
        var count = 0;
        Dictionary<string, List<string>> groupDi = new Dictionary<string, List<string>>();
        foreach (var guid in guids)
        {
            count++;
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var config = AssetDatabase.LoadAssetAtPath<AddressableDispatcherConfig>(assetPath);
            config.Load();
            EditorUtility.DisplayProgressBar("Run checker :", config.PackagePath, (float)count / length);
            string is_atlas_model = EditorUserSettings.GetConfigValue(AddressableTools.is_atlas_model);
            AddressableDispatcher.Run(config, is_atlas_model,groupDi);
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    public static void Run()
    {
        ClearAllAssetBundles();
        RunAllCheckersTask();
    }


    //==================================================================================================================
    private static EditorApplication.CallbackFunction _updateDelegate;
    public delegate Dictionary<string, List<string>> ThreadRun(ThreadPars par);
    private const int ThreadCount = 4;
    public class ThreadPars
    {
        public List<AddressableDispatcherConfig> ChildDataList = new List<AddressableDispatcherConfig>();
        public string is_atlas_model = "1";
    }

    private static Dictionary<string, List<string>> ThreadFind(ThreadPars par)
    {
        if (par != null)
        {
            Dictionary<string, List<string>> groupDi = new Dictionary<string, List<string>>();
            for (int i = 0; i < par.ChildDataList.Count; i++)
            {
                AddressableDispatcher.Run(par.ChildDataList[i], par.is_atlas_model, groupDi);
            }
            return groupDi;
        }
        return null;
    }

    private static void RunAllCheckersTask()
    {
        ThreadPars[] threadParses = new ThreadPars[ThreadCount];
        for (int index = 0; index < ThreadCount; index++)
        {
            threadParses[index] = new ThreadPars();
        }

        var guids = AssetDatabase.FindAssets("t:AddressableDispatcherConfig", new string[] { AddressableInspectorUtils.DatabaseRoot });
        var length = guids.Length;
        var count = 0;
        foreach (var guid in guids)
        {
            count++;
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var config = AssetDatabase.LoadAssetAtPath<AddressableDispatcherConfig>(assetPath);
            config.Load();

            int index = count % ThreadCount;
            threadParses[index].ChildDataList.Add(config);
            threadParses[index].is_atlas_model = EditorUserSettings.GetConfigValue(AddressableTools.is_atlas_model);
        }

        List<Task<Dictionary<string, List<string>>>> taskList = new List<Task<Dictionary<string, List<string>>>>();

        taskList.Add(new Task<Dictionary<string, List<string>>>(()=>
        {
            return ThreadFind(threadParses[0]);
        }));
        taskList[0].Start();

        taskList.Add(new Task<Dictionary<string, List<string>>>(() =>
        {
            return ThreadFind(threadParses[1]);
        }));
        taskList[1].Start();

        taskList.Add(new Task<Dictionary<string, List<string>>>(() =>
        {
            return ThreadFind(threadParses[2]);
        }));
        taskList[2].Start();

        taskList.Add(new Task<Dictionary<string, List<string>>>(() =>
        {
            return ThreadFind(threadParses[3]);
        }));
        taskList[3].Start();

        for (int i = 0; i < ThreadCount; i++)
        {
            taskList[i].Wait();
        }

        //Logger.LogError("=======over=========");
        Dictionary<string, UnityEditor.AddressableAssets.Settings.AddressableAssetGroup> groupsDic = new Dictionary<string, UnityEditor.AddressableAssets.Settings.AddressableAssetGroup>();
        for (int i = 0; i < ThreadCount; i++)
        {
            foreach(var keyvalue in taskList[i].Result)
            {
                keyvalue.Value.Sort();
                foreach (var pathStr in keyvalue.Value)
                {
                    //Logger.LogError("path:" + pathStr + " groupName:" + keyvalue.Key);
                    if(!groupsDic.ContainsKey(keyvalue.Key))
                    {
                        groupsDic.Add(keyvalue.Key,AASUtility.CreateGroup(keyvalue.Key));
                    }
                    

                    UnityEditor.AddressableAssets.Settings.AddressableAssetEntry temp_entry = null;
                    var s = AASUtility.GetSettings();
                    foreach (UnityEditor.AddressableAssets.Settings.AddressableAssetGroup group in s.groups)
                    {
                        if (group == null)
                        {
                            continue;
                        }
                        foreach (UnityEditor.AddressableAssets.Settings.AddressableAssetEntry entry in group.entries)
                        {
                            //Logger.LogError("entry.AssetPath:" + entry.AssetPath + " pathstr:" + pathStr + " group.name:" + group.name + " keyvalue.key:" + keyvalue.Key);
                            if ((entry.AssetPath.Replace('\\','/') == pathStr.Replace('\\', '/')) && (group.name == keyvalue.Key))
                            {
                                //Logger.LogError("============temp_entry=====================" + pathStr);
                                temp_entry = entry;
                            }
                        }
                    }

                    if(temp_entry == null)
                    {
                        //Logger.LogError("=================================" + pathStr);
                        var guid = AssetDatabase.AssetPathToGUID(pathStr);
                        AASUtility.AddAssetToGroup(guid, keyvalue.Key);
                    }
                }
            }
        }

        AssetDatabase.Refresh();
    }

}
