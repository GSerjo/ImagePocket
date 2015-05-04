using System;

namespace Core
{
    public static class ObjectExtensions
    {
        public static void BeginRaiseEvent<T>(this object sender, EventHandler<T> eventToRaise, T eventArgs)
            where T : EventArgs
        {
            if (eventToRaise == null)
            {
                return;
            }
            foreach (EventHandler<T> handler in eventToRaise.GetInvocationList())
            {
                EventHandler<T> handlerTemp = handler;
                handler.BeginInvoke(
                    sender,
                    eventArgs,
                    ar =>
                    {
                        try
                        {
                            handlerTemp.EndInvoke(ar);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    },
                    null);
            }
        }

        public static void BeginRaiseEvent<T>(
            this object sender, EventHandler<T> eventToRaise, Func<T> eventArgsCreator)
            where T : EventArgs
        {
            if (eventToRaise == null)
            {
                return;
            }
            foreach (EventHandler<T> handler in eventToRaise.GetInvocationList())
            {
                EventHandler<T> handlerTemp = handler;
                handler.BeginInvoke(
                    sender,
                    eventArgsCreator(),
                    ar =>
                    {
                        try
                        {
                            handlerTemp.EndInvoke(ar);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    },
                    null);
            }
        }

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
    }
}
