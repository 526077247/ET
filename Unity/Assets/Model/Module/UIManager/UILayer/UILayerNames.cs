using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
	public class UILayerNamesComparer : IEqualityComparer<UILayerNames>
	{
		public static UILayerNamesComparer Instance = new UILayerNamesComparer();
		public bool Equals(UILayerNames x, UILayerNames y)
		{
			return x == y;          //x.Equals(y);  注意这里不要使用Equals方法，因为也会造成封箱操作
		}

		public int GetHashCode(UILayerNames x)
		{
			return (int)x;
		}
	}
	public enum UILayerNames : byte
	{
		GameBackgroudLayer,
		BackgroudLayer,
		GameLayer,
		SceneLayer,
		NormalLayer,
		TipLayer,
		TopLayer,
	}
}
