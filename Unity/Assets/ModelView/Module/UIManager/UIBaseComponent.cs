using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    public class UIBaseComponent : UIBaseContainer
    {
        protected GameObject _gameObject;
        protected Transform _transform;
        public GameObject gameObject
        {
            get
            {
                if(_gameObject == null)
                {
                    _gameObject = transform?.gameObject;
                }
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
                if (_transform == null)
                {
                    _transform = GetTransform(this);
                    if (_transform == null)
                    {
                        Log.Error("AddComponent fail,last relative_path: " + Path);
                    }
                }
                return _transform;
            }
            set
            {
                _transform = value;
                _gameObject = _transform.gameObject;
            }
        }
        protected Transform GetTransform(UIBaseContainer entity)
        {
            if (!(entity.Parent is UIBaseComponent bc))
            {
                if (!(entity.Parent is UIBaseContainer bcr)) return null;
                var res = GetTransform(bcr);
                return res.Find(entity.Path);
            }
            else
            {
                var baseTrans = bc.transform.Find(entity.Path);
                if (!baseTrans)
                {
                    Log.Error(baseTrans.name + " Find NotFound! Path: " + entity.Path);
                }
                return baseTrans;
            }
        }
        
        public override void SetActive(bool active)
        {
            if (active)
            {
                gameObject.SetActive(active);
                base.SetActive(active);
            }
            else
            {
                base.SetActive(active);
                gameObject.SetActive(active);
            }
        }

        public override void SetActive<T>(bool active, T param1)
        {
            if (active)
            {
                gameObject.SetActive(active);
                base.SetActive(active,param1);
            }
            else
            {
                base.SetActive(active, param1);
                gameObject.SetActive(active);
            }
        }
        public override void SetActive<T, P>(bool active, T param1, P param2)
        {
            if (active)
            {
                gameObject.SetActive(active);
                base.SetActive(active, param1, param2);
            }
            else
            {
                base.SetActive(active, param1, param2);
                gameObject.SetActive(active);
            }
        }
        public override void SetActive<T, P, K>(bool active, T param1, P param2, K param3)
        {
            if (active)
            {
                gameObject.SetActive(active);
                base.SetActive(active, param1, param2,param3);
            }
            else
            {
                base.SetActive(active, param1, param2, param3);
                gameObject.SetActive(active);
            }
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="relative_path">相对路径</param>
        public new T AddComponent<T>(string relative_path = "") where T : UIBaseContainer
        {
            var bc = base.AddComponent<T>(relative_path);
            if (bc is UIBaseComponent res)
            {
                var base_transform = transform.Find(relative_path);
                if (base_transform != null)
                {
                    res.transform = base_transform; ;
                }
            }
            return bc;
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="relative_path">相对路径</param>
        public new T AddComponent<T, A>(string relative_path, A a) where T : UIBaseContainer
        {
            var bc = base.AddComponent<T,A>(relative_path,a);
            if (bc is UIBaseComponent res)
            {
                var base_transform = transform.Find(relative_path);
                if (base_transform != null)
                {
                    res.transform = base_transform; ;
                }
            }
            return bc;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="relative_path">相对路径</param>
        public new T AddComponent<T, A, B>(string relative_path, A a, B b) where T : UIBaseContainer
        {
            var bc = base.AddComponent<T, A, B>(relative_path,a,b);
            if (bc is UIBaseComponent res)
            {
                var base_transform = transform.Find(relative_path);
                if (base_transform != null)
                {
                    res.transform = base_transform; ;
                }
            }
            return bc;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="relative_path">相对路径</param>
        public new T AddComponent<T, A, B, C>(string relative_path, A a, B b, C c) where T : UIBaseContainer
        {
            var bc = base.AddComponent<T, A, B, C>(relative_path, a, b, c);
            if (bc is UIBaseComponent res)
            {
                var base_transform = transform.Find(relative_path);
                if (base_transform != null)
                {
                    res.transform = base_transform; ;
                }
            }
            return bc;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="index">子物体序号</param>
        public T AddComponent<T>(int index) where T : UIBaseComponent
        {
            var base_transform = transform.GetChild(index);
            if (base_transform != null)
            {
                var res = base.AddComponent<T>(base_transform.name);
                res.transform = base_transform;;
                return res;
            }
            Log.Error(transform.name + " AddComponent fail, index: " + index);
            return null;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="index">子物体序号</param>
        public T AddComponent<T, A>(int index, A a) where T : UIBaseComponent
        {

            var base_transform = transform.GetChild(index);
            if (base_transform != null)
            {
                var res = base.AddComponent<T, A>(base_transform.name, a);
                res.transform = base_transform;;
                return res;
            }
            Log.Error(transform.name + " AddComponent fail, index: " + index);
            return null;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="index">子物体序号</param>
        public T AddComponent<T, A, B>(int index, A a, B b) where T : UIBaseComponent
        {

            var base_transform = transform.GetChild(index);
            if (base_transform != null)
            {
                var res = base.AddComponent<T, A, B>(base_transform.name, a, b);
                res.transform = base_transform;;
                return res;
            }
            Log.Error(transform.name + " AddComponent fail, index: " + index);
            return null;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="index">子物体序号</param>
        public T AddComponent<T, A, B, C>(int index, A a, B b, C c) where T : UIBaseComponent
        {

            var base_transform = transform.GetChild(index);
            if (base_transform != null)
            {
                var res = base.AddComponent<T>(base_transform.name);
                res.transform = base_transform;;
                return res;
            }
            Log.Error(transform.name + " AddComponent fail, index: " + index);
            return null;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">unity游戏物体</param>
        public T AddComponent<T>(GameObject obj) where T : UIBaseComponent
        {

            if (obj != null)
            {
                var res = base.AddComponent<T>(obj.name);
                res.AddComponent<UIBaseComponent, GameObject>(obj);
                return res;
            }
            Debug.LogError(transform.name + " AddComponent fail, obj.name: " + obj.name);
            return null;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">unity游戏物体</param>
        public T AddComponent<T, A>(GameObject obj, A a) where T : UIBaseComponent
        {

            if (obj != null)
            {
                var res = base.AddComponent<T, A>(obj.name, a);
                res.AddComponent<UIBaseComponent, GameObject>(obj);
                return res;
            }
            Debug.LogError(transform.name + " AddComponent fail, obj.name: " + obj.name);
            return null;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">unity游戏物体</param>
        public T AddComponent<T, A, B>(GameObject obj, A a, B b) where T : UIBaseComponent
        {

            if (obj != null)
            {
                var res = base.AddComponent<T, A, B>(obj.name, a, b);
                res.AddComponent<UIBaseComponent, GameObject>(obj);
                return res;
            }
            Debug.LogError(transform.name + " AddComponent fail, obj.name: " + obj.name);
            return null;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">unity游戏物体</param>
        public T AddComponent<T, A, B, C>(GameObject obj, A a, B b, C c) where T : UIBaseComponent
        {

            if (obj != null)
            {
                var res = base.AddComponent<T, A, B, C>(obj.name, a, b, c);
                res.AddComponent<UIBaseComponent, GameObject>(obj);
                return res;
            }
            Debug.LogError(transform.name + " AddComponent fail, obj.name: " + obj.name);
            return null;
        }
        /// <summary>
        /// 获取组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>(string relative_path = "") where T : UIBaseComponent
        {
            return InnerGetComponent<T>(relative_path);
        }

        /// <summary>
        /// 移除组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        public void RemoveComponent<T>(string relative_path = "") where T : UIBaseComponent
        {
            InnerRemoveComponent<T>(relative_path);
        }
    }
}
