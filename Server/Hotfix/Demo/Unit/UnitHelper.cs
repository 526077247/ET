using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ET
{
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(MoveComponent))]
    [FriendClass(typeof(NumericComponent))]
    [FriendClass(typeof(BuffComponent))]
    [FriendClass(typeof(Buff))]
    public static class UnitHelper
    {
        public static UnitInfo CreateUnitInfo(Unit unit)
        {
            UnitInfo unitInfo = new UnitInfo();
            
            unitInfo.UnitId = unit.Id;
            unitInfo.ConfigId = unit.ConfigId;
            unitInfo.Type = (int)unit.Type;
            Vector3 position = unit.Position;
            unitInfo.X = position.x;
            unitInfo.Y = position.y;
            unitInfo.Z = position.z;
            Vector3 forward = unit.Forward;
            unitInfo.ForwardX = forward.x;
            unitInfo.ForwardY = forward.y;
            unitInfo.ForwardZ = forward.z;

            #region 移动信息
            MoveComponent moveComponent = unit.GetComponent<MoveComponent>();
            if (moveComponent != null)
            {
                if (!moveComponent.IsArrived())
                {
                    unitInfo.MoveInfo = new MoveInfo();
                    for (int i = moveComponent.N; i < moveComponent.Targets.Count; ++i)
                    {
                        Vector3 pos = moveComponent.Targets[i];
                        unitInfo.MoveInfo.X.Add(pos.x);
                        unitInfo.MoveInfo.Y.Add(pos.y);
                        unitInfo.MoveInfo.Z.Add(pos.z);
                    }
                }
            }
            

            #endregion

            #region 数值信息

            NumericComponent nc = unit.GetComponent<NumericComponent>();
            if(nc!=null)
            {
                foreach ((int key, long value) in nc.NumericDic)
                {
                    unitInfo.Ks.Add(key);
                    unitInfo.Vs.Add(value);
                }
            }
            #endregion

            #region 战斗数据

            var cuc = unit.GetComponent<CombatUnitComponent>();
            if (cuc != null)
            {
                unitInfo.SkillIds = cuc.IdSkills.Keys.ToList();
                var buffC = cuc.GetComponent<BuffComponent>();
                if (buffC != null)
                {
                    unitInfo.BuffIds = new List<int>();
                    unitInfo.BuffTimestamp = new List<long>();
                    foreach (var item in buffC.Groups)
                    {
                        var buff = item.Value;
                        unitInfo.BuffIds.Add(buff.ConfigId);
                        unitInfo.BuffTimestamp.Add(buff.Timestamp);
                    }
                }
            }
            
            #endregion
           
            
            return unitInfo;
        }
        
        /// <summary>
        /// 获取看见unit的玩家，主要用于广播,注意不能Dispose
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static List<AOIUnitComponent> GetBeSeeUnits(this Unit self)
        {
            return self.GetComponent<AOIUnitComponent>().GetBeSeeUnits();
        }
        
        public static void NoticeUnitAdd(Unit unit, Unit sendUnit)
        {
            M2C_CreateUnits createUnits = new M2C_CreateUnits();
            createUnits.Units.Add(CreateUnitInfo(sendUnit));
            MessageHelper.SendToClient(unit, createUnits);
        }
        
        public static void NoticeUnitRemove(Unit unit, Unit sendUnit)
        {
            M2C_RemoveUnits removeUnits = new M2C_RemoveUnits();
            removeUnits.Units.Add(sendUnit.Id);
            MessageHelper.SendToClient(unit, removeUnits);
        }
    }
}