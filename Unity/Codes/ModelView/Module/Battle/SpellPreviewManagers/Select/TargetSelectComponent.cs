﻿using System;
using UnityEngine;
using UnityEngine.UI;
namespace ET
{

    public class TargetSelectComponent:Entity,IAwake,IDestroy,IUpdate,IShow<Action<Unit>,int[]>,IHide
    {
        public ETTask<GameObject> waiter;
        public Action<Unit> OnSelectTargetCallback { get; set; }
        public int TargetLimitType { get; set; }
        public Color CursorColor { get; set; }
        public GameObject HeroObj;
        public GameObject RangeCircleObj;
        public Image CursorImage;
        public GameObject gameObject;
        public bool IsShow;
        public int distance;

    }
}