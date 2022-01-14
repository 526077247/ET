using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	public class UIHelpWin : Entity, IAwake
	{
		public UIText text;
		
		public static string PrefabPath => "UI/UIHelp/Prefabs/UIHelpWin.prefab";
	}
}
