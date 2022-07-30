using System;
using System.Collections.Generic;

namespace ET
{
    using OneTypeSystems = UnOrderMultiMap<Type, object>;
    [FriendClass(typeof(InputWatcherComponent))]
    public static class InputWatcherComponentSystem
    {
        [ObjectSystem]
        public class InputWatcherComponentAwakeSystem : AwakeSystem<InputWatcherComponent>
        {
            public override void Awake(InputWatcherComponent self)
            {
                InputWatcherComponent.Instance = self;
                self.InputEntitys = new List<Entity>();
                self.Init();
            }
        }

	
        public class InputWatcherComponentLoadSystem : LoadSystem<InputWatcherComponent>
        {
            public override void Load(InputWatcherComponent self)
            {
                self.Init();
            }
        }

        private static void Init(this InputWatcherComponent self)
        {
            self.typeSystems = new TypeSystems();
            self.typeMapAttr = new UnOrderMultiMap<IInputSystem, InputSystemAttribute>();
            self.sortList = new MultiDictionary<int, int, LinkedList<Tuple<IInputSystem, Entity,int>>>();
            List<Type> types = Game.EventSystem.GetTypes(typeof(InputSystemAttribute));
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(InputSystemAttribute), false);
                if(attrs.Length<=0) return;
                IInputSystem obj = Activator.CreateInstance(type) as IInputSystem;
                for (int i = 0; i < attrs.Length; i++)
                {
                    var attr = attrs[i] as InputSystemAttribute;
                    if (!Define.Debug && attr.Priority <= -10000)
                    {
                        continue;
                    }
                    if (obj is ISystemType iSystemType)
                    {
                        OneTypeSystems oneTypeSystems = self.typeSystems.GetOrCreateOneTypeSystems(iSystemType.Type());
                        oneTypeSystems.Add(iSystemType.SystemType(), obj);
                        self.typeMapAttr.Add(obj, attr);
                        InputComponent.Instance.AddListenter(attr.KeyCode);
                    }
                }
            }
        }

        public static void Run(this InputWatcherComponent self, int code,int type)
        {
            LinkedList<Tuple<IInputSystem,Entity,int>> typeSystems;
            if (!self.sortList.TryGetValue(code,type, out typeSystems))
            {
                return;
            }

            bool stop = false;
            //优先级高的在后面
            for (var node = typeSystems.Last; node!=null&&!stop; node=node.Previous)
            {
                var component = node.Value.Item2;
                var system = node.Value.Item1;
                try
                {
                    system.Run(component,code,type,ref stop);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        
        public static void RegisterInputEntity(this InputWatcherComponent self,Entity entity)
        {
            if (!self.InputEntitys.Contains(entity))
            {
                self.InputEntitys.Add(entity);
                List<object> iInputSystems = self.typeSystems.GetSystems(entity.GetType(), typeof(IInputSystem));
                if (iInputSystems == null)
                {
                    return;
                }
                for (int i = 0; i < iInputSystems.Count; i++)
                {
                    IInputSystem inputSystem = (IInputSystem)iInputSystems[i];
                    if (inputSystem == null)
                    {
                        continue;
                    }

                    if (self.typeMapAttr.TryGetValue(inputSystem, out var attrs))
                    {
                        for (int j = 0; j < attrs.Count; j++)
                        {
                            var attr = attrs[j];
                            var code = attr.KeyCode;
                            var type = attr.InputType;
                            var priority = attr.Priority;
                            if (!self.sortList.TryGetValue(code, type,out var list))
                            {
                                list = new LinkedList<Tuple<IInputSystem, Entity, int>>();
                                self.sortList.Add(code, type,list);
                                list.AddLast(new Tuple<IInputSystem, Entity, int>(inputSystem, entity, priority));
                            }
                            else
                            {
                                bool isAdd = false;
                                for (var node = list.Last; node!=null; node=node.Previous)
                                {
                                    if (node.Value.Item3 <= priority)
                                    {
                                        list.AddAfter(node,new Tuple<IInputSystem, Entity, int>(inputSystem, entity, priority));
                                        isAdd = true;
                                        break;
                                    }
                                }
                                if (!isAdd)
                                {
                                    list.AddFirst(new Tuple<IInputSystem, Entity, int>(inputSystem, entity, priority));
                                }
                            }
                        }
                    }
                    else
                    {
                        Log.Error("RegisterInputEntity attr miss! type="+inputSystem.GetType().Name);
                    }
                }
            }

        }
        
        public static void RemoveInputEntity(this InputWatcherComponent self,Entity entity)
        {
            self.InputEntitys.Remove(entity);
            List<object> iInputSystems = self.typeSystems.GetSystems(entity.GetType(), typeof(IInputSystem));
            if (iInputSystems == null)
            {
                return;
            }
            for (int i = 0; i < iInputSystems.Count; i++)
            {
                IInputSystem inputSystem = (IInputSystem)iInputSystems[i];
                if (inputSystem == null)
                {
                    continue;
                }

                if (self.typeMapAttr.TryGetValue(inputSystem, out var attrs))
                {
                    for (int j = 0; j < attrs.Count; j++)
                    {
                        var attr = attrs[j];
                        var code = attr.KeyCode;
                        var type = attr.InputType;
                        if (self.sortList.TryGetValue(code, type,out var list))
                        {
                            for (var node = list.Last; node!=null;node = node.Previous)
                            {
                                if (node.Value.Item1 == inputSystem)
                                {
                                    list.Remove(node);
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Log.Error("RemoveInputEntity attr miss! type="+inputSystem.GetType().Name);
                }
            }
        }
    }
}