using System;
using System.Collections.Generic;
using Settings;
using Object = UnityEngine.Object;

namespace Managers
{
    public enum ObjectPoolType
    {
        BoardObject
    }

    public class ObjectPool<T> where T : IPooledObject
    {
        private readonly Func<T> _create;
        private Queue<IPooledObject> _pooledObjects;
        
        public ObjectPool(Func<T> create)
        {
            _pooledObjects = new Queue<IPooledObject>();
            _create = create;
        }

        private void CreateNewObject()
        {
            var newObject = _create();
            newObject.OnCreate();
            _pooledObjects.Enqueue(newObject);
        }

        public IPooledObject GetObject()
        {
            if (_pooledObjects.Count == 0)
            {
                CreateNewObject();
            }
            var objectToReturn = _pooledObjects.Dequeue();
            objectToReturn.OnActivate();
            return objectToReturn;
        }

        public void ReturnObject(IPooledObject obj)
        {
            _pooledObjects.Enqueue(obj);
        }

        public void Clear()
        {
            _pooledObjects.Clear();
        }
    }

    public interface IPooledObject
    {
        public void OnCreate();
        public void OnActivate();
        public void OnDeactivate();
    }
    
    public class ObjectPoolManager : IManager
    {
        private readonly GameSettings _settings = Injection.GetManager<SettingsManager>().ActiveSettings;
        private Dictionary<ObjectPoolType, ObjectPool<IPooledObject>> _pools;
        public void Init()
        {
            _pools ??= new Dictionary<ObjectPoolType, ObjectPool<IPooledObject>>();

            foreach (ObjectPoolType poolType in Enum.GetValues(typeof(ObjectPoolType)))
            {
                _pools.Add(poolType, CreatePool(poolType));
            }
        }

        public void PostInit()
        {
           
        }

        private ObjectPool<IPooledObject> CreatePool(ObjectPoolType poolType)
        {
            return new ObjectPool<IPooledObject>(GetFactoryMethod(poolType));
        }

        public T GetObject<T>(ObjectPoolType poolType) where T : IPooledObject
        {
            if (_pools.TryGetValue(poolType, out var pool)) return (T)pool.GetObject();

            var newPool = CreatePool(poolType);
            _pools.Add(poolType, newPool);
            return (T)newPool.GetObject();
        }

        private Func<IPooledObject> GetFactoryMethod(ObjectPoolType objectPoolType)
        {
            return objectPoolType switch
            {
                ObjectPoolType.BoardObject => () => Object.Instantiate(_settings.boardObjectSettings.baseObjectPrefab),
                _ => throw new ArgumentOutOfRangeException(nameof(objectPoolType), objectPoolType, $"No factory method set up for {objectPoolType}")
            };
        }

        public void Dispose()
        {
            if (_pools == null) return;
            _pools.Clear();
            _pools = null;
        }

        public ObjectPool<IPooledObject> GetPool(ObjectPoolType objectPoolType)
        {
            return _pools[objectPoolType];
        }

        public void ClearPools()
        {
            foreach (var pool in _pools.Values)
            {
                pool?.Clear();
            }
        }
    }
}