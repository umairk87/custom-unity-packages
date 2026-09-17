using System;
using System.Collections.Generic;

namespace UKFramework.Game.Events
{
    public static class EventBus
    {
        private static readonly Dictionary<Type, Delegate> handlers = new();

        public static void Subscribe<T>(Action<T> callback)
        {
            if (callback == null)
                return;

            Type type = typeof(T);

            if (handlers.TryGetValue(type, out Delegate existing))
            {
                handlers[type] =
                    Delegate.Combine(existing, callback);
            }
            else
            {
                handlers[type] = callback;
            }
        }

        public static void Unsubscribe<T>(Action<T> callback)
        {
            if (callback == null)
                return;

            Type type = typeof(T);

            if (!handlers.TryGetValue(type, out Delegate existing))
                return;

            Delegate updated =
                Delegate.Remove(existing, callback);

            if (updated == null)
                handlers.Remove(type);
            else
                handlers[type] = updated;
        }

        public static void Publish<T>(T eventData)
        {
            Type type = typeof(T);

            if (!handlers.TryGetValue(
                    type,
                    out Delegate existing))
                return;

            if (existing is Action<T> callback)
                callback.Invoke(eventData);
        }

        public static void Clear()
        {
            handlers.Clear();
        }
    }
}