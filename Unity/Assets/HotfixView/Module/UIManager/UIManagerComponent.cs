using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
	public class UIWindow
	{
		/// <summary>
		/// 窗口名字
		/// </summary>
		public string Name;
		/// <summary>
		/// Layer层级
		/// </summary>
		public UILayer Layer;
		/// <summary>
		///  View实例
		/// </summary>
		public UIBaseView View;
		/// <summary>
		/// 是否激活
		/// </summary>
		public bool Active;
		/// <summary>
		/// 是否正在加载
		/// </summary>
		public bool IsLoading;
		/// <summary>
		/// 预制体路径
		/// </summary>
		public string PrefabPath;
	}
	public class UILayerDefine
    {
		public string Name;
		public int PlaneDistance;
		public int OrderInLayer;
	}
	public enum UILayerNames
    {
		GameBackgroudLayer,
		BackgroudLayer,
		GameLayer,
		SceneLayer,
		NormalLayer,
		TipLayer ,
		TopLayer,
	}
	//UILayers配置
	//做了横竖屏适配，见UIManager的 SetCanvasScaleEditorPortrait 方法
	public static class UILayers
	{
		static Dictionary<string, UILayerDefine> layers;
		// 游戏内的背景层
		public static readonly UILayerDefine GameBackgroudLayer = new UILayerDefine
		{
			Name = "GameBackgroudLayer",
			PlaneDistance = 1000,
			OrderInLayer = 0,
		};
		
		//主界面、全屏的一些界面
		public static readonly UILayerDefine BackgroudLayer = new UILayerDefine
		{
			Name = "BackgroudLayer",
			PlaneDistance = 900,
			OrderInLayer = 1000,
		};

		//游戏内的View层
		public static readonly UILayerDefine GameLayer = new UILayerDefine
		{
			Name = "GameLayer",
			PlaneDistance = 800,
			OrderInLayer = 1800,
		};
		// 场景UI，如：点击建筑查看建筑信息---一般置于场景之上，界面UI之下
		public static readonly UILayerDefine SceneLayer = new UILayerDefine
		{
			Name = "SceneLayer",
			PlaneDistance = 700,
			OrderInLayer = 2000,
		};
		//普通UI，一级、二级、三级等窗口---一般由用户点击打开的多级窗口
		public static readonly UILayerDefine NormalLayer = new UILayerDefine
		{
			Name = "NormalLayer",
			PlaneDistance = 600,
			OrderInLayer = 3000,
		};
		//提示UI，如：错误弹窗，网络连接弹窗等
		public static readonly UILayerDefine TipLayer = new UILayerDefine
		{
			Name = "TipLayer",
			PlaneDistance = 500,
			OrderInLayer = 4000,
		};
		//顶层UI，如：场景加载
		public static readonly UILayerDefine TopLayer = new UILayerDefine
		{
			Name = "TopLayer",
			PlaneDistance = 400,
			OrderInLayer = 5000,
		};
		static UILayers()
        {
			layers = new Dictionary<string, UILayerDefine>()
			{
				{ "GameBackgroudLayer",GameBackgroudLayer },
				{ "BackgroudLayer",BackgroudLayer },
				{ "GameLayer",GameLayer },
				{ "SceneLayer",SceneLayer },
				{ "NormalLayer",NormalLayer },
				{ "TipLayer",TipLayer },
				{ "TopLayer",TopLayer },
			};
		}
		public static UILayerDefine[] GetUILayers()
        {
			return layers.Values.ToArray();
		}

		public static UILayerDefine GetUILayer(string name)
        {
			if(layers.TryGetValue(name,out var res)){
				return res;
            }
			return new UILayerDefine();
		}
	}

    [ObjectSystem]
	public class UIManagerComponentAwakeSystem : AwakeSystem<UIManagerComponent>
	{
		public override void Awake(UIManagerComponent self)
		{
			self.Awake();
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
	public class UIManagerComponent :Entity
    {
		string UIRootPath;//UIRoot路径
		string EventSystemPath;// EventSystem路径
		string UICameraPath;// UICamera路径
		public static UIManagerComponent Instance;

		Dictionary<string, UIWindow> windows;//所有存活的窗体  {ui_name:window}
		Dictionary<string, UILayer> layers;//所有可用的层级
		Dictionary<string,LinkedList<string>> __window_stack;//窗口记录队列

		GameObject gameObject;//UIRoot游戏物体
		Transform transform;//UIRoot的transform
		bool need_turn;
		public Camera UICamera;
		public Vector2 Resolution;
		public int MaxOderPerWindow = 10;
		private float screen_sizeflag = -1;
		public float ScreenSizeflag
		{
            get
            {
				if (screen_sizeflag < 0)
				{
					var flagx = (float)Define.DesignScreen_Width / (Screen.width > Screen.height ? Screen.width : Screen.height);
					var flagy = (float)Define.DesignScreen_Height / (Screen.width > Screen.height ? Screen.height : Screen.width);
					screen_sizeflag = flagx > flagy ? flagx : flagy;
				}
				return screen_sizeflag;
			}
        }
		public void Awake()
		{
			Log.Info("UIManagerComponent Awake");
			Instance = this;
			UIRootPath = "Global/UI";
			EventSystemPath = "EventSystem";
			UICameraPath = UIRootPath + "/UICamera";
			gameObject = GameObject.Find(UIRootPath);
			var event_system = GameObject.Find(EventSystemPath);
			transform = gameObject.transform;
			UICamera = GameObject.Find(UICameraPath).GetComponent<Camera>();
			GameObject.DontDestroyOnLoad(gameObject);
			GameObject.DontDestroyOnLoad(event_system);
			Resolution = new Vector2(Define.DesignScreen_Width, Define.DesignScreen_Height);//分辨率
			windows = new Dictionary<string, UIWindow>();
			layers = new Dictionary<string, UILayer>();
			__window_stack = new Dictionary<string, LinkedList<string>>();
			UILayerDefine[] uILayers = UILayers.GetUILayers();
            for (int i = 0; i < uILayers.Length; i++)
            {
				var layer = uILayers[i];
				var go = new GameObject(layer.Name);
				go.layer = 5;
				var trans = go.transform;
				trans.SetParent(transform, false);
				UILayer new_layer = AddChild<UILayer, UILayerDefine, GameObject>(layer, go);
				layers[layer.Name] = new_layer;
				__window_stack[layer.Name] = new LinkedList<string>();
			}

		}

		public GameObject GetUIRoot()
        {
			return gameObject;
		}

		public Camera GetUICamera()
        {
			return UICamera;
		}

		public GameObject GetUICameraGo()
        {
			return UICamera.gameObject;
		}

		public Vector2 GetResolution()
        {
			return Resolution;
		}

		public void SetNeedTurn(bool flag)
        {
			need_turn = flag;
		}

		public bool GetNeedTurn()
        {
			return need_turn;
		}
		/// <summary>
		/// 获取UI窗口
		/// </summary>
		/// <param name="ui_name"></param>
		/// <param name="active">1打开，-1关闭,0不做限制</param>
		/// <param name="view_active">1打开，-1关闭,0不做限制</param>
		/// <returns></returns>
		public UIWindow GetWindow(string ui_name,int active =0, int view_active=0)
        {
			if(windows.TryGetValue(ui_name,out var target))
            {
                if (active == 0 || active== (target.Active?1:-1))
                {
					if (view_active == 0 || view_active == (target.View.gameObject.activeSelf ? 1 : -1))
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

		public UIBaseView GetView(string ui_name)
		{
			var res = GetWindow(ui_name);
			if (res != null)
            {
				return res.View;
            }
			return null;
		}

		public UILayer GetLayer(string layer)
        {
			if(layers.TryGetValue(layer,out var res))
            {
				return res;
			}
			return null;
        }


		public void CloseWindow<T>()
		{
			string ui_name = typeof(T).Name;
			CloseWindow(ui_name);
		}
		public void CloseWindow(string ui_name)
        {
			var target = GetWindow(ui_name, 1);
			if (target == null) return;

			__RemoveFromStack(target);
			__InnnerCloseWindow(target);

		}

		public void CloseWindowByLayer(UILayerDefine layer, string[] except_ui_names = null)
		{
			Dictionary<string, bool> dict_ui_names= null;
			if (except_ui_names != null)
			{
				dict_ui_names = new Dictionary<string, bool>();
				foreach (var item in except_ui_names) dict_ui_names[item] = true;
			}

            foreach (var item in windows)
            {
				if (item.Value.Layer.Name == layer.Name && dict_ui_names!=null&&!dict_ui_names.ContainsKey(item.Key))
                {
					CloseWindow(item.Key);
				}
            }
        }

		public void DestroyWindow<T>()
		{
			string ui_name = typeof(T).Name;
			DestroyWindow(ui_name);
		}
		public void DestroyWindow(string ui_name)
		{
			var target = GetWindow(ui_name);
			if (target != null)
			{
				CloseWindow(ui_name);
				__InnerDestroyWindow(target);
			}
		}

		void __InnerDestroyWindow(UIWindow target)
        {
			if (GameObjectPoolComponent.Instance == null)
				GameObject.Destroy(target.View.gameObject);
			else
				GameObjectPoolComponent.Instance.RecycleGameObject(target.View.gameObject);
			target.View.OnDestroy();
			windows.Remove(target.Name);
		}

		/// <summary>
		/// 初始化window
		/// </summary>
		UIWindow __InitWindow<T>(string layer_name)where T: UIBaseView
        {
			if(layers.TryGetValue(layer_name,out var layer))
            {
				UIWindow window = new UIWindow();
				var type = typeof(T);
				window.Name = type.Name;
				window.View = AddChild<T>();
				window.View.__BaseViewName = type.Name;
				window.Active = false;
				window.Layer = layer;
				window.PrefabPath = window.View.PrefabPath;
				return window;
			}
			Log.Error("No layer named : " + layer_name + ".You should create it first!");
			return null;
        }


		void __ActivateWindow(UIWindow target)
        {
			if(target.View.gameObject == null)
            {
				Log.Error("You can only activate window after prefab loaded!");
				return;
            }
			target.View.SetActive(true);

		}
		void __ActivateWindow<T>(UIWindow target,T p1)
		{
			if (target.View.gameObject == null)
			{
				Log.Error("You can only activate window after prefab loaded!");
				return;
			}
			target.View.SetActive(true, p1);

		}
		void __ActivateWindow<T,P>(UIWindow target,T p1,P p2)
		{
			if (target.View.gameObject == null)
			{
				Log.Error("You can only activate window after prefab loaded!");
				return;
			}
			target.View.SetActive(true, p1, p2);

		}
		void __ActivateWindow<T,P,K>(UIWindow target, T p1, P p2,K p3)
		{
			if (target.View.gameObject == null)
			{
				Log.Error("You can only activate window after prefab loaded!");
				return;
			}
			target.View.SetActive(true, p1, p2, p3);

		}

		void __Deactivate(UIWindow target)
        {
			if(target.View==null || target.View.gameObject == null)
            {
				return;
            }
			target.View.SetActive(false);
		}

		async ETTask<T> __InnerOpenWindow<T>(UIWindow target,Action<T> callback) where T: UIBaseView
		{
			target.Active = true;
			T res;
			var need_load = target.View.gameObject == null;
			if (!need_load)
			{
				__AddWindowToStack(target);
				res = target.View as T;
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
					__AddWindowToStack(target);
					res = target.View as T;
					callback?.Invoke(res);
					return res;
				}
				target.IsLoading = true;
				await __CoLoadDependency(target);
				var go = await GameObjectPoolComponent.Instance.GetGameObjectAsync(target.PrefabPath);
				OnLoadGameObjectDone(target, go);
				res = target.View as T;
				callback?.Invoke(res);
				return res;
			}
		}
		async ETTask<T> __InnerOpenWindow<T,P1>(UIWindow target, P1 p1, Action<T> callback) where T : UIBaseView
		{
			target.Active = true;
			T res;
			var need_load = target.View.gameObject == null;
			if (!need_load)
			{
				__AddWindowToStack(target,p1);
				res = target.View as T;
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
					__AddWindowToStack(target, p1);
					res = target.View as T;
					callback?.Invoke(res);
					return res;
				}
				target.IsLoading = true;
				await __CoLoadDependency(target);
				var go = await GameObjectPoolComponent.Instance.GetGameObjectAsync(target.PrefabPath);
				OnLoadGameObjectDone(target, go,p1);
				res = target.View as T;
				callback?.Invoke(res);
				return res;
			}
		}
		async ETTask<T> __InnerOpenWindow<T,P1,P2>(UIWindow target, P1 p1, P2 p2, Action<T> callback) where T : UIBaseView
		{
			target.Active = true;
			T res;
			var need_load = target.View.gameObject == null;
			if (!need_load)
			{
				__AddWindowToStack(target, p1,p2);
				res = target.View as T;
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
					__AddWindowToStack(target, p1, p2);
					res = target.View as T;
					callback?.Invoke(res);
					return res;
				}
				target.IsLoading = true;
				await __CoLoadDependency(target);
				var go = await GameObjectPoolComponent.Instance.GetGameObjectAsync(target.PrefabPath);
				OnLoadGameObjectDone(target, go, p1, p2);
				res = target.View as T;
				callback?.Invoke(res);
				return res;
			}
		}
		async ETTask<T> __InnerOpenWindow<T,P1,P2,P3>(UIWindow target, P1 p1, P2 p2, P3 p3, Action<T> callback) where T : UIBaseView
		{
			target.Active = true;
			T res;
			var need_load = target.View.gameObject == null;
			if (!need_load)
			{
				__AddWindowToStack(target, p1,p2,p3);
				res = target.View as T;
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
					__AddWindowToStack(target, p1, p2, p3);
					res = target.View as T;
					callback?.Invoke(res);
					return res;
				}
				target.IsLoading = true;
				await __CoLoadDependency(target);
				var go = await GameObjectPoolComponent.Instance.GetGameObjectAsync(target.PrefabPath);
				OnLoadGameObjectDone(target, go, p1, p2, p3);
				res = target.View as T;
				callback?.Invoke(res);
				return res;
			}
		}
		//先加载UI依赖的资源
		async ETTask __CoLoadDependency(UIWindow target)
        {
			var reslist = __GetWindowPreloadRes(target);
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

		List<string> __GetWindowPreloadRes(UIWindow target)
        {
			List<string> res = new List<string>();
			//加载VIEW_CONFIG里面配置的依赖

			//允许代码逻辑控制需要增加的依赖
			var res2 = target.View.OnPreload();
			if (res2 != null)
				res.AddRange(res2);
			return res;
		}
		void OnLoadGameObjectDone(UIWindow target, GameObject go)
        {
            if (go == null)
            {
				Log.Error(string.Format("UIManager InnerOpenWindow {0} faild", target.PrefabPath));
				return;
			}
			var trans = go.transform;
			trans.SetParent(target.Layer.transform, false);
			trans.name = target.Name;
			target.IsLoading = false;
			target.View.gameObject = go;
			target.View.OnCreate();
			if (target.Active)
            {
				__AddWindowToStack(target);
			}
		}
		void OnLoadGameObjectDone<P1>(UIWindow target, GameObject go, P1 p1)
		{
			if (go == null)
			{
				Log.Error(string.Format("UIManager InnerOpenWindow {0} faild", target.PrefabPath));
				return;
			}
			var trans = go.transform;
			trans.SetParent(target.Layer.transform, false);
			trans.name = target.Name;
			target.IsLoading = false;
			target.View.gameObject = go;
			target.View.OnCreate();
			if (target.Active)
			{
				__AddWindowToStack(target,p1);
			}
		}
		void OnLoadGameObjectDone<P1,P2>(UIWindow target, GameObject go, P1 p1, P2 p2)
		{
			if (go == null)
			{
				Log.Error(string.Format("UIManager InnerOpenWindow {0} faild", target.PrefabPath));
				return;
			}
			var trans = go.transform;
			trans.SetParent(target.Layer.transform, false);
			trans.name = target.Name;
			target.IsLoading = false;
			target.View.gameObject = go;
			target.View.OnCreate();
			if (target.Active)
			{
				__AddWindowToStack(target,p1,p2);
			}
		}
		void OnLoadGameObjectDone<P1,P2,P3>(UIWindow target, GameObject go, P1 p1, P2 p2, P3 p3)
		{
			if (go == null)
			{
				Log.Error(string.Format("UIManager InnerOpenWindow {0} faild", target.PrefabPath));
				return;
			}
			var trans = go.transform;
			trans.SetParent(target.Layer.transform, false);
			trans.name = target.Name;
			target.IsLoading = false;
			target.View.gameObject = go;
			target.View.OnCreate();
			if (target.Active)
			{
				__AddWindowToStack(target,p1,p2,p3);
			}
		}

		void __InnnerCloseWindow(UIWindow target)
		{
			if (target.Active) {
				__Deactivate(target);
				target.Active = false;
			}
		}
		//判断窗口是否打开
		bool IsActiveWindow<T>() where T : UIBaseContainer
		{
			string ui_name = typeof(T).Name;
			var target = GetWindow(ui_name);
            if (target == null)
            {
				return false;
			}
			return target.Active;
		}

		//打开窗口
		public async ETTask<T> OpenWindow<T>(UILayerNames layertype = UILayerNames.NormalLayer, Action<T> callback =null) where T:UIBaseView
        {
			string layer_name = layertype.ToString();
			string ui_name = typeof(T).Name;
			var target = GetWindow(ui_name);
			if (target == null)
			{
				target = __InitWindow<T>(layer_name);
				windows[ui_name] = target;
			}
            else if(layer_name!= target.Layer.Name)
            {
				if (layers.TryGetValue(layer_name, out var layer))
				{
					target.Layer = layer;
					var trans = target.View.transform;
					if (trans)
						trans.SetParent(target.Layer.transform, false);
                }
                else
                {
					Log.Error("not layer, name: " + layer_name);
					callback(null);
					return null;
                }
			}
			return await __InnerOpenWindow(target, callback);
		}
		//打开窗口
		public async ETTask<T> OpenWindow<T,P1>( P1 p1, UILayerNames layertype = UILayerNames.NormalLayer, Action<T> callback = null) where T : UIBaseView
		{
			string layer_name = layertype.ToString();
			string ui_name = typeof(T).Name;
			var target = GetWindow(ui_name);
			if (target == null)
			{
				target = __InitWindow<T>(layer_name);
				windows[ui_name] = target;
			}
			else if (layer_name != target.Layer.Name)
			{
				if (layers.TryGetValue(layer_name, out var layer))
				{
					target.Layer = layer;
					var trans = target.View.transform;
					if (trans)
						trans.SetParent(target.Layer.transform, false);
				}
				else
				{
					Log.Error("not layer, name: " + layer_name);
					callback(null);
					return null;
				}
			}
			return await __InnerOpenWindow(target, p1, callback);
		}
		//打开窗口
		public async ETTask<T> OpenWindow<T, P1,P2>(P1 p1, P2 p2, UILayerNames layertype = UILayerNames.NormalLayer, Action<T> callback = null) where T : UIBaseView
		{
			string layer_name = layertype.ToString();
			string ui_name = typeof(T).Name;
			var target = GetWindow(ui_name);
			if (target == null)
			{
				target = __InitWindow<T>(layer_name);
				windows[ui_name] = target;
			}
			else if (layer_name != target.Layer.Name)
			{
				if (layers.TryGetValue(layer_name, out var layer))
				{
					target.Layer = layer;
					var trans = target.View.transform;
					if (trans)
						trans.SetParent(target.Layer.transform, false);
				}
				else
				{
					Log.Error("not layer, name: " + layer_name);
					callback(null);
					return null;
				}
			}
			return await __InnerOpenWindow(target, p1,p2, callback);
		}
		//打开窗口
		public async ETTask<T> OpenWindow<T, P1,P2,P3>(P1 p1, P2 p2, P3 p3, UILayerNames layertype = UILayerNames.NormalLayer, Action<T> callback=null) where T : UIBaseView
		{
			string layer_name = layertype.ToString();
			string ui_name = typeof(T).Name;
			var target = GetWindow(ui_name);
			if (target == null)
			{
				target = __InitWindow<T>(layer_name);
				windows[ui_name] = target;
			}
			else if (layer_name != target.Layer.Name)
			{
				if (layers.TryGetValue(layer_name, out var layer))
				{
					target.Layer = layer;
					var trans = target.View.transform;
					if (trans)
						trans.SetParent(target.Layer.transform, false);
				}
				else
				{
					Log.Error("not layer, name: " + layer_name);
					callback(null);
					return null;
				}
			}
			return await __InnerOpenWindow(target, p1,p2,p3, callback);
		}
		public void SetCanvasScaleEditorPortrait(bool flag)
        {
			layers[UILayers.GameLayer.Name].SetCanvasScaleEditorPortrait(flag);
			layers[UILayers.TipLayer.Name].SetCanvasScaleEditorPortrait(flag);
			layers[UILayers.TopLayer.Name].SetCanvasScaleEditorPortrait(flag);
			layers[UILayers.GameBackgroudLayer.Name].SetCanvasScaleEditorPortrait(flag);
		}
		void __AddWindowToStack(UIWindow target)
        {
			var ui_name = target.Name;
			var layer_name = target.Layer.Name;
			bool isFirst = false;
            if (__window_stack[layer_name].Contains(ui_name))
            {
				isFirst = true;
				__window_stack[layer_name].Remove(ui_name);
			}
			__window_stack[layer_name].AddFirst(ui_name);
			if (target.View.transform)
			{
				target.View.transform.SetAsLastSibling();
				__ActivateWindow(target);
			}
			if(isFirst && layer_name == UILayers.BackgroudLayer.Name || layer_name == UILayers.GameBackgroudLayer.Name)
            {
				//如果是背景layer，则销毁所有的normal层|BackgroudLayer
				CloseWindowByLayer(UILayers.NormalLayer);
				CloseWindowByLayer(UILayers.GameLayer);
				CloseWindowByLayer(UILayers.BackgroudLayer, new string[]{ ui_name});
				CloseWindowByLayer(UILayers.GameBackgroudLayer, new string[] { ui_name});
			}
		}
		void __AddWindowToStack<P1>(UIWindow target,P1 p1)
		{
			var ui_name = target.Name;
			var layer_name = target.Layer.Name;
			bool isFirst = false;
			if (__window_stack[layer_name].Contains(ui_name))
			{
				isFirst = true;
				__window_stack[layer_name].Remove(ui_name);
			}
			__window_stack[layer_name].AddFirst(ui_name);
			target.View.transform.SetAsLastSibling();
			__ActivateWindow(target, p1);
			if (isFirst && layer_name == UILayers.BackgroudLayer.Name || layer_name == UILayers.GameBackgroudLayer.Name)
			{
				//如果是背景layer，则销毁所有的normal层|BackgroudLayer
				CloseWindowByLayer(UILayers.NormalLayer);
				CloseWindowByLayer(UILayers.GameLayer);
				CloseWindowByLayer(UILayers.BackgroudLayer, new string[] { ui_name });
				CloseWindowByLayer(UILayers.GameBackgroudLayer, new string[] { ui_name });
			}
		}
		void __AddWindowToStack<P1, P2>(UIWindow target, P1 p1, P2 p2)
		{
			var ui_name = target.Name;
			var layer_name = target.Layer.Name;
			bool isFirst = false;
			if (__window_stack[layer_name].Contains(ui_name))
			{
				isFirst = true;
				__window_stack[layer_name].Remove(ui_name);
			}
			__window_stack[layer_name].AddFirst(ui_name);
			target.View.transform.SetAsLastSibling();
			__ActivateWindow(target, p1,p2);
			if (isFirst && layer_name == UILayers.BackgroudLayer.Name || layer_name == UILayers.GameBackgroudLayer.Name)
			{
				//如果是背景layer，则销毁所有的normal层|BackgroudLayer
				CloseWindowByLayer(UILayers.NormalLayer);
				CloseWindowByLayer(UILayers.GameLayer);
				CloseWindowByLayer(UILayers.BackgroudLayer, new string[] { ui_name });
				CloseWindowByLayer(UILayers.GameBackgroudLayer, new string[] { ui_name });
			}
		}
		void __AddWindowToStack<P1, P2,P3>(UIWindow target, P1 p1, P2 p2,P3 p3)
		{
			var ui_name = target.Name;
			var layer_name = target.Layer.Name;
			bool isFirst = false;
			if (__window_stack[layer_name].Contains(ui_name))
			{
				isFirst = true;
				__window_stack[layer_name].Remove(ui_name);
			}
			__window_stack[layer_name].AddFirst(ui_name);
			target.View.transform.SetAsLastSibling();
			__ActivateWindow(target,p1,p2,p3);
			if (isFirst && layer_name == UILayers.BackgroudLayer.Name || layer_name == UILayers.GameBackgroudLayer.Name)
			{
				//如果是背景layer，则销毁所有的normal层|BackgroudLayer
				CloseWindowByLayer(UILayers.NormalLayer);
				CloseWindowByLayer(UILayers.GameLayer);
				CloseWindowByLayer(UILayers.BackgroudLayer, new string[] { ui_name });
				CloseWindowByLayer(UILayers.GameBackgroudLayer, new string[] { ui_name });
			}
		}
		void __RemoveFromStack(UIWindow target)
		{
			var ui_name = target.Name;
			var layer_name = target.Layer.Name;
			if (__window_stack.ContainsKey(layer_name))
			{
				__window_stack[layer_name].Remove(ui_name);
			}
            else
            {
				Log.Error("not layer, name :" + layer_name);
            }
		}
		//销毁指定窗口所有窗口
		public void DestroyWindowExceptNames(string[] type_names = null)
		{
			Dictionary<string, bool> dict_ui_names = new Dictionary<string, bool>();
			if (type_names != null)
			{
				for (int i = 0; i < type_names.Length; i++)
				{
					dict_ui_names[type_names[i]] = true;
				}
			}
			var keys = windows.Keys.ToArray();
			for (int i = windows.Count - 1; i >= 0; i--)
			{
				if (!dict_ui_names.ContainsKey(keys[i]))
				{
					DestroyWindow(keys[i]);
				}
			}
		}
		//销毁指定层级外层级所有窗口
		public void DestroyWindowExceptLayer(UILayerDefine layer)
        {
			var keys = windows.Keys.ToArray();
			for (int i = windows.Count-1; i >= 0; i--)
            {
				if (windows[keys[i]].Layer.Name != layer.Name)
				{
					DestroyWindow(keys[i]);
				}
			}
        }
		//销毁层级所有窗口
		public void DestroyWindowByLayer(UILayerDefine layer)
		{
			var keys = windows.Keys.ToArray();
			for (int i = windows.Count - 1; i >= 0; i--)
			{
				if (windows[keys[i]].Layer.Name == layer.Name)
				{
					DestroyWindow(windows[keys[i]].Name);
				}
			}
		}
		public void DestroyAllWindow()
        {
			var keys = windows.Keys.ToArray();
			for (int i = windows.Count - 1; i >= 0; i--)
			{
				DestroyWindow(windows[keys[i]].Name);
			}
		}

        public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}
            DestroyAllWindow();
            windows.Clear();
            windows = null;
			__window_stack.Clear();
			__window_stack = null;
			foreach (var item in layers)
            {
				GameObject.Destroy(item.Value.gameObject);
            }
			layers.Clear();
            layers = null;
            base.Dispose();

			Instance = null;
			Log.Info("UIManagerComponent Dispose");
		}
	}
}
