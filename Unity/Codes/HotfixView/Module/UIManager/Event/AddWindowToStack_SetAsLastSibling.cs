using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
	public class AddWindowToStack_SetAsLastSibling : AEvent<UIEventType.AddWindowToStack>
	{
		protected override async ETTask Run(UIEventType.AddWindowToStack args)
		{
			var view = args.window.GetComponent(args.window.ViewType) as UIBaseView;
			if (view.transform)
			{
				view.transform.SetAsLastSibling();
			}
			await ETTask.CompletedTask;
		}
	}
}
