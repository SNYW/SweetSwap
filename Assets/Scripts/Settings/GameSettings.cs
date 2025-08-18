using Board;
using UI;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "New GameSettings", menuName = "Game Data/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        public Vector2Int baseGridDimensions;
        public int roundDuration;
        public SelectionIndicator selectionIndicatorPrefab;
        public FadePanel fadePanelPrefab;
        public BoardObjectSettings boardObjectSettings;
    }
}