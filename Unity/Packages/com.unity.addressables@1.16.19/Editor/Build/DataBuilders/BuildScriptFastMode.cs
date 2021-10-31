using System;
using System.IO;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.Initialization;
using System.Security.Cryptography;
using System.Text;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

namespace UnityEditor.AddressableAssets.Build.DataBuilders
{
    /// <summary>
    /// Only saves the guid of the settings asset to PlayerPrefs.  All catalog data is generated directly from the settings as needed.
    /// </summary>
    [CreateAssetMenu(fileName = nameof(BuildScriptFastMode) + ".asset", menuName = "Addressables/Content Builders/Use Asset Database (fastest)")]
    public class BuildScriptFastMode : BuildScriptBase
    {
        /// <inheritdoc />
        public override string Name
        {
            get
            {
                return "Use Asset Database (fastest)";
            }
        }

        private bool m_DataBuilt;

        /// <inheritdoc />
        public override void ClearCachedData()
        {
            m_DataBuilt = false;
        }

        /// <inheritdoc />
        public override bool IsDataBuilt()
        {
            return m_DataBuilt;
        }

        /// <inheritdoc />
        protected override string ProcessGroup(AddressableAssetGroup assetGroup, AddressableAssetsBuildContext aaContext)
        {
            return string.Empty;
        }

        /// <inheritdoc />
        public override bool CanBuildData<T>()
        {
            return typeof(T).IsAssignableFrom(typeof(AddressablesPlayModeBuildResult));
        }

        /// <inheritdoc />
        protected override TResult BuildDataImplementation<TResult>(AddressablesDataBuilderInput builderInput)
        {
            if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(builderInput.AddressableSettings, out var guid, out long _))
            {
                IDataBuilderResult res = new AddressablesPlayModeBuildResult() { Error = "Invalid Settings asset." };
                return (TResult)res;
            }
            else
            {
                PlayerPrefs.SetString(Addressables.kAddressablesRuntimeDataPath, $"GUID:{guid}");
                IDataBuilderResult res = new AddressablesPlayModeBuildResult() { OutputPath = "", Duration = 0 };
                m_DataBuilt = true;
                return (TResult)res;
            }
        }

        /// <summary>
        /// Loops over each group, after doing some data checking.
        /// </summary>
        /// <param name="aaContext">The Addressables builderInput object to base the group processing on</param>
        /// <returns>An error string if there were any problems processing the groups</returns>
        protected override string ProcessAllGroups(AddressableAssetsBuildContext aaContext)
        {
            if (aaContext == null ||
                aaContext.Settings == null ||
                aaContext.Settings.groups == null)
            {
                return "No groups found to process in build script " + Name;
            }
            //intentionally for not foreach so groups can be added mid-loop.
            for (int index = 0; index < aaContext.Settings.groups.Count; index++)
            {
                AddressableAssetGroup assetGroup = aaContext.Settings.groups[index];
                if (assetGroup == null)
                    continue;

                ///group差异化
            /*    string group_md5 = EditorUserSettings.GetConfigValue(assetGroup.Name);
                try
                {
                    FileStream file = new FileStream(Path.Combine(Application.dataPath, "AddressableAssetsData", "AssetGroups", assetGroup.Name + ".asset"), FileMode.Open);
                    MD5 md5 = new MD5CryptoServiceProvider();
                    byte[] retval = md5.ComputeHash(file);
                    file.Close();

                    StringBuilder sc = new StringBuilder();
                    for (int i = 0; i < retval.Length; i++)
                    {
                        sc.Append(retval[i].ToString("x2"));
                    }
                    //Debug.LogError(assetGroup.Name + "文件MD5：" + sc);
                    if (group_md5 == sc.ToString())
                    {
                        continue;
                    }
                    UnityEngine.Debug.LogError(assetGroup.Name + " 有差异!");
                    EditorUserSettings.SetConfigValue(assetGroup.Name, sc.ToString());
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError(ex.Message);
                }
                */

                if (assetGroup.Schemas.Find((x) => x.GetType() == typeof(PlayerDataGroupSchema)) &&
                   assetGroup.Schemas.Find((x) => x.GetType() == typeof(BundledAssetGroupSchema)))
                {
                    EditorUtility.ClearProgressBar();
                    return $"Addressable group {assetGroup.Name} cannot have both a {typeof(PlayerDataGroupSchema).Name} and a {typeof(BundledAssetGroupSchema).Name}";
                }

                EditorUtility.DisplayProgressBar($"Processing Addressable Group", assetGroup.Name, (float)index / aaContext.Settings.groups.Count);
                var errorString = ProcessGroup(assetGroup, aaContext);
                if (!string.IsNullOrEmpty(errorString))
                {
                    EditorUtility.ClearProgressBar();
                    return errorString;
                }
            }

            EditorUtility.ClearProgressBar();
            return string.Empty;
        }
    }
}
