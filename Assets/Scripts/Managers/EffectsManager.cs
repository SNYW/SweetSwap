using System;
using UI;
using UnityEngine;

namespace Managers
{
    public enum EffectName
    {
        BlueExplosion
    }
    
    public class EffectsManager : IManager
    {
        private readonly ObjectPoolManager _objectPoolManager = Injection.GetManager<ObjectPoolManager>();
        
        public void Init() { }

        public void PostInit() { }

        public void GenerateEffect(EffectName effectName, Vector3 position)
        {
            switch (effectName)
            {
                case EffectName.BlueExplosion: GenerateExplosion(effectName, position); break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(effectName), effectName, null);
            }
        }

        private void GenerateExplosion(EffectName effectName, Vector3 position)
        {
            var newExplosion = _objectPoolManager.GetObject<ExplosionEffect>(ObjectPoolType.ExplosionEffect);
            newExplosion.Init(effectName, position);
        }

        public void Dispose() { }
    }
}