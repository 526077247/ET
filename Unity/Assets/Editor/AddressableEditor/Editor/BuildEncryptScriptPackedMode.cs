using AddressableExts;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceProviders;

[CreateAssetMenu(fileName = "BuildEncryptScriptPackMode.asset", menuName = "Addressable Assets/Data Builders/Build Encrypt Script")]
public class BuildEncryptScriptPackedMode : BuildScriptPackedMode
{
    public override string Name
    {
        get { return "Encrypt Build"; }
    }

    /*
      string(8)         signature                                AB文件头标识                         556e 6974 7946 5300 UnityFS
      unt32(4)         version Archive                       version                                   0000 0006   6
      string(6)        bundleVersion                        bundleVersion                       352e 782e 7800  5.x.x
      string(8)         minimumRevision                   AB所需最低版本                    3230 3139 2e33 2e32 2019.3.2
      uint64(8)        size                                          整个AB的大小                         0000 0000 0000 0790 1936
      uint32(4)        compressedBlocksInfoSize      压缩后的BlockInfo大小          0000 0041   65
      uint32 (4)       uncompressedBlocksInfoSize  压缩前的BlockInfo大小          0000 005B   91
      uint32 (4)       flag                                           AB生成的一些标记                 00000043    
  总共47个字节
*/
    protected void EncodeAssetBundleBySetOffset(string bundleName, string filePath)
    {
        int hashCode = bundleName.ToLower().GetHashCode();
        int headLen = 47; //头部长度
        int offset = headLen + Math.Abs(hashCode) % 256;
        byte[] fileData = File.ReadAllBytes(filePath);
        offset = Math.Min(offset, fileData.Length);
        int fileLen = offset + fileData.Length;
        byte[] buffer = new byte[fileLen];
        //拷贝头部到前面去，混淆视听
        Array.Copy(fileData, 0, buffer, 0, headLen);
        //随机产生字节进行填充后续的
        for (int i = headLen; i < offset; i++)
        {
            System.Random ran = new System.Random();
            buffer[i] = (byte)ran.Next(0, 255);
        }
        Array.Copy(fileData, 0, buffer, offset, fileData.Length);
        FileStream fs = File.OpenWrite(filePath);
        fs.Write(buffer, 0, fileLen);
        fs.Close();
    }


    public override void PostProcessBundles(AddressableAssetGroup assetGroup, List<string> buildBundles, List<string> outputBundles, IBundleBuildResults buildResult, ResourceManagerRuntimeData runtimeData, List<ContentCatalogDataEntry> locations, FileRegistry registry, Dictionary<string, ContentCatalogDataEntry> primaryKeyToCatalogEntry, Dictionary<string, string> bundleRenameMap, List<Action> postCatalogUpdateCallbacks)
    {
        var schema = assetGroup.GetSchema<BundledAssetGroupSchema>();
        if (schema == null)
            return;

        var path = schema.BuildPath.GetValue(assetGroup.Settings);
        if (string.IsNullOrEmpty(path))
            return;

        for (int i = 0; i < buildBundles.Count; ++i)
        {
            if (primaryKeyToCatalogEntry.TryGetValue(buildBundles[i], out ContentCatalogDataEntry dataEntry))
            {
                var info = buildResult.BundleInfos[buildBundles[i]];
                var requestOptions = new AssetBundleEncryptRequestOptions
                {
                    Crc = schema.UseAssetBundleCrc ? info.Crc : 0,
                    UseCrcForCachedBundle = schema.UseAssetBundleCrcForCachedBundles,
                    Hash = schema.UseAssetBundleCache ? info.Hash.ToString() : "",
                    ChunkedTransfer = schema.ChunkedTransfer,
                    RedirectLimit = schema.RedirectLimit,
                    RetryCount = schema.RetryCount,
                    Timeout = schema.Timeout,
                    BundleName = Path.GetFileName(info.FileName),
                    BundleSize = GetFileSize(info.FileName)
                };
                dataEntry.Data = requestOptions;

                int extensionLength = Path.GetExtension(outputBundles[i]).Length;
                string[] deconstructedBundleName = outputBundles[i].Substring(0, outputBundles[i].Length - extensionLength).Split('_');
                string reconstructedBundleName = string.Join("_", deconstructedBundleName, 1, deconstructedBundleName.Length - 1) + ".bundle";

                outputBundles[i] = ConstructAssetBundleName(assetGroup, schema, info, reconstructedBundleName);
                dataEntry.InternalId = dataEntry.InternalId.Remove(dataEntry.InternalId.Length - buildBundles[i].Length) + outputBundles[i];
                dataEntry.Keys[0] = outputBundles[i];
                ReplaceDependencyKeys(buildBundles[i], outputBundles[i], locations);

                Debug.Log(outputBundles[i] + "crc = " + requestOptions.Crc + " hash = " + requestOptions.Hash);

                if (!m_BundleToInternalId.ContainsKey(buildBundles[i]))
                    m_BundleToInternalId.Add(buildBundles[i], dataEntry.InternalId);

                if (dataEntry.InternalId.StartsWith("http:\\"))
                    dataEntry.InternalId = dataEntry.InternalId.Replace("http:\\", "http://").Replace("\\", "/");
                if (dataEntry.InternalId.StartsWith("https:\\"))
                    dataEntry.InternalId = dataEntry.InternalId.Replace("https:\\", "https://").Replace("\\", "/");
                dataEntry.InternalId = CheckNameNeedStripped(assetGroup, dataEntry.InternalId);
            }
            else
            {
                Debug.LogWarningFormat("Unable to find ContentCatalogDataEntry for bundle {0}.", outputBundles[i]);
            }

            //获取bundle的保存路径
            var targetPath = Path.Combine(path, outputBundles[i]);
            //var srcPath = Path.Combine(assetGroup.Settings.buildSettings.bundleBuildPath, buildBundles[i]);
            // bundleRenameMap.Add(buildBundles[i], outputBundles[i]);
            //CopyFileWithTimestampIfDifferent(srcPath, targetPath);

            //判断目录是否存在
            if (!Directory.Exists(Path.GetDirectoryName(targetPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath));

            File.Copy(Path.Combine(assetGroup.Settings.buildSettings.bundleBuildPath, buildBundles[i]), targetPath, true);

            var nameWithourHash = CheckNameNeedStripped(assetGroup, outputBundles[i]);
            EncodeAssetBundleBySetOffset(nameWithourHash, targetPath);
            var bundleName = Path.GetFileName(nameWithourHash);
            if (IsBuildInAssetBundle(bundleName))
            {
                string RuntimePath = UnityEngine.AddressableAssets.Addressables.RuntimePath;
                string destPath = Path.Combine(System.Environment.CurrentDirectory, RuntimePath, PlatformMappingService.GetPlatform().ToString(), nameWithourHash);
                if (!Directory.Exists(Path.GetDirectoryName(destPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                if (!File.Exists(destPath))
                {
                    File.Copy(targetPath, destPath);
                }
            }

            AddPostCatalogUpdatesInternal(assetGroup, postCatalogUpdateCallbacks, dataEntry, targetPath);

            registry.AddFile(targetPath);
        }
    }

    //判断assetbundle 是否需要内置到包里面
    //后面控制包体以及皮肤的时候会用到，暂时先全部打进入包里面去
    private bool IsBuildInAssetBundle(string bundleName)
    {
        return true;
    }

    private string CheckNameNeedStripped(AddressableAssetGroup assetGroup, string strname)
    {
        if (assetGroup.GetSchema<BundledAssetGroupSchema>()?.BundleNaming ==
            BundledAssetGroupSchema.BundleNamingStyle.NoHash)
        {
            return StripHashFromBundleLocation(strname);
        }
        return strname;
    }

    protected override TResult DoBuild<TResult>(AddressablesDataBuilderInput builderInput, AddressableAssetsBuildContext aaContext)
    {
        List<string> buildInABHashLines = new List<string>();

        TResult opResult = base.DoBuild<TResult>(builderInput, aaContext);
        var groups = aaContext.Settings.groups;
        for (int i = 0; i < groups.Count; i++)
        {
            List<string> bundles;
            if (aaContext.assetGroupToBundles.TryGetValue(groups[i], out bundles))
            {
                var locations = aaContext.locations;
                for (int j = 0; j < locations.Count; j++)
                {
                    var d = locations[j].Data as AssetBundleRequestOptions;
                    if (d != null)
                    {
                        for (int k = 0; k < bundles.Count; k++)
                        {
                            if (d.BundleName == bundles[k])
                            {
                                if (d.BundleSize > 2097152 || d.BundleSize < 1572864)//大于2M和小于1.5M的
                                {
                                    // Logger.Log(string.Format("the size of group:{0} is {1},bundleName is :{2}", groups[i].name, GetFormatSizeString(d.BundleSize), bundles[k]));
                                }
                                else
                                {
                                    //Logger.Log(string.Format("the size of group:{0} is {1},bundleName is :{2}", groups[i].name, GetFormatSizeString(d.BundleSize), bundles[k]));
                                }
                                if (IsBuildInAssetBundle(groups[i].name))
                                {
                                    buildInABHashLines.Add(d.Hash);
                                }
                            }
                        }
                    }
                }
            }
        }

        //写文件
        string RuntimePath = UnityEngine.AddressableAssets.Addressables.RuntimePath;
        string BuildInABHashInfoFilePath = Path.Combine(System.Environment.CurrentDirectory, RuntimePath, "BuildInABHashFile.bytes");
        //   Logger.Log("BuildInABHashInfoFilePath  = " + BuildInABHashInfoFilePath);
        if (File.Exists(BuildInABHashInfoFilePath))
        {
            File.Delete(BuildInABHashInfoFilePath);
        }
        Debug.Log(buildInABHashLines);
        File.WriteAllLines(BuildInABHashInfoFilePath, buildInABHashLines);

        return opResult;
    }

    private string GetFormatSizeString(long size)
    {
        return GetFormatSizeString(size, 1024);
    }

    private string GetFormatSizeString(long size, int p)
    {
        return GetFormatSizeString(size, p, "#,##0.##");
    }

    private string GetFormatSizeString(long size, int p, string specifier)
    {
        var suffix = new[] { "", "K", "M", "G", "T", "P", "E", "Z", "Y" };
        int index = 0;

        while (size >= p)
        {
            size /= p;
            index++;
        }

        return string.Format(
            "{0}{1}B",
            size.ToString(specifier),
            index < suffix.Length ? suffix[index] : "-"
        );
    }
}
