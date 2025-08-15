using Board;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "New BOSettings", menuName = "Game Data/Board Object Settings")]
    public class BoardObjectSettings : ScriptableObject
    {
        public BoardObject baseObjectPrefab;
        public BoardObjectDefinition[] activeBoardObjects;
    }
}