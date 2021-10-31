using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace ET
{
    public class AddressableEditor
    {
        [MenuItem("Tools/Addressable/Marked AssetsPackage Addressable(图集模式)", false, 20)]
        public static void RunCheckAssetBundle()
        {
            AddressableTools.RunCheckAssetBundle();
        }


        //[MenuItem("Tools/Addressable/Marked AssetsPackage Addressable(散图模式)", false, 21)]
        //public static void RunCheckAssetBundleWithDiscreteImages()
        //{
        //    var start = DateTime.Now;
        //    AddressableTools.RunCheckAssetBundle(false);
        //    Logger.Log("Marked AssetsPackage Addressable use time:" + (DateTime.Now - start).TotalSeconds);
        //}

        [MenuItem("Tools/Addressable/Build Default Build Script", false, 31)]
        public static void BuildPlayerContent()
        {
            AddressableTools.BuildPlayerContent();
        }


        [MenuItem("Tools/Addressable/Set All Groups To Local_Static", false, 42)]
        public static void SetAllGroupsToLocalStatic()
        {
            AddressableTools.SetAllGroupsToLocalStatic();
        }

        [MenuItem("Tools/Addressable/Set All Groups To Remote_Static", false, 53)]
        public static void SetAllGroupsToRemoteStatic()
        {
            AddressableTools.SetAllGroupsToRemoteStatic();
        }

        [MenuItem("Tools/Addressable/Set All Groups To Remote_NoStatic", false, 64)]
        public static void SetAllGroupsToRemoteNoStatic()
        {
            AddressableTools.SetAllGroupsToRemoteNoStatic();
        }

        [MenuItem("Tools/Addressable/Set All Asset Label", false, 74)]
        public static void SetAllAssetLabel()
        {
            AddressableTools.SetAssetAddressAndLabel();
        }

        [MenuItem("Tools/Addressable/Marked Addressable(选择)", false, 101)]
        [MenuItem("Assets/Marked Addressable(选择)", false, 101)]
        public static void MarkedAddressable()
        {
            AddressableTools.MarkedAddressable();
        }

    }
}
