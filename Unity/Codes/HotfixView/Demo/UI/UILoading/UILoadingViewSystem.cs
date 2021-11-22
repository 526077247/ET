using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	[UISystem]
	public class UILoadingViewOnCreateSystem : OnCreateSystem<UILoadingView>
	{
		public override void OnCreate(UILoadingView self)
		{
			UILoadingView.Instance = self;
			self.slider = self.AddComponent<UISlider>("Loadingscreen/Slider");
		}
	}
	[UISystem]
	public class UILoadingViewOnDestroySystem : OnDestroySystem<UILoadingView>
	{
		public override void OnDestroy(UILoadingView self)
		{
			UILoadingView.Instance = null;
		}
	}

	public static class UILoadingViewSystem
	{
		public static void SetSlidValue(this UILoadingView self, float pro)
        {
			self.slider.SetValue(pro);
		}
	
    }
}
