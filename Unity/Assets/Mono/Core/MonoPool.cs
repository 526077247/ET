using System;
using System.Collections.Generic;

namespace ET
{
    public class MonoPool: IDisposable
    {
        private readonly Dictionary<Type, Queue<object>> pool = new Dictionary<Type, Queue<object>>();
        
        public static MonoPool Instance = new MonoPool();
        
        private MonoPool()
        {
        }

        public object Fetch(Type type)
        {
            Queue<object> queue = null;
            if (!pool.TryGetValue(type, out queue))
            {
#if !NOT_UNITY
                if(type is ILRuntime.Reflection.ILRuntimeType)
                {
                    return CodeLoader.Instance.CreateInstance(type);
                }
#endif
                return Activator.CreateInstance(type);
            }

            if (queue.Count == 0)
            {
#if !NOT_UNITY
                if(type is ILRuntime.Reflection.ILRuntimeType)
                {
                    return CodeLoader.Instance.CreateInstance(type);
                }
#endif
                return Activator.CreateInstance(type);
            }
            return queue.Dequeue();
        }

        public void Recycle(object obj)
        {
            Type type = obj.GetType();
            Queue<object> queue = null;
            if (!pool.TryGetValue(type, out queue))
            {
                queue = new Queue<object>();
                pool.Add(type, queue);
            }
            queue.Enqueue(obj);
        }

        public void Dispose()
        {
            this.pool.Clear();
        }
    }
}