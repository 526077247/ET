namespace ET
{
    public enum CampType
    {
        Player = 0,//玩家
        Monster, //怪物
        NeutralNPC, //中立NPC(不能被任何人或怪攻击)

        SkillCollider,//技能碰撞器
        //上面添加了下面依次加一
        MAX, //最大值
        ALL, //全部
    }
}