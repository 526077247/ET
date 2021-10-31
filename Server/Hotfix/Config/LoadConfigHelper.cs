using System.Collections.Generic;
using System.IO;

namespace ET
{
    public class LoadConfigHelper
    {
        public static Dictionary<string, byte[]> LoadAllConfigBytes()
        {
            Dictionary<string, byte[]> output = new Dictionary<string, byte[]>();
            foreach (string file in Directory.GetFiles($"../Config", "*.bytes"))
            {
                string key = Path.GetFileNameWithoutExtension(file);
                output[key] = File.ReadAllBytes(file);
            }
            return output;
        }
        
        public static byte[] GetOneConfigBytes(string configName)
        {
            byte[] configBytes = File.ReadAllBytes($"../Config/{configName}.bytes");
            return configBytes;
        }
    }
}