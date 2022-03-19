using System;

namespace Raid.Toolkit.DataModel
{
    public class ApiEventHandler<T> where T : EventArgs
    {
        private event EventHandler<T> Sink;
        public static ApiEventHandler<T> operator +(ApiEventHandler<T> left, EventHandler<T> right)
        {
            left.Sink += right;
            return left;
        }
        public static ApiEventHandler<T> operator -(ApiEventHandler<T> left, EventHandler<T> right)
        {
            left.Sink -= right;
            return left;
        }
    }
}