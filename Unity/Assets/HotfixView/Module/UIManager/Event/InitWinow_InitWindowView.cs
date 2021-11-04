using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    public class InitWinow_InitWindowView : AEvent<UIEventType.InitWindow>
	{
		protected override async ETTask Run(UIEventType.InitWindow args)
		{
			UIBaseView uibaseview = args.uibaseview as UIBaseView;
			uibaseview.__BaseViewName = args.name;
			args.window.PrefabPath = uibaseview.PrefabPath;
		}
	}
}
