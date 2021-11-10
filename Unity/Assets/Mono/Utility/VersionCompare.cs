using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    public static class VersionCompare
    {
        public static int Compare(string sourceVersion, string targetVersion)
        {
            string[] sVerList = sourceVersion.Split('.');
            string[] tVerList = targetVersion.Split('.');

            if (sVerList.Length >= 3 && tVerList.Length >= 3)
            {
                try
                {
                    int sV0 = int.Parse(sVerList[0]);
                    int sV1 = int.Parse(sVerList[1]);
                    int sV2 = int.Parse(sVerList[2]);
                    int tV0 = int.Parse(tVerList[0]);
                    int tV1 = int.Parse(tVerList[1]);
                    int tV2 = int.Parse(tVerList[2]);

                    if (tV0 > sV0)
                    {
                        return -1;
                    }
                    else if (tV0 < sV0)
                    {
                        return 1;
                    }

                    if (tV1 > sV1)
                    {
                        return -1;
                    }
                    else if (tV1 < sV1)
                    {
                        return 1;
                    }

                    if (tV2 > sV2)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(string.Format("parse version error. clientversion: {0} serverversion: {1}\n {2}\n{3}", sourceVersion, targetVersion, ex.Message, ex.StackTrace));
                    return 1;
                }
            }

            return 0;
        }
    }
}
