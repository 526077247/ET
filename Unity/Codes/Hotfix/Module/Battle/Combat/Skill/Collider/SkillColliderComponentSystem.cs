namespace ET
{
    public class SkillColliderAwakeSystem : AwakeSystem<SkillColliderComponent, int,CombatUnitComponent>
    {
        public override void Awake(SkillColliderComponent self, int a,CombatUnitComponent from)
        {
            self.ConfigId = a;
            self.From = from;

        }
    }
    
    public static class SkillColliderComponentSystem
    {
        
    }
}