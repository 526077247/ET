using System;
using System.Collections.Generic;

namespace ET
{
    public class Messager
    {
        static readonly Messager instance = new Messager();
        public static Messager Instance { get => instance; }

        readonly Dictionary<string, LinkedList<MulticastDelegate>> evts = new Dictionary<string, LinkedList<MulticastDelegate>>();

        public void AddListener<T>(string name, T evt) where T : MulticastDelegate
        {
            if (!evts.ContainsKey(name))
                evts.Add(name, new LinkedList<MulticastDelegate>());
            evts[name].AddLast(evt);
        }

        public void RemoveListener<T>(string name, T evt) where T : MulticastDelegate
        {
            if (evts.ContainsKey(name))
            {
                evts[name].Remove(evt);
            }
        }

        public void Broadcast<T>(string name, params object[] param) where T : MulticastDelegate
        {
            if (evts.TryGetValue(name, out var evt))
            {
                foreach (var item in evt)
                {
                    (item as T)?.DynamicInvoke(param);
                }
            }
        }
    }
}