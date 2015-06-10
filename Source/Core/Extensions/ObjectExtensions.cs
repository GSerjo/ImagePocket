using System;

namespace Core
{
    public static class ObjectExtensions
    {
        public static void RaiseEvent<T>(this object sender, EventHandler<T> eventToRaise, T eventArgs)
            where T : EventArgs
        {
            if (eventToRaise == null)
            {
                return;
            }
            eventToRaise(sender, eventArgs);
        }

        public static void RaiseEvent<T>(this object sender, EventHandler<T> eventToRaise, Func<T> eventArgsCreator)
            where T : EventArgs
        {
            if (eventToRaise == null)
            {
                return;
            }
            eventToRaise(sender, eventArgsCreator());
        }

        public static WeakReference<T> ToWeakReference<T>(this T value)
            where T : class
        {
            return new WeakReference<T>(value);
        }
    }
}
