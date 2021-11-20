using System;

namespace Raid.Service
{
    public class LazyInitializer<T, TContext>
    {
        private readonly Func<TContext, T> m_initFunction;
        private T m_value;
        public LazyInitializer(Func<TContext, T> initFunction)
        {
            m_initFunction = initFunction;
        }

        public T GetValue(TContext context)
        {
            if (m_value == null)
            {
                m_value = m_initFunction(context);
            }
            return m_value;
        }
    }
}
