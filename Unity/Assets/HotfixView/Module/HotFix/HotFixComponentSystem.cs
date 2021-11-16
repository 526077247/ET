using IFix.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    public class HotFixComponentAwakeSystem : AwakeSystem<HotFixComponent>
    {
        public override void Awake(HotFixComponent self)
        {
            HotFixComponent.Instance = self;
        }
    }

    public static class HotFixComponentSystem
    {
        public static async ETTask HotFix(this HotFixComponent self)
        {
            var asset = await ResourcesComponent.Instance.LoadTextAsync("Hotfix/HotfixInfo.bytes");
            self.Assemblys = asset.text.Split(',');
            for (int i = 0; i < self.Assemblys.Length; i++)
            {
                if (string.IsNullOrEmpty(self.Assemblys[i])) continue;
                var bytes = await ResourcesComponent.Instance.LoadTextAsync("Hotfix/" + self.Assemblys[i] + ".patch.bytes", ignoreError:true);
                if (bytes != null)
                {
                    Log.Info("Start Patch " + self.Assemblys[i]);
                    PatchManager.Load(new MemoryStream(bytes.bytes));
                }
            }
        } 
    }
}
