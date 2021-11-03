using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{

	[ObjectSystem]
	public class UIManagerComponentAwakeSystem : AwakeSystem<UIManagerComponent>
	{
		public override void Awake(UIManagerComponent self)
		{
			Log.Info("UIManagerComponent Awake");
			UIManagerComponent.Instance = self;
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
			self.windows = new Dictionary<string, UIWindow>();
			self.layers = new Dictionary<UILayerNames, UILayer>(UILayerNamesComparer.Instance);
			self.window_stack = new Dictionary<UILayerNames, LinkedList<string>>(UILayerNamesComparer.Instance);
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
				UILayer new_layer = self.AddChild<UILayer, UILayerDefine,GameObject >(layer,go);
				self.layers[layer.Name] = new_layer;
				self.window_stack[layer.Name] = new LinkedList<string>();
			}

			var flagx = (float)Define.DesignScreen_Width / (Screen.width > Screen.height ? Screen.width : Screen.height);
			var flagy = (float)Define.DesignScreen_Height / (Screen.width > Screen.height ? Screen.height : Screen.width);
			self.ScreenSizeflag = flagx > flagy ? flagx : flagy;
		}
	}

    public class UIManagerComponentDestroySystem : DestroySystem<UIManagerComponent>
    {
        public override void Destroy(UIManagerComponent self)
        {
			self.DestroyAllWindow();
			self.windows.Clear();
			self.windows = null;
			self.window_stack.Clear();
			self.window_stack = null;
			foreach (var item in self.layers)
			{
				var obj = item.Value.gameObject;
				GameObject.Destroy(obj);
			}
			self.layers.Clear();
			self.layers = null;
			UIManagerComponent.Instance = null;
			Log.Info("UIManagerComponent Dispose");
		}

	}

    public class DestroyWindowExceptNames_UIManager : AEvent<EventType.DestroyWindowExceptNames>
	{
		protected override async ETTask Run(EventType.DestroyWindowExceptNames args)
		{
			UIManagerComponent.Instance.DestroyWindowExceptNames(args.names);
			await ETTask.CompletedTask;
		}
	}


	/// <summary>
	/// fd: UI管理类，所有UI都应该通过该管理类进行创建 
	/// UIManager.Instance.OpenWindow<T>();
	/// 提供UI操作、UI层级、UI消息、UI资源加载、UI调度、UI缓存等管理
	/// </summary>
	public static class UIManagerComponentSystem
    {
		public static GameObject GetUIRoot(this UIManagerComponent self)
        {
			return self.gameObject;
		}

		public static Camera GetUICamera(this UIManagerComponent self)
        {
			return self.UICamera;
		}

		public static GameObject GetUICameraGo(this UIManagerComponent self)
        {
			return self.UICamera.gameObject;
		}

		public static Vector2 GetResolution(this UIManagerComponent self)
        {
			return self.Resolution;
		}

		public static void SetNeedTurn(this UIManagerComponent self,bool flag)
        {
			self.need_turn = flag;
		}

		public static bool GetNeedTurn(this UIManagerComponent self)
        {
			return self.need_turn;
		}
		/// <summary>
		/// 获取UI窗口
		/// </summary>
		/// <param name="ui_name"></param>
		/// <param name="active">1打开，-1关闭,0不做限制</param>
		/// <param name="view_active">1打开，-1关闭,0不做限制</param>
		/// <returns></returns>
		public static UIWindow GetWindow(this UIManagerComponent self,string ui_name,int active =0, int view_active=0)
        {
			if(self.windows.TryGetValue(ui_name,out var target))
            {
                if (active == 0 || active== (target.Active?1:-1))
                {
					if (view_active == 0 || view_active == ((target.GetComponent(target.ViewType) as UIBaseView).IsActiveSelf() ? 1 : -1))
					{
						return target;
					}
					Debug.Log("Not view_active");
					return null;
				}
				return null;
            }
			return null;
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
			if(self.layers.TryGetValue(layer,out var res))
            {
				return res;
			}
			return null;
        }


		public static void CloseWindow<T>(this UIManagerComponent self)
		{
			string ui_name = typeof(T).Name;
			self.CloseWindow(ui_name);
		}
		public static void CloseWindow(this UIManagerComponent self,string ui_name)
        {
			var target = self.GetWindow(ui_name, 1);
			if (target == null) return;

			self.__RemoveFromStack(target);
			self.__InnnerCloseWindow(target);

		}

		public static void CloseWindowByLayer(this UIManagerComponent self,UILayerDefine layer, string[] except_ui_names = null)
		{
			Dictionary<string, bool> dict_ui_names= null;
			if (except_ui_names != null)
			{
				dict_ui_names = new Dictionary<string, bool>();
				foreach (var item in except_ui_names) dict_ui_names[item] = true;
			}

            foreach (var item in self.windows)
            {
				if (item.Value.Layer == layer.Name && dict_ui_names!=null&&!dict_ui_names.ContainsKey(item.Key))
                {
					self.CloseWindow(item.Key);
				}
            }
        }

		public static void DestroyWindow<T>(this UIManagerComponent self)
		{
			string ui_name = typeof(T).Name;
			self.DestroyWindow(ui_name);
		}
		public static void DestroyWindow(this UIManagerComponent self,string ui_name)
		{
			var target = self.GetWindow(ui_name);
			if (target != null)
			{
				self.CloseWindow(ui_name);
				self.__InnerDestroyWindow(target);
			}
		}

		static void __InnerDestroyWindow(this UIManagerComponent self,UIWindow target)
        {
			UIBaseView view = target.GetComponent(target.ViewType)  as UIBaseView;
			var obj = view?.gameObject;
			if (obj)
			{
				if (GameObjectPoolComponent.Instance == null)
					GameObject.Destroy(obj);
				else
					GameObjectPoolComponent.Instance.RecycleGameObject(obj);
			}
			view?.OnDestroy();
			self.windows.Remove(target.Name);
		}

		/// <summary>
		/// 初始化window
		/// </summary>
		static UIWindow __InitWindow<T>(this UIManagerComponent self, UILayerNames layer_name)where T: UIBaseView, new()
        {
			if(self.layers.ContainsKey(layer_name))
            {
				UIWindow window = self.AddChild<UIWindow>();
				var type = typeof(T);
				window.Name = type.Name;
				var uibaseview = window.AddComponent<T>(true);
				uibaseview.__BaseViewName = type.Name;
				window.Active = false;
				window.ViewType = type;
				window.Layer = layer_name;
				window.PrefabPath = uibaseview.PrefabPath;
				return window;
			}
			Log.Error("No layer named : " + layer_name + ".You should create it first!");
			return null;
        }


		static void __ActivateWindow(this UIManagerComponent self, UIWindow target)
        {
			var view = target.GetComponent(target.ViewType) as UIBaseView;
			var obj = view.gameObject;
			if (obj == null)
            {
				Log.Error("You can only activate window after prefab loaded!");
				return;
            }
			view.SetActive(true);

		}
		static void __ActivateWindow<T>(this UIManagerComponent self, UIWindow target,T p1)
		{
			var view = target.GetComponent(target.ViewType) as UIBaseView;
			var obj = view.gameObject;
			if (obj == null)
			{
				Log.Error("You can only activate window after prefab loaded!");
				return;
			}
			view.SetActive(true, p1);

		}
		static void __ActivateWindow<T,P>(this UIManagerComponent self, UIWindow target,T p1,P p2)
		{
			var view = target.GetComponent(target.ViewType) as UIBaseView;
			var obj = view.gameObject;
			if (obj == null)
			{
				Log.Error("You can only activate window after prefab loaded!");
				return;
			}
			view.SetActive(true, p1, p2);

		}
		static void __ActivateWindow<T,P,K>(this UIManagerComponent self, UIWindow target, T p1, P p2,K p3)
		{
			var view = target.GetComponent(target.ViewType) as UIBaseView;
			var obj = view.gameObject;
			if (obj == null)
			{
				Log.Error("You can only activate window after prefab loaded!");
				return;
			}
			view.SetActive(true, p1, p2, p3);

		}

		static void __Deactivate(this UIManagerComponent self, UIWindow target)
        {
			var view = target.GetComponent(target.ViewType) as UIBaseView;
			var obj = view?.gameObject;
			if (obj == null)
				return;
			view.SetActive(false);
		}

		static async ETTask<T> __InnerOpenWindow<T>(this UIManagerComponent self, UIWindow target,Action<T> callback) where T: UIBaseView
		{
			target.Active = true;
			T res;
			var view = target.GetComponent(target.ViewType) as UIBaseView;
			var need_load = view.gameObject == null;
			if (!need_load)
			{
				self.__AddWindowToStack(target);
				res = view as T;
				callback?.Invoke(res);
				return res;
			}
			else
			{
				if (target.IsLoading)
				{
					//正在加载时 不要再次加载资源了
					while (target.IsLoading)
					{
						await TimerComponent.Instance.WaitAsync(1);
					}
					self.__AddWindowToStack(target);
					res = view as T;
					callback?.Invoke(res);
					return res;
				}
				target.IsLoading = true;
				await self.__CoLoadDependency(target);
				var go = await GameObjectPoolComponent.Instance.GetGameObjectAsync(target.PrefabPath);
				self.OnLoadGameObjectDone(target, go);
				res = view as T;
				callback?.Invoke(res);
				return res;
			}
		}
		static async ETTask<T> __InnerOpenWindow<T,P1>(this UIManagerComponent self, UIWindow target, P1 p1, Action<T> callback) where T : UIBaseView
		{
			target.Active = true;
			T res;
			var view = target.GetComponent(target.ViewType) as UIBaseView;
			var need_load = view.gameObject == null;
			if (!need_load)
			{
				self.__AddWindowToStack(target,p1);
				res = view as T;
				callback?.Invoke(res);
				return res;
			}
			else
			{
				if (target.IsLoading)
				{
					//正在加载时 不要再次加载资源了
					while (target.IsLoading)
					{
						await TimerComponent.Instance.WaitAsync(1);
					}
					self.__AddWindowToStack(target, p1);
					res = view as T;
					callback?.Invoke(res);
					return res;
				}
				target.IsLoading = true;
				await self.__CoLoadDependency(target);
				var go = await GameObjectPoolComponent.Instance.GetGameObjectAsync(target.PrefabPath);
				self.OnLoadGameObjectDone(target, go,p1);
				res = view as T;
				callback?.Invoke(res);
				return res;
			}
		}
		static async ETTask<T> __InnerOpenWindow<T,P1,P2>(this UIManagerComponent self, UIWindow target, P1 p1, P2 p2, Action<T> callback) where T : UIBaseView
		{
			target.Active = true;
			T res;
			var view = target.GetComponent(target.ViewType) as UIBaseView;
			var need_load = view.gameObject == null;
			if (!need_load)
			{
				self.__AddWindowToStack(target, p1,p2);
				res = view as T;
				callback?.Invoke(res);
				return res;
			}
			else
			{
				if (target.IsLoading)
				{
					//正在加载时 不要再次加载资源了
					while (target.IsLoading)
					{
						await TimerComponent.Instance.WaitAsync(1);
					}
					self.__AddWindowToStack(target, p1, p2);
					res = view as T;
					callback?.Invoke(res);
					return res;
				}
				target.IsLoading = true;
				await self.__CoLoadDependency(target);
				var go = await GameObjectPoolComponent.Instance.GetGameObjectAsync(target.PrefabPath);
				self.OnLoadGameObjectDone(target, go, p1, p2);
				res = view as T;
				callback?.Invoke(res);
				return res;
			}
		}
		static async ETTask<T> __InnerOpenWindow<T,P1,P2,P3>(this UIManagerComponent self, UIWindow target, P1 p1, P2 p2, P3 p3, Action<T> callback) where T : UIBaseView
		{
			target.Active = true;
			T res;
			var view = target.GetComponent(target.ViewType) as UIBaseView;
			var need_load = view.gameObject == null;
			if (!need_load)
			{
				self.__AddWindowToStack(target, p1,p2,p3);
				res = view as T;
				callback?.Invoke(res);
				return res;
			}
			else
			{
				if (target.IsLoading)
				{
					//正在加载时 不要再次加载资源了
					while (target.IsLoading)
					{
						await TimerComponent.Instance.WaitAsync(1);
					}
					self.__AddWindowToStack(target, p1, p2, p3);
					res = view as T;
					callback?.Invoke(res);
					return res;
				}
				target.IsLoading = true;
				await self.__CoLoadDependency(target);
				var go = await GameObjectPoolComponent.Instance.GetGameObjectAsync(target.PrefabPath);
				self.OnLoadGameObjectDone(target, go, p1, p2, p3);
				res = view as T;
				callback?.Invoke(res);
				return res;
			}
		}
		//先加载UI依赖的资源
		static async ETTask __CoLoadDependency(this UIManagerComponent self, UIWindow target)
        {
			var reslist = self.__GetWindowPreloadRes(target);
			if (reslist.Count == 0) return;
			using (ListComponent<ETTask> TaskScheduler = ListComponent<ETTask>.Create())
			{
				foreach (var res_path in reslist)
				{
					TaskScheduler.List.Add(GameObjectPoolComponent.Instance.PreLoadGameObjectAsync(res_path, 1));

				}
				await ETTaskHelper.WaitAll(TaskScheduler.List);
			}
		}

		static List<string> __GetWindowPreloadRes(this UIManagerComponent self, UIWindow target)
        {
			List<string> res = new List<string>();
			//加载VIEW_CONFIG里面配置的依赖
			var view = target.GetComponent(target.ViewType) as UIBaseView;
			//允许代码逻辑控制需要增加的依赖
			var res2 = view.OnPreload();
			if (res2 != null)
				res.AddRange(res2);
			return res;
		}
		static void OnLoadGameObjectDone(this UIManagerComponent self, UIWindow target, GameObject go)
        {
            if (go == null)
            {
				Log.Error(string.Format("UIManager InnerOpenWindow {0} faild", target.PrefabPath));
				return;
			}
			var trans = go.transform;
			trans.SetParent(self.layers[target.Layer].transform, false);
			trans.name = target.Name;
			target.IsLoading = false;
			var view = target.GetComponent(target.ViewType) as UIBaseView;
			view.gameObject = go;
			view.OnCreate();
			if (target.Active)
            {
				self.__AddWindowToStack(target);
			}
		}
		static void OnLoadGameObjectDone<P1>(this UIManagerComponent self, UIWindow target, GameObject go, P1 p1)
		{
			if (go == null)
			{
				Log.Error(string.Format("UIManager InnerOpenWindow {0} faild", target.PrefabPath));
				return;
			}
			var trans = go.transform;
			trans.SetParent(self.layers[target.Layer].transform, false);
			trans.name = target.Name;
			target.IsLoading = false;
			var view = target.GetComponent(target.ViewType) as UIBaseView;
			view.gameObject = go;
			view.OnCreate();
			if (target.Active)
			{
				self.__AddWindowToStack(target,p1);
			}
		}
		static void OnLoadGameObjectDone<P1,P2>(this UIManagerComponent self, UIWindow target, GameObject go, P1 p1, P2 p2)
		{
			if (go == null)
			{
				Log.Error(string.Format("UIManager InnerOpenWindow {0} faild", target.PrefabPath));
				return;
			}
			var trans = go.transform;
			trans.SetParent(self.layers[target.Layer].transform, false);
			trans.name = target.Name;
			target.IsLoading = false;
			var view = target.GetComponent(target.ViewType) as UIBaseView;
			view.gameObject = go;
			view.OnCreate();
			if (target.Active)
			{
				self.__AddWindowToStack(target,p1,p2);
			}
		}
		static void OnLoadGameObjectDone<P1,P2,P3>(this UIManagerComponent self, UIWindow target, GameObject go, P1 p1, P2 p2, P3 p3)
		{
			if (go == null)
			{
				Log.Error(string.Format("UIManager InnerOpenWindow {0} faild", target.PrefabPath));
				return;
			}
			var trans = go.transform;
			trans.SetParent(self.layers[target.Layer].transform, false);
			trans.name = target.Name;
			target.IsLoading = false;
			var view = target.GetComponent(target.ViewType) as UIBaseView;
			view.gameObject = go;
			view.OnCreate();
			if (target.Active)
			{
				self.__AddWindowToStack(target,p1,p2,p3);
			}
		}

		static void __InnnerCloseWindow(this UIManagerComponent self, UIWindow target)
		{
			if (target.Active) {
				self.__Deactivate(target);
				target.Active = false;
			}
		}
		//判断窗口是否打开
		static bool IsActiveWindow<T>(this UIManagerComponent self) where T : UIBaseContainer
		{
			string ui_name = typeof(T).Name;
			var target = self.GetWindow(ui_name);
            if (target == null)
            {
				return false;
			}
			return target.Active;
		}

		//打开窗口
		public static async ETTask<T> OpenWindow<T>(this UIManagerComponent self, UILayerNames layer_name = UILayerNames.NormalLayer, Action<T> callback =null) where T:UIBaseView,new()
        {
			string ui_name = typeof(T).Name;
			var target = self.GetWindow(ui_name);
			if (target == null)
			{
				target = self.__InitWindow<T>(layer_name);
				self.windows[ui_name] = target;
			}
            else if(layer_name!= self.layers[target.Layer].Name)
            {
				if (self.layers.TryGetValue(layer_name, out var layer))
				{
					self.layers[target.Layer] = layer;
					var view = target.GetComponent(target.ViewType) as UIBaseView;
					if (view.transform)
						view.transform.SetParent(self.layers[target.Layer].transform, false);
                }
                else
                {
					Log.Error("not layer, name: " + layer_name);
					callback(null);
					return null;
                }
			}
			return await self.__InnerOpenWindow(target, callback);
		}
		//打开窗口
		public static async ETTask<T> OpenWindow<T,P1>(this UIManagerComponent self, P1 p1, UILayerNames layer_name = UILayerNames.NormalLayer, Action<T> callback = null) where T : UIBaseView, new()
		{
			string ui_name = typeof(T).Name;
			var target = self.GetWindow(ui_name);
			if (target == null)
			{
				target = self.__InitWindow<T>(layer_name);
				self.windows[ui_name] = target;
			}
			else if (layer_name != self.layers[target.Layer].Name)
			{
				if (self.layers.TryGetValue(layer_name, out var layer))
				{
					self.layers[target.Layer] = layer;
					var view = target.GetComponent(target.ViewType) as UIBaseView;
					if (view.transform)
						view.transform.SetParent(self.layers[target.Layer].transform, false);
				}
				else
				{
					Log.Error("not layer, name: " + layer_name);
					callback(null);
					return null;
				}
			}
			return await self.__InnerOpenWindow(target, p1, callback);
		}
		//打开窗口
		public static async ETTask<T> OpenWindow<T, P1,P2>(this UIManagerComponent self, P1 p1, P2 p2, UILayerNames layer_name = UILayerNames.NormalLayer, Action<T> callback = null) where T : UIBaseView, new()
		{
			string ui_name = typeof(T).Name;
			var target = self.GetWindow(ui_name);
			if (target == null)
			{
				target = self.__InitWindow<T>(layer_name);
				self.windows[ui_name] = target;
			}
			else if (layer_name != self.layers[target.Layer].Name)
			{
				if (self.layers.TryGetValue(layer_name, out var layer))
				{
					self.layers[target.Layer] = layer;
					var view = target.GetComponent(target.ViewType) as UIBaseView;
					if (view.transform)
						view.transform.SetParent(self.layers[target.Layer].transform, false);
				}
				else
				{
					Log.Error("not layer, name: " + layer_name);
					callback(null);
					return null;
				}
			}
			return await self.__InnerOpenWindow(target, p1,p2, callback);
		}
		//打开窗口
		public static async ETTask<T> OpenWindow<T, P1,P2,P3>(this UIManagerComponent self, P1 p1, P2 p2, P3 p3, UILayerNames layer_name = UILayerNames.NormalLayer, Action<T> callback=null) where T : UIBaseView, new()
		{
			string ui_name = typeof(T).Name;
			var target = self.GetWindow(ui_name);
			if (target == null)
			{
				target = self.__InitWindow<T>(layer_name);
				self.windows[ui_name] = target;
			}
			else if (layer_name != self.layers[target.Layer].Name)
			{
				if (self.layers.TryGetValue(layer_name, out var layer))
				{
					self.layers[target.Layer] = layer;
					var view = target.GetComponent(target.ViewType) as UIBaseView;
					if (view.transform)
						view.transform.SetParent(self.layers[target.Layer].transform, false);
				}
				else
				{
					Log.Error("not layer, name: " + layer_name);
					callback(null);
					return null;
				}
			}
			return await self.__InnerOpenWindow(target, p1,p2,p3, callback);
		}
		public static void SetCanvasScaleEditorPortrait(this UIManagerComponent self, bool flag)
        {
			self.layers[UILayers.GameLayer.Name].SetCanvasScaleEditorPortrait(flag);
			self.layers[UILayers.TipLayer.Name].SetCanvasScaleEditorPortrait(flag);
			self.layers[UILayers.TopLayer.Name].SetCanvasScaleEditorPortrait(flag);
			self.layers[UILayers.GameBackgroudLayer.Name].SetCanvasScaleEditorPortrait(flag);
		}
		static void __AddWindowToStack(this UIManagerComponent self, UIWindow target)
        {
			var ui_name = target.Name;
			var layer_name = self.layers[target.Layer].Name;
			bool isFirst = false;
            if (self.window_stack[layer_name].Contains(ui_name))
            {
				isFirst = true;
				self.window_stack[layer_name].Remove(ui_name);
			}
			self.window_stack[layer_name].AddFirst(ui_name);
			var view = target.GetComponent(target.ViewType) as UIBaseView;
			if (view.transform)
			{
				view.transform.SetAsLastSibling();
				self.__ActivateWindow(target);
			}
			if(isFirst && layer_name == UILayers.BackgroudLayer.Name || layer_name == UILayers.GameBackgroudLayer.Name)
            {
				//如果是背景layer，则销毁所有的normal层|BackgroudLayer
				self.CloseWindowByLayer(UILayers.NormalLayer);
				self.CloseWindowByLayer(UILayers.GameLayer);
				self.CloseWindowByLayer(UILayers.BackgroudLayer, new string[]{ ui_name});
				self.CloseWindowByLayer(UILayers.GameBackgroudLayer, new string[] { ui_name});
			}
		}
		static void __AddWindowToStack<P1>(this UIManagerComponent self, UIWindow target,P1 p1)
		{
			var ui_name = target.Name;
			var layer_name = self.layers[target.Layer].Name;
			bool isFirst = false;
			if (self.window_stack[layer_name].Contains(ui_name))
			{
				isFirst = true;
				self.window_stack[layer_name].Remove(ui_name);
			}
			self.window_stack[layer_name].AddFirst(ui_name);
			var view = target.GetComponent(target.ViewType) as UIBaseView;
			if (view.transform)
			{
				view.transform.SetAsLastSibling();
				self.__ActivateWindow(target, p1);
			}
			if (isFirst && layer_name == UILayers.BackgroudLayer.Name || layer_name == UILayers.GameBackgroudLayer.Name)
			{
				//如果是背景layer，则销毁所有的normal层|BackgroudLayer
				self.CloseWindowByLayer(UILayers.NormalLayer);
				self.CloseWindowByLayer(UILayers.GameLayer);
				self.CloseWindowByLayer(UILayers.BackgroudLayer, new string[] { ui_name });
				self.CloseWindowByLayer(UILayers.GameBackgroudLayer, new string[] { ui_name });
			}
		}
		static void __AddWindowToStack<P1, P2>(this UIManagerComponent self, UIWindow target, P1 p1, P2 p2)
		{
			var ui_name = target.Name;
			var layer_name = self.layers[target.Layer].Name;
			bool isFirst = false;
			if (self.window_stack[layer_name].Contains(ui_name))
			{
				isFirst = true;
				self.window_stack[layer_name].Remove(ui_name);
			}
			self.window_stack[layer_name].AddFirst(ui_name);
			var view = target.GetComponent(target.ViewType) as UIBaseView;
			if (view.transform)
			{
				view.transform.SetAsLastSibling();
				self.__ActivateWindow(target, p1,p2);
			}
			if (isFirst && layer_name == UILayers.BackgroudLayer.Name || layer_name == UILayers.GameBackgroudLayer.Name)
			{
				//如果是背景layer，则销毁所有的normal层|BackgroudLayer
				self.CloseWindowByLayer(UILayers.NormalLayer);
				self.CloseWindowByLayer(UILayers.GameLayer);
				self.CloseWindowByLayer(UILayers.BackgroudLayer, new string[] { ui_name });
				self.CloseWindowByLayer(UILayers.GameBackgroudLayer, new string[] { ui_name });
			}
		}
		static void __AddWindowToStack<P1, P2,P3>(this UIManagerComponent self, UIWindow target, P1 p1, P2 p2,P3 p3)
		{
			var ui_name = target.Name;
			var layer_name = self.layers[target.Layer].Name;
			bool isFirst = false;
			if (self.window_stack[layer_name].Contains(ui_name))
			{
				isFirst = true;
				self.window_stack[layer_name].Remove(ui_name);
			}
			self.window_stack[layer_name].AddFirst(ui_name);
			var view = target.GetComponent(target.ViewType) as UIBaseView;
			if (view.transform)
			{
				view.transform.SetAsLastSibling();
				self.__ActivateWindow(target, p1,p2,p3);
			}
			if (isFirst && layer_name == UILayers.BackgroudLayer.Name || layer_name == UILayers.GameBackgroudLayer.Name)
			{
				//如果是背景layer，则销毁所有的normal层|BackgroudLayer
				self.CloseWindowByLayer(UILayers.NormalLayer);
				self.CloseWindowByLayer(UILayers.GameLayer);
				self.CloseWindowByLayer(UILayers.BackgroudLayer, new string[] { ui_name });
				self.CloseWindowByLayer(UILayers.GameBackgroudLayer, new string[] { ui_name });
			}
		}
		static void __RemoveFromStack(this UIManagerComponent self, UIWindow target)
		{
			var ui_name = target.Name;
			var layer_name = self.layers[target.Layer].Name;
			if (self.window_stack.ContainsKey(layer_name))
			{
				self.window_stack[layer_name].Remove(ui_name);
			}
            else
            {
				Log.Error("not layer, name :" + layer_name);
            }
		}
		//销毁指定窗口所有窗口
		public static void DestroyWindowExceptNames(this UIManagerComponent self, string[] type_names = null)
		{
			Dictionary<string, bool> dict_ui_names = new Dictionary<string, bool>();
			if (type_names != null)
			{
				for (int i = 0; i < type_names.Length; i++)
				{
					dict_ui_names[type_names[i]] = true;
				}
			}
			var keys = self.windows.Keys.ToArray();
			for (int i = self.windows.Count - 1; i >= 0; i--)
			{
				if (!dict_ui_names.ContainsKey(keys[i]))
				{
					self.DestroyWindow(keys[i]);
				}
			}
		}
		//销毁指定层级外层级所有窗口
		public static void DestroyWindowExceptLayer(this UIManagerComponent self, UILayerDefine layer)
        {
			var keys = self.windows.Keys.ToArray();
			for (int i = self.windows.Count-1; i >= 0; i--)
            {
				if (self.windows[keys[i]].Layer != layer.Name)
				{
					self.DestroyWindow(keys[i]);
				}
			}
        }
		//销毁层级所有窗口
		public static void DestroyWindowByLayer(this UIManagerComponent self, UILayerDefine layer)
		{
			var keys = self.windows.Keys.ToArray();
			for (int i = self.windows.Count - 1; i >= 0; i--)
			{
				if (self.windows[keys[i]].Layer == layer.Name)
				{
					self.DestroyWindow(self.windows[keys[i]].Name);
				}
			}
		}
		public static void DestroyAllWindow(this UIManagerComponent self)
        {
			var keys = self.windows.Keys.ToArray();
			for (int i = self.windows.Count - 1; i >= 0; i--)
			{
				self.DestroyWindow(self.windows[keys[i]].Name);
			}
		}

        
	}
}
