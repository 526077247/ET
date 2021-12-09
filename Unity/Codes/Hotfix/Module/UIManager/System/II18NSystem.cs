using System;

namespace ET
{
    public interface II18NSystem : ISystemType
    {
        void Run(object o);
    }

    
    [UISystem]
    public abstract class I18NSystem<T> : II18NSystem where T :Entity
    {
        public Type Type()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(II18NSystem);
        }

        public void Run(object o)
        {
            this.OnLanguageChange((T)o);
        }

        public abstract void OnLanguageChange(T self);
    }

   
}