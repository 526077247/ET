﻿using System;

namespace ET
{
    public interface IOnCreateSystem : ISystemType
    {
        void Run(object o);
    }

    public interface IOnCreateSystem<A> : ISystemType
    {
        void Run(object o, A a);
    }

    public interface IOnCreateSystem<A, B> : ISystemType
    {
        void Run(object o, A a, B b);
    }

    public interface IOnCreateSystem<A, B, C> : ISystemType
    {
        void Run(object o, A a, B b, C c);
    }

    public interface IOnCreateSystem<A, B, C, D> : ISystemType
    {
        void Run(object o, A a, B b, C c, D d);
    }

    [UISystem]
    public abstract class OnCreateSystem<T> : IOnCreateSystem where T :UIBaseContainer
    {
        public Type Type()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IOnCreateSystem);
        }

        public void Run(object o)
        {
            this.OnCreate((T)o);
        }

        public abstract void OnCreate(T self);
    }

    [UISystem]
    public abstract class OnCreateSystem<T, A> : IOnCreateSystem<A> where T : UIBaseContainer
    {
        public Type Type()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IOnCreateSystem<A>);
        }

        public void Run(object o, A a)
        {
            this.OnCreate((T)o, a);
        }

        public abstract void OnCreate(T self, A a);
    }

    [UISystem]
    public abstract class OnCreateSystem<T, A, B> : IOnCreateSystem<A, B> where T : UIBaseContainer
    {
        public Type Type()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IOnCreateSystem<A, B>);
        }

        public void Run(object o, A a, B b)
        {
            this.OnCreate((T)o, a, b);
        }

        public abstract void OnCreate(T self, A a, B b);
    }

    [UISystem]
    public abstract class OnCreateSystem<T, A, B, C> : IOnCreateSystem<A, B, C> where T : UIBaseContainer
    {
        public Type Type()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IOnCreateSystem<A, B, C>);
        }

        public void Run(object o, A a, B b, C c)
        {
            this.OnCreate((T)o, a, b, c);
        }

        public abstract void OnCreate(T self, A a, B b, C c);
    }
}