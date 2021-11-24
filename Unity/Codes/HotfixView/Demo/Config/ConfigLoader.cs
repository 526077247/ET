using AssetBundles;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    public class ConfigLoader : IConfigLoader
    {
        public void GetAllConfigBytes(Dictionary<string, byte[]> output)
        {
            Dictionary<string, TextAsset> keys = AddressablesManager.Instance.LoadAllTextAsset();

            foreach (var kv in keys)
            {
                TextAsset v = kv.Value as TextAsset;
                string key = kv.Key;
                Log.Info(key);
                output[key] = v.bytes;
            }
        }

        public byte[] GetOneConfigBytes(string configName)
        {
            TextAsset v = AddressablesManager.Instance.LoadTextAsset(configName) as TextAsset;
            return v.bytes;
        }
    }
}