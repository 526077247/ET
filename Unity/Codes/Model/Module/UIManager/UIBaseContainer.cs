using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ET
{
    /// <summary>
    /// UI容器，可以自定义UI侧的组件，并对组件的生命周期进行管理，不受Unity生命周期管理
    /// 所有UI相关扩展组件都应继承此类
    /// </summary>
    public class UIBaseContainer : Entity
    {
        Dictionary<string, Dictionary<Type, UIBaseContainer>> components = new Dictionary<string, Dictionary<Type, UIBaseContainer>>();//[path]:[component_name:UIBaseContainer]

        int length = 1;
        
        Action OnComponentDestroy;
        public string Path;

        public void AfterOnEnable()
        {
            Walk((component) =>
            {
                UIEventSystem.Instance.OnEnable(component);
            });
        }

        public void BeforeOnDisable()
        {
            Walk((component) =>
            {
                UIEventSystem.Instance.OnDisable(component);
            });
        }

        public void BeforeOnDestroy()
        {
            var keys1 = components.Keys.ToList();
            for (int i = keys1.Count-1; i >= 0; i--)
            {
                if (components[keys1[i]] != null)
                {
                    var keys2 = components[keys1[i]].Keys.ToList();
                    for (int j = keys2.Count-1; j >= 0; j--)
                    {
                        UIEventSystem.Instance.OnDestroy(components[keys1[i]][keys2[j]]);
                    }
                }
            }
            length--;
            if (length <= 0)
                OnComponentDestroy?.Invoke();
            else
                Log.Error("OnDestroy fail, length != 0");
            components = null;
            Dispose();
        }

        //遍历：注意，这里是无序的
        protected void Walk(Action<UIBaseContainer> callback)
        {
            foreach (var item in components)
            {
                if (item.Value != null)
                {
                    foreach (var item2 in item.Value)
                    {
                        callback(item2.Value);
                    }
                }
            }
        }

        //记录Component
        protected void RecordComponent(string name, Type component_class, UIBaseContainer component)
        {
            if(components.TryGetValue(name,out var obj))
            {
                if(obj.ContainsKey(component_class))
                {
                    Log.Error("Aready exist component_class : " + component_class.Name);
                    return;
                }
            }
            else//如果必要，创建新的记录，对应Unity下一个Transform下所有挂载脚本的记录表
            {
                components[name] = new Dictionary<Type, UIBaseContainer>();
            }
            components[name][component_class] = component;
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="path">路径</param>
        public T AddComponent<T>(string path = "") where T : UIBaseContainer
        {
            Type type = typeof(T);
            T component_inst = AddChild<T>();
            component_inst.Path = path;
            component_inst.OnComponentDestroy = () =>
            {
                __RemoveComponent<T>(path);
            };
            RecordComponent(path, type, component_inst);
            Game.EventSystem.Publish(new UIEventType.AddComponent() { entity = component_inst }); 
            UIEventSystem.Instance.OnCreate(component_inst);
            length++;
            return component_inst;
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="path">相对路径</param>
        public T AddComponent<T, A>(string path, A a) where T : UIBaseContainer
        {
            Type type = typeof(T);
            T component_inst = AddChild<T>();
            component_inst.Path = path;
            component_inst.OnComponentDestroy = () =>
            {
                __RemoveComponent<T>(path);
            };
            Game.EventSystem.Publish(new UIEventType.AddComponent() { entity = component_inst }); 
            UIEventSystem.Instance.OnCreate(component_inst,a);

            RecordComponent(path, type, component_inst);
            length++;
            return component_inst;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="path">路径</param>
        public T AddComponent<T, A, B>(string path, A a, B b) where T : UIBaseContainer
        {
            Type type = typeof(T);
            T component_inst = AddChild<T>();
            component_inst.Path = path;
            component_inst.OnComponentDestroy = () =>
            {
                __RemoveComponent<T>(path);
            };
            Game.EventSystem.Publish(new UIEventType.AddComponent() { entity = component_inst }); 
            UIEventSystem.Instance.OnCreate(component_inst, a,b);

            RecordComponent(path, type, component_inst);
            length++;
            return component_inst;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="path">路径</param>
        public T AddComponent<T, A, B, C>(string path, A a, B b, C c) where T : UIBaseContainer
        {
            Type type = typeof(T);
            T component_inst = AddChild<T>();
            component_inst.Path = path;
            component_inst.OnComponentDestroy = () =>
            {
                __RemoveComponent<T>(path);
            };
            Game.EventSystem.Publish(new UIEventType.AddComponent() { entity = component_inst }); 
            UIEventSystem.Instance.OnCreate(component_inst, a, b,c);

            RecordComponent(path, type, component_inst);
            length++;
            return component_inst;
        }
        public void SetActive(bool active)
        {
            Game.EventSystem.Publish(new UIEventType.SetActive() { entity = this,Active = active}); 
            if (active)
            {
                UIEventSystem.Instance.OnEnable(this);
            }
            else
            {
                UIEventSystem.Instance.OnDisable(this);
            }
        }

        public void SetActive<T>(bool active, T param1)
        {
            Game.EventSystem.Publish(new UIEventType.SetActive() { entity = this,Active = active}); 
            if (active)
            {
                UIEventSystem.Instance.OnEnable(this,param1);
            }
            else
            {
                UIEventSystem.Instance.OnDisable(this,param1);
            }
        }
        public void SetActive<T, P>(bool active, T param1, P param2)
        {
            Game.EventSystem.Publish(new UIEventType.SetActive() { entity = this,Active = active}); 
            if (active)
            {
                UIEventSystem.Instance.OnEnable(this, param1, param2);
            }
            else
            {
                UIEventSystem.Instance.OnDisable(this, param1, param2);
            }
        }
        public void SetActive<T, P, K>(bool active, T param1, P param2, K param3)
        {
            Game.EventSystem.Publish(new UIEventType.SetActive() { entity = this,Active = active}); 
            if (active)
            {
                UIEventSystem.Instance.OnEnable(this, param1, param2, param3);
            }
            else
            {
                UIEventSystem.Instance.OnDisable(this, param1, param2, param3);
            }
        }
        /// <summary>
        /// 获取组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T GetComponent<T>(string path = "") where T : UIBaseContainer
        {
            if (components.TryGetValue(path, out var obj))
            {
                Type type = typeof(T);
                if (obj.TryGetValue(type, out var component))
                {
                    return component as T;
                }
            }
            return null;
        }

        /// <summary>
        /// 移除组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        protected void RemoveComponent<T>(string path = "") where T : UIBaseContainer
        {
            var component = GetComponent<T>(path);
            if (component != null)
            {
                UIEventSystem.Instance.OnDestroy(component);
                components[path].Remove(typeof(T));
            }
        }

        /// <summary>
        /// 移除组件回调方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        void __RemoveComponent<T>(string path) where T : UIBaseContainer
        {
            var component = GetComponent<T>(path);
            if (component != null)
            {
                components[path].Remove(typeof(T));
                length--;
                if (components[path].Count <= 0)
                {
                    components.Remove(path);
                }
            }
        }

    }
}
