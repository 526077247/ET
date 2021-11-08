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
		UISlider slider;

        public override void OnCreate()
        {
			base.OnCreate();
			Instance = this;
			slider = this.AddComponent<UISlider>("Loadingscreen/Slider");
		}

		public void SetSlidValue(float pro)
        {
			slider.SetValue(pro);
		}
		
        public override void Dispose()
        {
			Instance = null;
			base.Dispose();
        }
    }
}
