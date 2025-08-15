using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "New Board Object Definition", menuName = "Game Data/Board Object Definition")]
    public class BoardObjectDefinition : ScriptableObject
    {
        public Sprite sprite;
    }
}