using UnityEngine;
using System.Collections.Generic;
namespace ET
{
    public class SkillPara
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public CombatUnitComponent From;
        public CombatUnitComponent To;
        public List<int> CostId;
        public List<int> Cost;
        public SkillAbility Ability;

        #region 步骤参数
        
        public int Index;
        public object[] Paras;
        public int Interval;
        

        #endregion

        public SkillPara()
        {
            CostId = new List<int>();
            Cost = new List<int>();
        }
        public void Clear()
        {
            Position=Vector3.zero;
            Rotation = Quaternion.identity;
            From = null;
            To = null;
            CostId.Clear();
            Cost.Clear();
            Ability = null;
            Index = -1;
            Paras = null;
            Interval = 0;
        }

    }
}