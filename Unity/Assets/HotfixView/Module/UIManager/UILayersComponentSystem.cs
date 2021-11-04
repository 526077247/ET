using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{

	public class UIManagerComponentDestroySystem : DestroySystem<UILayersComponent>
	{
		public override void Destroy(UILayersComponent self)
		{
			foreach (var item in self.layers)
			{
				var obj = item.Value.gameObject;
				GameObject.Destroy(obj);
			}
			self.layers.Clear();
			self.layers = null;
			Log.Info("UIManagerComponent Dispose");
		}

	}

	public static class UILayersComponentSystem
    {
		public static GameObject GetUIRoot(this UIManagerComponent self)
        {
			return self.GetComponent<UILayersComponent>().gameObject;
		}

		public static Camera GetUICamera(this UIManagerComponent self)
        {
			return self.GetComponent<UILayersComponent>().UICamera;
		}

		public static GameObject GetUICameraGo(this UIManagerComponent self)
        {
			return self.GetComponent<UILayersComponent>().UICamera.gameObject;
		}

		public static Vector2 GetResolution(this UIManagerComponent self)
        {
			return self.GetComponent<UILayersComponent>().Resolution;
		}

		public static void SetNeedTurn(this UIManagerComponent self,bool flag)
        {
			self.GetComponent<UILayersComponent>().need_turn = flag;
		}

		public static bool GetNeedTurn(this UIManagerComponent self)
        {
			return self.GetComponent<UILayersComponent>().need_turn;
		}
		

		public static UIBaseView GetView(this UIManagerComponent self,string ui_name)
		{
			var res = self.GetWindow(ui_name);
			if (res != null)
            {
				return res.GetComponent<UIBaseView>();
            }
			return null;
		}

		public static UILayer GetLayer(this UIManagerComponent self, UILayerNames layer)
        {
			if(self.GetComponent<UILayersComponent>().layers.TryGetValue(layer,out var res))
            {
				return res;
			}
			return null;
        }

		public static void SetCanvasScaleEditorPortrait(this UILayersComponent self, bool flag)
		{
			self.layers[UILayerNames.GameLayer].SetCanvasScaleEditorPortrait(flag);
			self.layers[UILayerNames.TipLayer].SetCanvasScaleEditorPortrait(flag);
			self.layers[UILayerNames.TopLayer].SetCanvasScaleEditorPortrait(flag);
			self.layers[UILayerNames.GameBackgroudLayer].SetCanvasScaleEditorPortrait(flag);
		}

	}
}
