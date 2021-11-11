using UnityEngine;
using UnityEngine.UI;

namespace ET
{


	public class UILoadingView : UIBaseView
	{
		public static UILoadingView Instance;
		public override string PrefabPath => "UI/UILoading/Prefabs/UILoadingView.prefab";
		public UILoadingView()
        {
			if (!SceneManagerComponent.Instance.ScenesChangeIgnoreClean.Contains(PrefabPath))
			{
				SceneManagerComponent.Instance.ScenesChangeIgnoreClean.Add(PrefabPath);
				SceneManagerComponent.Instance.DestroyWindowExceptNames.Add(typeof(UILoadingView).Name);
			}
		}
		public UISlider slider;

    }
}
