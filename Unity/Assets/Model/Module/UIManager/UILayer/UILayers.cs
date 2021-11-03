using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
	//UILayers配置
	//做了横竖屏适配，见UIManager的 SetCanvasScaleEditorPortrait 方法
	public static class UILayers
	{
		static Dictionary<UILayerNames, UILayerDefine> layers;
		// 游戏内的背景层
		static readonly UILayerDefine GameBackgroudLayer = new UILayerDefine
		{
			Name = UILayerNames.GameBackgroudLayer,
			PlaneDistance = 1000,
			OrderInLayer = 0,
		};

		//主界面、全屏的一些界面
		static readonly UILayerDefine BackgroudLayer = new UILayerDefine
		{
			Name = UILayerNames.BackgroudLayer,
			PlaneDistance = 900,
			OrderInLayer = 1000,
		};

		//游戏内的View层
		static readonly UILayerDefine GameLayer = new UILayerDefine
		{
			Name = UILayerNames.GameLayer,
			PlaneDistance = 800,
			OrderInLayer = 1800,
		};
		// 场景UI，如：点击建筑查看建筑信息---一般置于场景之上，界面UI之下
		static readonly UILayerDefine SceneLayer = new UILayerDefine
		{
			Name = UILayerNames.SceneLayer,
			PlaneDistance = 700,
			OrderInLayer = 2000,
		};
		//普通UI，一级、二级、三级等窗口---一般由用户点击打开的多级窗口
		static readonly UILayerDefine NormalLayer = new UILayerDefine
		{
			Name = UILayerNames.NormalLayer,
			PlaneDistance = 600,
			OrderInLayer = 3000,
		};
		//提示UI，如：错误弹窗，网络连接弹窗等
		static readonly UILayerDefine TipLayer = new UILayerDefine
		{
			Name = UILayerNames.TipLayer,
			PlaneDistance = 500,
			OrderInLayer = 4000,
		};
		//顶层UI，如：场景加载
		static readonly UILayerDefine TopLayer = new UILayerDefine
		{
			Name = UILayerNames.TopLayer,
			PlaneDistance = 400,
			OrderInLayer = 5000,
		};
		static UILayers()
		{
			layers = new Dictionary<UILayerNames, UILayerDefine>(UILayerNamesComparer.Instance)
			{
				{ UILayerNames.GameBackgroudLayer,GameBackgroudLayer },
				{ UILayerNames.BackgroudLayer,BackgroudLayer },
				{ UILayerNames.GameLayer,GameLayer },
				{ UILayerNames.SceneLayer,SceneLayer },
				{ UILayerNames.NormalLayer,NormalLayer },
				{ UILayerNames.TipLayer,TipLayer },
				{ UILayerNames.TopLayer,TopLayer },
			};
		}
		public static UILayerDefine[] GetUILayers()
		{
			return layers.Values.ToArray();
		}

		public static UILayerDefine GetUILayer(UILayerNames name)
		{
			if (layers.TryGetValue(name, out var res))
			{
				return res;
			}
			return null;
		}
	}
}
