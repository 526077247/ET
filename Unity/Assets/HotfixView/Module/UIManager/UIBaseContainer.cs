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
        Dictionary<string, Dictionary<Type, UIBaseContainer>> components;//[path]:[component_name:UIBaseContainer]

        int length;
        
        Action OnComponentDestroy;
        GameObject _gameObject;
        Transform _transform;
        public virtual bool HasI18N => false;
        public GameObject gameObject
        {
            get
            {
                return _gameObject;
            }
            set
            {
                _gameObject = value;
                _transform = _gameObject.transform;
            }
        }
        public Transform transform
        {
            get
            {
                return _transform;
            }
            set
            {
                _transform = value;
                _gameObject = _transform.gameObject;
            }
        }
        public virtual void BeforeOnCreate()
        {
            components = new Dictionary<string, Dictionary<Type, UIBaseContainer>>();
            length = 1;
        }
        public virtual void OnCreate()
        {
            BeforeOnCreate();
        }
        public virtual void OnCreate<T>(T param1)
        {
            BeforeOnCreate();
        }

        public virtual void OnCreate<T,P>(T param1,P param2)
        {
            BeforeOnCreate();
        }
        public virtual void OnCreate<T, P,K>(T param1, P param2,K param3)
        {
            BeforeOnCreate();
        }
        void BeroreOnEnable()
        {
            if (HasI18N)
                Messager.Instance.AddListener<Action>("OnLanguageChange", OnLanguageChange);
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
                Messager.Instance.RemoveListener<Action>("OnLanguageChange", OnLanguageChange);
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
        public virtual void OnLanguageChange()
        {

        }
        public virtual void SetActive(bool active)
        {
            if (active)
            {
                gameObject.SetActive(active);
                OnEnable();
            }
            else
            {
                OnDisable();
                gameObject.SetActive(active);
            }
        }

        public virtual void SetActive<T>(bool active,T param1)
        {
            if (active)
            {
                gameObject.SetActive(active);
                OnEnable(param1);
            }
            else
            {
                gameObject.SetActive(active);
                OnDisable(param1);
            }
        }
        public virtual void SetActive<T, P>(bool active, T param1, P param2)
        {
            if (active)
            {
                gameObject.SetActive(active);
                OnEnable(param1, param2);
            }
            else
            {
                gameObject.SetActive(active);
                OnDisable(param1, param2);
            }
        }
        public virtual void SetActive<T, P, K>(bool active, T param1, P param2, K param3)
        {
            if (active)
            {
                gameObject.SetActive(active);
                OnEnable(param1, param2, param3);
            }
            else
            {
                gameObject.SetActive(active);
                OnDisable(param1, param2,param3);
            }
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
                Log.Error(gameObject.name + "OnDestroy fail, length != 0");
            components = null;
            Dispose();
        }

        public List<string> OnPreload()
        {
            return null;
        }

        //遍历：注意，这里是无序的
        public void Walk(Action<UIBaseContainer> callback)
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

        //如果必要，创建新的记录，对应Unity下一个Transform下所有挂载脚本的记录表
        public void AddNewRecordIfNeeded(string name)
        {
            if (!components.ContainsKey(name))
            {
                components[name] = new Dictionary<Type, UIBaseContainer>();
            }
        }

        //记录Component
        public void RecordComponent(string name, Type component_class, UIBaseContainer component)
        {
            if(components.TryGetValue(name,out var obj))
            {
                if(obj.ContainsKey(component_class))
                {
                    Log.Error("Aready exist component_class : " + component_class.Name);
                    return;
                }
            }
            else
            {
                components[name] = new Dictionary<Type, UIBaseContainer>();
            }
            components[name][component_class] = component;
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="relative_path">相对路径</param>
        public T AddComponent<T>(string relative_path="") where T : UIBaseContainer
        {
            var base_transform = transform.Find(relative_path);
            if (base_transform != null)
            {
                Type type = typeof(T);
                T component_inst = AddChild<T>();
                component_inst.transform = base_transform;
                component_inst.OnComponentDestroy = () =>
                {
                    __RemoveComponent<T>(relative_path);
                };
                component_inst.OnCreate();

                RecordComponent(relative_path, type, component_inst);
                length++;
                return component_inst;
            }
            Log.Error(gameObject.name + " AddComponent fail, relative_path: " + relative_path);
            return null;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="relative_path">相对路径</param>
        public T AddComponent<T,A>(string relative_path,A a) where T : UIBaseContainer
        {
            var base_transform = transform.Find(relative_path);
            if (base_transform != null)
            {
                Type type = typeof(T);
                T component_inst = AddChild<T>();
                component_inst.transform = base_transform;
                component_inst.OnComponentDestroy = () =>
                {
                    __RemoveComponent<T>(relative_path);
                };
                component_inst.OnCreate(a);

                RecordComponent(relative_path, type, component_inst);
                length++;
                return component_inst;
            }
            Log.Error(gameObject.name + " AddComponent fail, relative_path: " + relative_path);
            return null;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="relative_path">相对路径</param>
        public T AddComponent<T,A,B>(string relative_path, A a,B b) where T : UIBaseContainer
        {
            var base_transform = transform.Find(relative_path);
            if (base_transform != null)
            {
                Type type = typeof(T);
                T component_inst = AddChild<T>();
                component_inst.transform = base_transform;
                component_inst.OnComponentDestroy = () =>
                {
                    __RemoveComponent<T>(relative_path);
                };
                component_inst.OnCreate(a, b);

                RecordComponent(relative_path, type, component_inst);
                length++;
                return component_inst;
            }
            Log.Error(gameObject.name + " AddComponent fail, relative_path: " + relative_path);
            return null;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="relative_path">相对路径</param>
        public T AddComponent<T,A,B,C>(string relative_path,A a,B b,C c) where T : UIBaseContainer
        {
            var base_transform = transform.Find(relative_path);
            if (base_transform != null)
            {
                Type type = typeof(T);
                T component_inst = AddChild<T>();
                component_inst.transform = base_transform;
                component_inst.OnComponentDestroy = () =>
                {
                    __RemoveComponent<T>(relative_path);
                };
                component_inst.OnCreate(a,b,c);

                RecordComponent(relative_path, type, component_inst);
                length++;
                return component_inst;
            }
            Log.Error(gameObject.name + " AddComponent fail, relative_path: " + relative_path);
            return null;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="index">子物体序号</param>
        public T AddComponent<T>(int index) where T : UIBaseContainer
        {
            var base_transform = transform.GetChild(index);
            if (base_transform != null)
            {
                Type type = typeof(T);
                T component_inst = AddChild<T>();
                component_inst.transform = base_transform;
                var relative_path = component_inst.gameObject.name;
                component_inst.OnComponentDestroy = () =>
                {
                    __RemoveComponent<T>(relative_path);
                };
                component_inst.OnCreate();
                RecordComponent(relative_path, type, component_inst);
                length++;
                return component_inst;
            }
            Log.Error(gameObject.name+" AddComponent fail, index: "+ index);
            return null;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="index">子物体序号</param>
        public T AddComponent<T, A>(int index, A a) where T : UIBaseContainer
        {
            var base_transform = transform.GetChild(index);
            if (base_transform != null)
            {
                Type type = typeof(T);
                T component_inst = AddChild<T>();
                component_inst.transform = base_transform;
                var relative_path = component_inst.gameObject.name;
                component_inst.OnComponentDestroy = () =>
                {
                    __RemoveComponent<T>(relative_path);
                };
                component_inst.OnCreate(a);

                RecordComponent(relative_path, type, component_inst);
                length++;
                return component_inst;
            }
            Log.Error(gameObject.name + " AddComponent fail, index: " + index);
            return null;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="index">子物体序号</param>
        public T AddComponent<T, A, B>(int index, A a, B b) where T : UIBaseContainer
        {
            var base_transform = transform.GetChild(index);
            if (base_transform != null)
            {
                Type type = typeof(T);
                T component_inst = AddChild<T>();
                component_inst.transform = base_transform;
                var relative_path = component_inst.gameObject.name;
                component_inst.OnComponentDestroy = () =>
                {
                    __RemoveComponent<T>(relative_path);
                };
                component_inst.OnCreate(a, b);

                RecordComponent(relative_path, type, component_inst);
                length++;
                return component_inst;
            }
            Log.Error(gameObject.name + " AddComponent fail, index: " + index);
            return null;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="index">子物体序号</param>
        public T AddComponent<T, A, B, C>(int index, A a, B b, C c) where T : UIBaseContainer
        {
            var base_transform = transform.GetChild(index);
            if (base_transform != null)
            {
                Type type = typeof(T);
                T component_inst = AddChild<T>();
                component_inst.transform = base_transform;
                var relative_path = component_inst.gameObject.name;
                component_inst.OnComponentDestroy = () =>
                {
                    __RemoveComponent<T>(relative_path);
                };
                component_inst.OnCreate(a, b, c);

                RecordComponent(relative_path, type, component_inst);
                length++;
                return component_inst;
            }
            Log.Error(gameObject.name + " AddComponent fail, index: " + index);
            return null;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">unity游戏物体</param>
        public T AddComponent<T>(GameObject obj) where T : UIBaseContainer
        {
            var base_transform = obj.transform;
            if (base_transform != null)
            {
                Type type = typeof(T);
                T component_inst = AddChild<T>();
                component_inst.transform = base_transform;
                var relative_path = component_inst.gameObject.name;
                component_inst.OnComponentDestroy = () =>
                {
                    __RemoveComponent<T>(relative_path);
                };
                component_inst.OnCreate();
                RecordComponent(relative_path, type, component_inst);
                length++;
                return component_inst;
            }
            Debug.LogError(gameObject.name + " AddComponent fail, index: " + obj.name);
            return null;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">unity游戏物体</param>
        public T AddComponent<T, A>(GameObject obj, A a) where T : UIBaseContainer
        {
            var base_transform = obj.transform;
            if (base_transform != null)
            {
                Type type = typeof(T);
                T component_inst = AddChild<T>();
                component_inst.transform = base_transform;
                var relative_path = component_inst.gameObject.name;
                component_inst.OnComponentDestroy = () =>
                {
                    __RemoveComponent<T>(relative_path);
                };
                component_inst.OnCreate(a);

                RecordComponent(relative_path, type, component_inst);
                length++;
                return component_inst;
            }
            Debug.LogError(gameObject.name + " AddComponent fail, index: " + obj.name);
            return null;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">unity游戏物体</param>
        public T AddComponent<T, A, B>(GameObject obj, A a, B b) where T : UIBaseContainer
        {
            var base_transform = obj.transform;
            if (base_transform != null)
            {
                Type type = typeof(T);
                T component_inst = AddChild<T>();
                component_inst.transform = base_transform;
                var relative_path = component_inst.gameObject.name;
                component_inst.OnComponentDestroy = () =>
                {
                    __RemoveComponent<T>(relative_path);
                };
                component_inst.OnCreate(a, b);

                RecordComponent(relative_path, type, component_inst);
                length++;
                return component_inst;
            }
            Debug.LogError(gameObject.name + " AddComponent fail, index: " + obj.name);
            return null;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">unity游戏物体</param>
        public T AddComponent<T, A, B, C>(GameObject obj, A a, B b, C c) where T : UIBaseContainer
        {
            var base_transform = obj.transform;
            if (base_transform != null)
            {
                Type type = typeof(T);
                T component_inst = AddChild<T>();
                component_inst.transform = base_transform;
                var relative_path = component_inst.gameObject.name;
                component_inst.OnComponentDestroy = () =>
                {
                    __RemoveComponent<T>(relative_path);
                };
                component_inst.OnCreate(a, b, c);

                RecordComponent(relative_path, type, component_inst);
                length++;
                return component_inst;
            }
            Debug.LogError(gameObject.name + " AddComponent fail, index: " + obj.name);
            return null;
        }
        /// <summary>
        /// 获取组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>(string relative_path="") where T : UIBaseContainer
        {
            if (components.TryGetValue(relative_path, out var obj))
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
        public void RemoveComponent<T>(string path="") where T : UIBaseContainer
        {
            var component = GetComponent<T>(path);
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
