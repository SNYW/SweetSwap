using UnityEngine;
using UnityEngine.Serialization;

namespace Settings
{
    [CreateAssetMenu(fileName = "New GameSettings", menuName = "Game Data/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        public Vector2Int baseGridDimensions;
        public BoardObjectSettings boardObjectSettings;
    }
}