using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
	public class UIWindow : Entity
	{
		/// <summary>
		/// 窗口名字
		/// </summary>
		public string Name;
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
		/// <summary>
		/// 窗口层级
		/// </summary>
		public UILayerNames Layer;
		/// <summary>
		/// 窗口类型
		/// </summary>
		public Type ViewType;
	}
}
