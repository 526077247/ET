﻿using System.Collections.Generic;
using UnityEngine;

namespace ET
{
	public enum MotionType:byte
	{
		None,
		Idle,
		Run,
		Died,
		Damage,
	}

	[ComponentOf(typeof(Unit))]
	public class AnimatorComponent : Entity, IAwake, IUpdate, IDestroy
	{
		public Dictionary<string, AnimationClip> animationClips = new Dictionary<string, AnimationClip>();
		public HashSet<string> Parameter = new HashSet<string>();

		public MotionType MotionType;
		public float MontionSpeed;
		public bool isStop;
		public float stopSpeed;
		public Animator Animator;
	}
}