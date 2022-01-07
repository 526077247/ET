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
        [MenuItem("Tools/Addressable/Marked AssetsPackage Addressable", false, 20)]
        public static void RunCheckAssetBundle()
        {
            AddressableTools.RunCheckAssetBundle();
        }

        [MenuItem("Tools/Addressable/Build Default Build Script", false, 31)]
        public static void BuildPlayerContent()
        {
            AddressableTools.BuildPlayerContent();
        }
        
        [MenuItem("Assets/Marked Addressable", false, 101)]
        public static void MarkedAddressable()
        {
            AddressableTools.MarkedAddressable();
        }

    }
}
