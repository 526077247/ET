using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public static class UIWindowComponentSystem
    {
		/// <summary>
		/// 获取UI窗口
		/// </summary>
		/// <param name="ui_name"></param>
		/// <param name="active">1打开，-1关闭,0不做限制</param>
		/// <returns></returns>
		public static UIWindow GetWindow(this UIWindowComponent self, string ui_name, int active = 0)
		{
			if (self.windows.TryGetValue(ui_name, out var target))
			{
				if (active == 0 || active == (target.Active ? 1 : -1))
				{
					return target;
				}
				return null;
			}
			return null;
		}

		/// <summary>
		/// 移除
		/// </summary>
		/// <param name="self"></param>
		/// <param name="target"></param>
		public static void __RemoveFromStack(this UIWindowComponent self, UIWindow target)
		{
			var ui_name = target.Name;
			var layer_name = target.Layer;
			if (self.window_stack.ContainsKey(layer_name))
			{
				self.window_stack[layer_name].Remove(ui_name);
			}
			else
			{
				Log.Error("not layer, name :" + layer_name);
			}
		}
	}
}
