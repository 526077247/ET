using System;
using System.Collections.Generic;

namespace ET
{
    public class Messager
    {
        public static Messager Instance { get; private set; } = new Messager();
        class Event
        {
            public object sender;
            public EventArgs args;
            public MessagerId name;
        }

        readonly Dictionary<MessagerId, LinkedList<Action<object,EventArgs>>> evts = new Dictionary<MessagerId, LinkedList<Action<object, EventArgs>>>(MessagerIdComparer.Instance);

        readonly Queue<Event> eventsQueue= new Queue<Event>();

        readonly Queue<Event> eventsPool = new Queue<Event>();

        Event GetEvent()
        {
            lock (eventsPool)
            {
                if (eventsPool.Count > 0)
                {
                    return eventsPool.Dequeue();
                }
                else return new Event();
            }
        }

        void RecycelEvent(Event evt)
        {
            evt.args = null;
            evt.sender = null;
            eventsPool.Enqueue(evt);
        }
        private void Update()
        {
            lock (eventsQueue)
            {
                while (eventsQueue.Count > 0)
                {
                    Event eventNode = eventsQueue.Dequeue();
                    BroadcastImmediate(eventNode.name, eventNode.sender,eventNode.args);
                    RecycelEvent(eventNode);
                }
            }
        }
        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="name"></param>
        /// <param name="evt"></param>
        public void AddListener(MessagerId name, Action<object, EventArgs> evt)
        {
            if (!evts.ContainsKey(name))
                evts.Add(name, new LinkedList<Action<object, EventArgs>>());
            evts[name].AddLast(evt);
        }
        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="name"></param>
        /// <param name="evt"></param>
        public void RemoveListener(MessagerId name, Action<object, EventArgs> evt)
        {
            if (evts.ContainsKey(name))
            {
                evts[name].Remove(evt);
            }
        }

        /// <summary>
        /// 抛出事件下一帧执行（线程安全的）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void Broadcast(MessagerId name, object sender = null, EventArgs args = null)
        {
            Event evt = GetEvent();
            evt.args = args;
            evt.sender = sender;
            evt.name = name;
            eventsQueue.Enqueue(evt);
        }

        /// <summary>
        /// 抛出事件立刻执行（线程不安全的）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void BroadcastImmediate(MessagerId name, object sender = null, EventArgs args = null)
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