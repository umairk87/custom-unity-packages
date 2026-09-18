using System;
using System.Collections.Generic;

namespace UKFramework.Game
{
    public static class ServiceLocator
    {
        private static Dictionary<Type, object> services =
        new();

        public static void Register<T>(T service)
        {
            services[typeof(T)] = service;
        }

        public static T Get<T>()
        {
            return (T)services[typeof(T)];
        }

        public static bool Has<T>()
        {
            return services.ContainsKey(typeof(T));
        }

        public static void Unregister<T>()
        {
            services.Remove(typeof(T));
        }

        public static void Clear()
        {
            services.Clear();
        }


    }

}