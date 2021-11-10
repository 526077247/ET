using AssetBundles;
using System;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor;
using UnityEngine;

public class AASUtility : Editor
{
    public static AddressableAssetSettings GetSettings()
    {
        //アドレサブルアセットセッティング取得
        var d = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(
            "Assets/AddressableAssetsData/AddressableAssetSettings.asset"
            );
        return d;
    }


    public static AddressableAssetGroup CreateGroup(string groupName,bool setAsDefaultGroup=false)
    {
        //アドレサブルアセットセッティング取得
        var s = GetSettings();
        //スキーマ生成
        List<AddressableAssetGroupSchema> schema = new List<AddressableAssetGroupSchema>() {
             ScriptableObject.CreateInstance<UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema>(),
             ScriptableObject.CreateInstance<UnityEditor.AddressableAssets.Settings.GroupSchemas.ContentUpdateGroupSchema>(),

        };
        //グループの作成
        var f = s.groups.Find((g) => {
            return g.name == groupName;
        });
        if (f == null)
        {
            return s.CreateGroup(groupName, setAsDefaultGroup, false, true, schema);
        }

        return f;
    }

    public static void AddAssetToGroup(string assetGuid, string groupName)
    {
        if (assetGuid.Equals(""))
        {
            Debug.Log($"assetGuid is empty, groupName: {groupName}");
            return;
        }
        var s = GetSettings();
        var g = CreateGroup(groupName);
        var entry = s.CreateOrMoveEntry(assetGuid, g);
        entry.address = entry.address.Replace("Assets/" + AssetBundleConfig.AssetsFolderName + "/", "");
    }
    public static void SetLabelToAsset(List<string> assetGuidList, string label, bool flag)
    {
        var s = GetSettings();
        //ラベルを追加するように呼んでおく。追加されていないと設定されない。
        s.AddLabel(label);
        List<AddressableAssetEntry> assetList = new List<AddressableAssetEntry>();
        s.GetAllAssets(assetList, true);
        foreach (var assetGuid in assetGuidList)
        {
            var asset = assetList.Find((a) => { return a.guid == assetGuid; });
            if (asset != null)
            {
                asset.SetLabel(label, flag);
            }
        }
    }
    public static void RemoveAssetFromGroup(string assetGuid)
    {
        var s = GetSettings();
        s.RemoveAssetEntry(assetGuid);
    }

    public static void RemoveAllGroups()
    {
        var s = GetSettings();
        var list = s.groups;
        List<AddressableAssetGroup> temp_list = new List<AddressableAssetGroup>();
        for (int i = list.Count - 1; i>=0; i--)
        {
            //默认的group不能删了重建，因为重建GUID变了
            if (list[i].Name != "default")
            {
                temp_list.Add(list[i]);
            }
        }
        for (int i = temp_list.Count - 1; i >= 0; i--)
        {
            s.RemoveGroup(temp_list[i]);
        }
    }

    public static void BuildPlayerContent()
    {
        var d = GetSettings();
        d.ActivePlayerDataBuilderIndex = 3;
        AddressableAssetSettings.BuildPlayerContent();
    }

    public static void CleanPlayerContent()
    {
        AddressableAssetSettings.CleanPlayerContent();
        UnityEditor.Build.Pipeline.Utilities.BuildCache.PurgeCache(false);
    }

    static public void Test()
    {
        var matguid = AssetDatabase.AssetPathToGUID("Assets/Data/hogeMat.mat");
        AddAssetToGroup(matguid, "CreatedGroup");
    }
}