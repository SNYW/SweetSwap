using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public static class Injection
    {
        private static Dictionary<Type, IManager> _cachedManagers;

        public static void Init()
        {
            AddManager<SettingsManager>();
            AddManager<AnimationManager>();
            AddManager<GridManager>();
            AddManager<ScoreManager>();
            AddManager<TimerManager>();
        }

        private static T AddManager<T>() where T : IManager, new()
        {
            _cachedManagers ??= new Dictionary<Type, IManager>();
            
            var type = typeof(T);
            if (_cachedManagers.TryGetValue(type, out var manager))
            {
                Debug.LogWarning($"{type} is already registered.");
                return (T)manager;
            }

            var newManager = new T();
            _cachedManagers.Add(type, newManager);
            newManager.Init();
            return newManager;
        }

        public static T GetManager<T>() where T : IManager, new()
        {
            if (_cachedManagers == null) return AddManager<T>();
            return !_cachedManagers.TryGetValue(typeof(T), out var cachedManager) ? AddManager<T>() : (T)cachedManager;
        }

        public static void Dispose()
        {
            foreach (var cachedManager in _cachedManagers.Values)
            {
                cachedManager.Dispose();
            }

            _cachedManagers.Clear();
            _cachedManagers = null;
        }
    }
}