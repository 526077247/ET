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
			if (!SceneManagerComponent.ScenesChangeIgnoreClean.Contains(PrefabPath))
			{
				SceneManagerComponent.ScenesChangeIgnoreClean.Add(PrefabPath);
				SceneManagerComponent.DestroyWindowExceptNames.Add(typeof(UILoadingView).Name);
			}
		}
		Slider slider;

        public override void OnCreate()
        {
			base.OnCreate();
			Instance = this;
			slider = transform.Find("Loadingscreen/Slider").GetComponent<Slider>();
		}

		public void SetSlidValue(float pro)
        {
			slider.value = pro;
		}
		
        public override void Dispose()
        {
			Instance = null;
			base.Dispose();
        }
    }
}
