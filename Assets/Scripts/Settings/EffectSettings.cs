using UI;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(menuName = "Game Data/Effect Settings")]
    public class EffectSettings : ScriptableObject
    {
        public ExplosionEffect explosionEffectPrefab;
    }
}