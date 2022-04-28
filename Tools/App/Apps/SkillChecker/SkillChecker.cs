using System;
using System.IO;
using System.Collections.Generic;
namespace ET
{
    public static class SkillChecker
    {
        private const string ClientDir = "../Unity/Assets/AssetsPackage/Skill/Config";
        private const string ServerDir = "../Skill";

        public static void Export()
        {
            if (!Directory.Exists(ClientDir))
            {
                Directory.CreateDirectory(ClientDir);
            }
            foreach (string jsonPath in AttrExporter.FindFile(ServerDir))
            {
                if (!jsonPath.EndsWith(".json") || jsonPath.Contains("#"))
                {
                    continue;
                }

                try
                {
                    JsonHelper.FromJson<List<SkillStep>>(File.ReadAllText(jsonPath));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    continue;
                }

                string fileName = Path.GetFileName(jsonPath);
                File.Copy(jsonPath,ClientDir+"/"+fileName,true);
            }
        }
    }
}