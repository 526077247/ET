﻿using System;
using UnityEngine;
namespace ET
{

    public class DirectRectSelectComponent:Entity,IAwake,IUpdate,IDestroy,IShow<Action<Vector3>,int[]>,IHide
    {
        public bool IsShow;
        public ETTask<GameObject> waiter;
        public Action<Vector3> OnSelectedCallback { get; set; }
        public GameObject HeroObj;
        public GameObject DirectObj;
        public GameObject gameObject;
        public GameObject AreaObj;
        public int distance;
        public int width;
        public int Mode { get; set; }
    }
}