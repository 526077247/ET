using System;

namespace ET
{
    public interface IOnEnableSystem : ISystemType
    {
        void Run(object o);
    }

    public interface IOnEnableSystem<A> : ISystemType
    {
        void Run(object o, A a);
    }

    public interface IOnEnableSystem<A, B> : ISystemType
    {
        void Run(object o, A a, B b);
    }

    public interface IOnEnableSystem<A, B, C> : ISystemType
    {
        void Run(object o, A a, B b, C c);
    }

    public interface IOnEnableSystem<A, B, C, D> : ISystemType
    {
        void Run(object o, A a, B b, C c, D d);
    }

    [UISystem]
    public abstract class OnEnableSystem<T> : IOnEnableSystem where T :UIBaseContainer
    {
        public Type Type()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IOnEnableSystem);
        }

        public void Run(object o)
        {
            this.OnEnable((T)o);
        }

        public abstract void OnEnable(T self);
    }

    [UISystem]
    public abstract class OnEnableSystem<T, A> : IOnEnableSystem<A> where T : UIBaseContainer
    {
        public Type Type()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IOnEnableSystem<A>);
        }

        public void Run(object o, A a)
        {
            this.OnEnable((T)o, a);
        }

        public abstract void OnEnable(T self, A a);
    }

    [UISystem]
    public abstract class OnEnableSystem<T, A, B> : IOnEnableSystem<A, B> where T : UIBaseContainer
    {
        public Type Type()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IOnEnableSystem<A, B>);
        }

        public void Run(object o, A a, B b)
        {
            this.OnEnable((T)o, a, b);
        }

        public abstract void OnEnable(T self, A a, B b);
    }

    [UISystem]
    public abstract class OnEnableSystem<T, A, B, C> : IOnEnableSystem<A, B, C> where T : UIBaseContainer
    {
        public Type Type()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IOnEnableSystem<A, B, C>);
        }

        public void Run(object o, A a, B b, C c)
        {
            this.OnEnable((T)o, a, b, c);
        }

        public abstract void OnEnable(T self, A a, B b, C c);
    }
}