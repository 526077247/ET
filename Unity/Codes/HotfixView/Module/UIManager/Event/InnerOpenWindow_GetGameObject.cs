﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    public class InnerOpenWindow_GetGameObject : AEvent<UIEventType.InnerOpenWindow>
    {
	    protected override async ETTask Run(UIEventType.InnerOpenWindow args)
        {
	        var target = args.window;
	        var view = target.GetComponent(target.ViewType);
	        
	        await UIEventSystem.Instance.OnViewInitializationSystem(view);
            
			var go = await GameObjectPoolComponent.Instance.GetGameObjectAsync(args.path);
			if (go == null)
			{
				Log.Error(string.Format("UIManager InnerOpenWindow {0} faild", target.PrefabPath));
				return;
			}
			var trans = go.transform;
			trans.SetParent(UIManagerComponent.Instance.GetComponent<UILayersComponent>().layers[target.Layer].transform, false);
			trans.name = target.Name;
			
			view.AddUIComponent<UITransform,Transform>("", trans);
            UIEventSystem.Instance.OnCreate(view);
			target.LoadingState = UIWindowLoadingState.LoadOver;
		}
    }
}
