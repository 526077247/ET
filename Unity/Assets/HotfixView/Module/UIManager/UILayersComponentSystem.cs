using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{

    public class UILayersComponentAwakeSystem : AwakeSystem<UILayersComponent>
    {
        public override void Awake(UILayersComponent self)
        {
			Log.Info("UILayersComponent Awake");
			UILayersComponent.Instance = self;
			self.UIRootPath = "Global/UI";
			self.EventSystemPath = "EventSystem";
			self.UICameraPath = self.UIRootPath + "/UICamera";
			self.gameObject = GameObject.Find(self.UIRootPath);
			var event_system = GameObject.Find(self.EventSystemPath);
			var transform = self.gameObject.transform;
			self.UICamera = GameObject.Find(self.UICameraPath).GetComponent<Camera>();
			GameObject.DontDestroyOnLoad(self.gameObject);
			GameObject.DontDestroyOnLoad(event_system);
			self.Resolution = new Vector2(Define.DesignScreen_Width, Define.DesignScreen_Height);//分辨率
			self.layers = new Dictionary<UILayerNames, UILayer>(UILayerNamesComparer.Instance);
			UILayerDefine[] uILayers = UILayers.GetUILayers();
			for (int i = 0; i < uILayers.Length; i++)
			{
				var layer = uILayers[i];
				var go = new GameObject(layer.Name.ToString())
				{
					layer = 5
				};
				var trans = go.transform;
				trans.SetParent(transform, false);
				UILayer new_layer = self.AddChild<UILayer, UILayerDefine, GameObject>(layer, go);
				self.layers[layer.Name] = new_layer;
				UIManagerComponent.Instance.window_stack[layer.Name] = new LinkedList<string>();
			}

			var flagx = (float)Define.DesignScreen_Width / (Screen.width > Screen.height ? Screen.width : Screen.height);
			var flagy = (float)Define.DesignScreen_Height / (Screen.width > Screen.height ? Screen.height : Screen.width);
			UIManagerComponent.Instance.ScreenSizeflag = flagx > flagy ? flagx : flagy;
		}
    }

    public class UILayersComponentDestroySystem : DestroySystem<UILayersComponent>
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
			Log.Info("UILayersComponent Dispose");
		}

	}

	public static class UILayersComponentSystem
    {
		public static GameObject GetUIRoot(this UIManagerComponent self)
        {
			return UILayersComponent.Instance.gameObject;
		}

		public static Camera GetUICamera(this UIManagerComponent self)
        {
			return UILayersComponent.Instance.UICamera;
		}

		public static GameObject GetUICameraGo(this UIManagerComponent self)
        {
			return UILayersComponent.Instance.UICamera.gameObject;
		}

		public static Vector2 GetResolution(this UIManagerComponent self)
        {
			return UILayersComponent.Instance.Resolution;
		}

		public static void SetNeedTurn(this UIManagerComponent self,bool flag)
        {
			UILayersComponent.Instance.need_turn = flag;
		}

		public static bool GetNeedTurn(this UIManagerComponent self)
        {
			return UILayersComponent.Instance.need_turn;
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
			if(UILayersComponent.Instance.layers.TryGetValue(layer,out var res))
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
