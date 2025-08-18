using System;
using System.Threading.Tasks;
using Managers;
using UnityEngine;

namespace UI
{
    public class ExplosionEffect : MonoBehaviour, IPooledObject
    {
        [SerializeField] private GameObject blueEffects;
        [SerializeField] private GameObject greenEffects;
        [SerializeField] private GameObject redEffects;
        [SerializeField] private GameObject orangeEffects;

        private ObjectPoolManager _objectPoolManager;
        
        public void Init(EffectName effectName, Vector3 pos)
        {
            switch (effectName)
            {
                case EffectName.BlueExplosion: blueEffects.gameObject.SetActive(true); break;
                case EffectName.RedExplosion: redEffects.gameObject.SetActive(true); break;
                case EffectName.OrangeExplosion: orangeEffects.gameObject.SetActive(true); break;
                case EffectName.GreenExplosion: greenEffects.gameObject.SetActive(true); break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(effectName), effectName, $"Explosion has no GameObject for {effectName}");
            }

            transform.position = pos;
            _ = DelayedDisable(TimeSpan.FromSeconds(3));
        }
        
        private async Task DelayedDisable(TimeSpan delay)
        {
            await Task.Delay(delay);
            OnDeactivate();
        }

        private void ResetEffects()
        {
            blueEffects.gameObject.SetActive(false);
            greenEffects.gameObject.SetActive(false);
            orangeEffects.gameObject.SetActive(false);
            redEffects.gameObject.SetActive(false);
        }
        
        public void OnCreate()
        {
            _objectPoolManager = Injection.GetManager<ObjectPoolManager>();
            gameObject.SetActive(false);
        }

        public void OnActivate()
        {
            ResetEffects();
            gameObject.SetActive(true);
        }

        public void OnDeactivate()
        {
            ResetEffects();
            _objectPoolManager.GetPool(ObjectPoolType.ExplosionEffect).ReturnObject(this);
            gameObject.SetActive(false);
        }
    }
}
