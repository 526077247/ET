﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ET
{
    public static class TriggerShapeType
    {
        public const int Sphere = 0;//球
        public const int Cube = 1;//OBB
    }
    /// <summary>
    /// 球形触发器
    /// </summary>
    public class AOITriggerComponent:Entity,IAwake<float,Action<AOIUnitComponent, AOITriggerType>>,IDestroy
    {
        public float Radius;
        public AOITriggerType Flag;
        public List<CampType> Selecter;
        public Action<AOIUnitComponent, AOITriggerType> Handler;
        public int TriggerType = TriggerShapeType.Sphere;//
        public bool IsCollider;
        public DictionaryComponent<AOIGrid, int> DebugMap;
        public ListComponent<string> LogInfo;
        public Vector3 Offset;
    }
}
