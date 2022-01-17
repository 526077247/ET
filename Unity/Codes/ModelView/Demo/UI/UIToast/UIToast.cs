using System;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    public class UIToast : Entity,IAwake,IOnCreate,IOnEnable<string>
	{
		public UITextmesh Text;
    }
}
