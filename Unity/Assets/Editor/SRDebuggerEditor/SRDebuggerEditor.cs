using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public class SRDebuggerEditor
    {

        [MenuItem("Tools/SRDebugger/导入SRDebugger", false, 401)]
        public static void AddSRDebugger()
        {
            GameUtility.SafeCopyDir(System.IO.Path.Combine(UnityEngine.Application.dataPath, "..", "..", "SRDebugger"), System.IO.Path.Combine(UnityEngine.Application.dataPath, "StompyRobot"));
        }

        [MenuItem("Tools/SRDebugger/清理SRDebugger", false, 400)]
        public static void ClearSRDebugger()
        {
            string StompyRobotPath = System.IO.Path.Combine(UnityEngine.Application.dataPath, "StompyRobot");
            GameUtility.SafeClearDir(StompyRobotPath);
            if (Directory.Exists(StompyRobotPath))
            {
                Directory.Delete(StompyRobotPath, false);
            }
            string robotmeta = System.IO.Path.Combine(UnityEngine.Application.dataPath, "StompyRobot.meta");
            if (File.Exists(robotmeta))
            {
                File.Delete(robotmeta);
            }
        }
    }
}
