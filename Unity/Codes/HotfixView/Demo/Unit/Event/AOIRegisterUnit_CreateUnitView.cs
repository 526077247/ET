using UnityEngine;
using System;
namespace ET
{
    [FriendClass(typeof(AOIUnitComponent))]
    [FriendClass(typeof(AOITriggerComponent))]
    [FriendClass(typeof(OBBComponent))]
    [FriendClass(typeof(GameObjectComponent))]
    [FriendClass(typeof(GlobalComponent))]
    public class AOIRegisterUnit_CreateUnitView : AEvent<EventType.AOIRegisterUnit>
    {
        protected override void Run(EventType.AOIRegisterUnit args)
        {
            var myunitId = args.Unit.GetMyUnitIdFromZoneScene();
            if (args.Receive.Id != myunitId)
            {
                // Log.Info("args.Receive.Id != myunitId  "+args.Unit.Id+"   " +args.Receive.Id +"  "+ myunitId);
                return;
            }
            RunAsync(args).Coroutine();
        }
        
        async ETTask RunAsync(EventType.AOIRegisterUnit args)
        {
            GameObjectComponent showObj;
            var unit = args.Unit.GetParent<Unit>();
            if (unit.Type==UnitType.Player||unit.Type==UnitType.Monster)//人物怪物类
            {
                Log.Info("AOIRegisterUnit"+args.Unit.Id);
                // Unit View层
                // 这里可以改成异步加载，demo就不搞了
                var go = await GameObjectPoolComponent.Instance.GetGameObjectAsync(unit.Config.Perfab);
                var trans = go.GetComponentsInChildren<Transform>();
                for (int i = 0; i < trans.Length; i++)
                {
                    trans[i].gameObject.layer = LayerMask.NameToLayer("Unit");
                }
                go.transform.position = unit.Position;
                go.transform.parent = GlobalComponent.Instance.Unit;
                var idc = go.GetComponent<UnitIdComponent>();
                if (idc == null)
                    idc = go.AddComponent<UnitIdComponent>();
                idc.UnitId = args.Unit.Id;
                showObj = unit.AddComponent<GameObjectComponent,GameObject,Action>(go, () =>
                {
                    GameObject.Destroy(idc);
                    GameObjectPoolComponent.Instance?.RecycleGameObject(go);
                });
                unit.AddComponent<AnimatorComponent>();
                
                // unit.AddComponent<InfoComponent>();
                var combatU = unit.GetComponent<CombatUnitComponent>();
                if (combatU != null)
                {
                    combatU.GetComponent<BuffComponent>()?.ShowAllBuffView();
                }
            }
            else if (unit.Type==UnitType.Skill)
            {
                SkillColliderComponent colliderComponent = unit.GetComponent<SkillColliderComponent>();
                var go = await GameObjectPoolComponent.Instance.GetGameObjectAsync(unit.Config.Perfab);
                var trans = go.GetComponentsInChildren<Transform>();
                for (int i = 0; i < trans.Length; i++)
                {
                    trans[i].gameObject.layer = LayerMask.NameToLayer("Skill");
                }
                go.transform.position = unit.Position;
                go.transform.parent = GlobalComponent.Instance.Unit;
                go.transform.rotation = unit.Rotation;
                showObj = unit.AddComponent<GameObjectComponent,GameObject,Action>(go, () =>
                {
                    GameObjectPoolComponent.Instance?.RecycleGameObject(go);
                });
            }
            else
            {
                Log.Error("类型未处理");
                return;
            }
            if (GlobalComponent.Instance.ColliderDebug)
            {
                await TimerComponent.Instance.WaitAsync(10);
                var SphereTriggers = unit.GetComponent<AOIUnitComponent>().SphereTriggers;
                for (int i = 0; i < SphereTriggers.Count; i++)
                {
                    var item = SphereTriggers[i];
                    GameObject obj;
                    if (item.TriggerType == TriggerShapeType.Sphere)
                    {
                        obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        obj.GetComponent<Collider>().isTrigger = true;
                        obj.transform.parent = showObj.GameObject.transform;
                        obj.transform.localPosition = item.Offset;
                        obj.transform.localScale = new Vector3(item.Radius*2,item.Radius*2,item.Radius*2);
                    }
                    else if (item.TriggerType == TriggerShapeType.Cube)
                    {
                        obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        obj.GetComponent<Collider>().isTrigger = true;
                        obj.transform.parent = showObj.GameObject.transform;
                        obj.transform.localPosition = item.Offset;
                        obj.transform.localRotation = Quaternion.identity;
                        obj.transform.localScale = item.GetComponent<OBBComponent>().Scale;
                    }
                    else
                    {
                        Log.Error("Define.Debug 碰撞体未添加");
                        continue;
                    }
                    
                    var debugObj = showObj.AddChild<GameObjectComponent, GameObject, Action>(obj, () =>
                    {
                        GameObject.Destroy(obj);
                    });
                    debugObj.IsDebug = true;
                }
            }
        }
    }
}