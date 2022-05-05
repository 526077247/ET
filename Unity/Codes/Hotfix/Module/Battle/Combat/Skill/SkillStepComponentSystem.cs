using System.Collections.Generic;
using UnityEngine;
#if NOT_UNITY
using System.IO;
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
    [FriendClass(typeof(SkillAbility))]
    public static class SkillStepComponentSystem
    {
        public static 
#if !NOT_UNITY
                async ETTask 
#else
                void
#endif
                GetSkillStepInfo(this SkillStepComponent self,int configId,SkillAbility skill)
        {
           
            bool needinit = false;
            if (!self.TimeLine.ContainsKey(configId))
            {
                needinit = true;
                self.TimeLine[configId] = new List<int>();
            }
            skill.TimeLine = self.TimeLine[configId];
            
            if (!self.StepType.ContainsKey(configId))
            {
                needinit = true;
                self.StepType[configId] = new List<int>();
            }
            skill.StepType = self.StepType[configId];
            
            if (!self.Params.ContainsKey(configId))
            {
                needinit = true;
                self.Params[configId] = new List<object[]>();
            }
            skill.Paras = self.Params[configId];
            if (needinit)
            {
                Log.Info("GetSkillStepInfo "+configId);
                var config = SkillConfigCategory.Instance.Get(configId);
                
#if NOT_UNITY
                var text = File.ReadAllText($"../Skill/{config.JsonFile}.json");
#else
                var text = (await ResourcesComponent.Instance.LoadAsync<TextAsset>($"Skill/Config/{config.JsonFile}.json")).text;
#endif
                var list = JsonHelper.FromJson<List<SkillStep>>(text);
                for (int i = 0; i < list.Count; i++)
                {
                    self.TimeLine[configId].Add(list[i].Trigger);
                    self.StepType[configId].Add(list[i].Type);
                    self.Params[configId].Add(list[i].Params);
                }
            }
        }
    }
}