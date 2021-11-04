using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    public class AfterUIManagerCreate_CreateUILayerManager : AEvent<UIEventType.AfterUIManagerCreate>
    {
        protected override async ETTask Run(UIEventType.AfterUIManagerCreate args)
        {
            var self = UIManagerComponent.Instance.AddComponent<UILayersComponent>();
			Log.Info("UIManagerComponent Awake");
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
}
