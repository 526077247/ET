using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    public static class LoadConfigHelper
    {
        public static async ETTask<Dictionary<string, byte[]>> LoadAllConfigBytes()
        {
            var output = new Dictionary<string, byte[]>();
            var jsonstr = (await ResourcesComponent.Instance.LoadTextAsync("Config/ConfigPaths.json")).text;
            var name = JsonHelper.FromJson<string[]>(jsonstr);
            for (int i = 0; i < name.Length; i++)
            {
                output[name[i]] = (await ResourcesComponent.Instance.LoadTextAsync("Config/" + name[i] + ".bytes")).bytes;
            }
            return output;
        }
    }
}