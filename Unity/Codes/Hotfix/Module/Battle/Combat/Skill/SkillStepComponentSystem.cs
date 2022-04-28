using System.Collections.Generic;
#if NOT_UNITY
using System.IO;
#else
using AssetBundles;
#endif

namespace ET
{
    [ObjectSystem]
    public class SkillStepComponentAwakeSystem : AwakeSystem<SkillStepComponent>
    {
        public override void Awake(SkillStepComponent self)
        {
            SkillStepComponent.Instance = self;
            self.Params = DictionaryComponent<int, List<object[]>>.Create();
            self.StepType = DictionaryComponent<int, List<int>>.Create();
            self.TimeLine = DictionaryComponent<int, List<int>>.Create();
        }
    }
    [ObjectSystem]
    public class SkillStepComponentDestroySystem : DestroySystem<SkillStepComponent>
    {
        public override void Destroy(SkillStepComponent self)
        {
            SkillStepComponent.Instance = null;
            self.Params.Dispose();
            self.StepType.Dispose();
            self.TimeLine.Dispose();
        }
    }

    [FriendClass(typeof(SkillStepComponent))]
    public static class SkillStepComponentSystem
    {
        public static void GetSkillStepInfo(this SkillStepComponent self,int configId, out List<int> timeline,
            out List<int> steptype, out List<object[]> paras)
        {
           
            bool needinit = false;
            if (!self.TimeLine.ContainsKey(configId))
            {
                needinit = true;
                self.TimeLine[configId] = new List<int>();
            }
            timeline = self.TimeLine[configId];
            
            if (!self.StepType.ContainsKey(configId))
            {
                needinit = true;
                self.StepType[configId] = new List<int>();
            }
            steptype = self.StepType[configId];
            
            if (!self.Params.ContainsKey(configId))
            {
                needinit = true;
                self.Params[configId] = new List<object[]>();
            }
            paras = self.Params[configId];
            if (needinit)
            {
                Log.Info("GetSkillStepInfo "+configId);
                var config = SkillConfigCategory.Instance.Get(configId);
                
#if NOT_UNITY
                var text = File.ReadAllText($"../Skill/{config.JsonFile}.json");
#else
                var text = AddressablesManager.Instance.LoadTextAsset($"Skill/Config/{config.JsonFile}.json").text;
#endif
                var list = JsonHelper.FromJson<List<SkillStep>>(text);
                for (int i = 0; i < list.Count; i++)
                {
                    timeline.Add(list[i].Trigger);
                    steptype.Add(list[i].Type);
                    paras.Add(list[i].Params);
                }
            }
        }
    }
}