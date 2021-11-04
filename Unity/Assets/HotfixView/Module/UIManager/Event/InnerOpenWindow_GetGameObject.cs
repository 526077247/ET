using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public class InnerOpenWindow_GetGameObject : AEvent<UIEventType.InnerOpenWindow>
    {
        async ETTask LoadDependency( UIWindow target)
        {
            List<string> res = new List<string>();
            //加载VIEW_CONFIG里面配置的依赖
            var view = target.GetComponent(target.ViewType) as UIBaseView;
            //允许代码逻辑控制需要增加的依赖
            var res2 = view.OnPreload();
            if (res2 != null)
                res.AddRange(res2);
            if (res.Count <= 0) return;


            using (ListComponent<ETTask> TaskScheduler = ListComponent<ETTask>.Create())
            {
                foreach (var res_path in res)
                {
                    TaskScheduler.List.Add(GameObjectPoolComponent.Instance.PreLoadGameObjectAsync(res_path, 1));
                }
                await ETTaskHelper.WaitAll(TaskScheduler.List);
            }
        }
        protected override async ETTask Run(UIEventType.InnerOpenWindow args)
        {
            await LoadDependency(args.window);
            var target = args.window;
			var go = await GameObjectPoolComponent.Instance.GetGameObjectAsync(args.path);
			if (go == null)
			{
				Log.Error(string.Format("UIManager InnerOpenWindow {0} faild", target.PrefabPath));
				return;
			}
			var trans = go.transform;
			trans.SetParent(UIManagerComponent.Instance.GetComponent<UILayersComponent>().layers[target.Layer].transform, false);
			trans.name = target.Name;
			var view = target.GetComponent(target.ViewType) as UIBaseView;
			view.gameObject = go;
			view.OnCreate();
			target.LoadingState = UIWindowLoadingState.LoadOver;
		}
    }
}
