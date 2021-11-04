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
        public virtual bool HasI18N => false;
        public string Path;
        public virtual void OnCreate()
        {
            
        }
        public virtual void OnCreate<T>(T param1)
        {
            
        }

        public virtual void OnCreate<T,P>(T param1,P param2)
        {
            
        }
        public virtual void OnCreate<T, P,K>(T param1, P param2,K param3)
        {
            
        }
        void BeroreOnEnable()
        {
            if (HasI18N)
                Messager.Instance.AddListener(MessagerId.OnLanguageChange, OnLanguageChange);
            Walk((component) =>
            {
                component.OnEnable();
            });
        }
        public virtual void OnEnable()
        {
            BeroreOnEnable();
        }
        public virtual void OnEnable<T>(T param1)
        {
            BeroreOnEnable();
        }

        public virtual void OnEnable<T, P>(T param1, P param2)
        {
            BeroreOnEnable();
        }
        public virtual void OnEnable<T, P, K>(T param1, P param2, K param3)
        {
            BeroreOnEnable();
        }
        void BeforeOnDisable()
        {
            if (HasI18N)
                Messager.Instance.RemoveListener(MessagerId.OnLanguageChange, OnLanguageChange);
            Walk((component) =>
            {
                component.OnDisable();
            });
        }
        public virtual void OnDisable()
        {
            BeforeOnDisable();
        }
        public virtual void OnDisable<T>(T param1)
        {
            BeforeOnDisable();
        }

        public virtual void OnDisable<T, P>(T param1, P param2)
        {
            BeforeOnDisable();
        }
        public virtual void OnDisable<T, P, K>(T param1, P param2, K param3)
        {
            BeforeOnDisable();
        }
        public virtual void OnLanguageChange(object sender = null, EventArgs args = null)
        {

        }

        public virtual void OnDestroy()
        {
            var keys1 = components.Keys.ToList();
            for (int i = keys1.Count-1; i >= 0; i--)
            {
                if (components[keys1[i]] != null)
                {
                    var keys2 = components[keys1[i]].Keys.ToList();
                    for (int j = keys2.Count-1; j >= 0; j--)
                    {
                        components[keys1[i]][keys2[j]].OnDestroy();
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

        public List<string> OnPreload()
        {
            return null;
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
        public T AddComponent<T>(string path) where T : UIBaseContainer
        {
            Type type = typeof(T);
            T component_inst = AddChild<T>();
            component_inst.Path = path;
            component_inst.OnComponentDestroy = () =>
            {
                __RemoveComponent<T>(path);
            };
            component_inst.OnCreate();

            RecordComponent(path, type, component_inst);
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
            component_inst.OnCreate(a);

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
            component_inst.OnCreate(a, b);

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
            component_inst.OnCreate(a, b, c);

            RecordComponent(path, type, component_inst);
            length++;
            return component_inst;
        }
        public virtual void SetActive(bool active)
        {
            if (active)
            {
                OnEnable();
            }
            else
            {
                OnDisable();
            }
        }

        public virtual void SetActive<T>(bool active, T param1)
        {
            if (active)
            {
                OnEnable(param1);
            }
            else
            {
                OnDisable(param1);
            }
        }
        public virtual void SetActive<T, P>(bool active, T param1, P param2)
        {
            if (active)
            {
                OnEnable(param1, param2);
            }
            else
            {
                OnDisable(param1, param2);
            }
        }
        public virtual void SetActive<T, P, K>(bool active, T param1, P param2, K param3)
        {
            if (active)
            {
                OnEnable(param1, param2, param3);
            }
            else
            {
                OnDisable(param1, param2, param3);
            }
        }
        /// <summary>
        /// 获取组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        protected T InnerGetComponent<T>(string path) where T : UIBaseContainer
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
        protected void InnerRemoveComponent<T>(string path) where T : UIBaseContainer
        {
            var component = InnerGetComponent<T>(path);
            if (component != null)
            {
                component.OnDestroy();
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
            var component = InnerGetComponent<T>(path);
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
