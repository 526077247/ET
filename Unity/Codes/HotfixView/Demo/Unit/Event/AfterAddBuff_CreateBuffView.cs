using System;
using UnityEngine;

namespace ET
{
    [FriendClass(typeof(BuffComponent))]
    public class AfterAddBuff_CreateBuffView: AEvent<EventType.AfterAddBuff>
    {
        protected override void Run(EventType.AfterAddBuff args)
        {
            RunAsync(args).Coroutine();
        }
        async ETTask RunAsync(EventType.AfterAddBuff args)
        {
            if (args.Buff.Config.ObjRoot != 0)
            {
                var unit = args.Buff.GetParent<BuffComponent>().unit;
                if (unit != null)
                {
                    var showObj = unit.GetComponent<GameObjectComponent>();
                    if (showObj == null) return;
                    Transform root = null;
                    if (args.Buff.Config.ObjRoot == 1)//ObjRoot=1对应挂点Head
                    {
                        root = showObj.GameObject.transform.Find("Head");
                    }
                    if(root==null) return;
                    var obj = await GameObjectPoolComponent.Instance.GetGameObjectAsync(args.Buff.Config.BuffObj);
                    obj.transform.SetParent(root);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localScale = Vector3.one;
                    args.Buff.AddComponent<GameObjectComponent, GameObject, Action>(obj, () =>
                    {
                        GameObjectPoolComponent.Instance?.RecycleGameObject(obj);
                    });
                }
            }
            await ETTask.CompletedTask;
        }
        
    }
}