using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace ET
{
    
    public class PointSelectComponent : Entity,IAwake,IUpdate,IDestroy,IShow<Action<Vector3>,int[]>,IHide
    {
        public ETTask<GameObject> waiter;
        public Action<Vector3> OnSelectPointCallback { get; set; }
        public GameObject HeroObj;
        public GameObject RangeCircleObj;
        public GameObject SkillPointObj;
        public GameObject gameObject;
        public bool IsShow;
        public int distance;
        public int range;
        public int Mode { get; set; }
    }
}