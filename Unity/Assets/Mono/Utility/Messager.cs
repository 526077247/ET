using System;
using System.Collections.Generic;

namespace ET
{
    public class Messager
    {
        static readonly Messager instance = new Messager();
        public static Messager Instance { get => instance; }

        readonly Dictionary<MessagerId, LinkedList<Action<object,EventArgs>>> evts = new Dictionary<MessagerId, LinkedList<Action<object, EventArgs>>>(MessagerIdComparer.Instance);

        public void AddListener(MessagerId name, Action<object, EventArgs> evt)
        {
            if (!evts.ContainsKey(name))
                evts.Add(name, new LinkedList<Action<object, EventArgs>>());
            evts[name].AddLast(evt);
        }

        public void RemoveListener(MessagerId name, Action<object, EventArgs> evt)
        {
            if (evts.ContainsKey(name))
            {
                evts[name].Remove(evt);
            }
        }

        public void Broadcast(MessagerId name, object sender = null, EventArgs args = null)
        {
            if (evts.TryGetValue(name, out var evt))
            {
                foreach (var item in evt)
                {
                    item?.Invoke(sender, args);
                }
            }
        }
    }
}