using System;
using Managers;
using UnityEngine;

namespace UI
{
    public class ExplosionEffect : MonoBehaviour, IPooledObject
    {
        [SerializeField] private GameObject blueEffects;
        [SerializeField] private GameObject greenEffects;
        [SerializeField] private GameObject redEffects;
        
        public void Init(EffectName effectName, Vector3 pos)
        {
            switch (effectName)
            {
                case EffectName.BlueExplosion: blueEffects.gameObject.SetActive(true); break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(effectName), effectName, $"Explosion has no GameObject for {effectName}");
            }

            transform.position = pos;
        }
        
        public void OnCreate()
        {
            gameObject.SetActive(false);
        }

        public void OnActivate()
        {
            gameObject.SetActive(true);
        }

        public void OnDeactivate()
        {
            foreach (var tr in GetComponentsInChildren<Transform>())
            {
                tr.gameObject.SetActive(false);
            }
            gameObject.SetActive(false);
        }
    }
}
