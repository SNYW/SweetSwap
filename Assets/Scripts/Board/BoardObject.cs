using Settings;
using UnityEngine;

namespace Board
{
    public class BoardObject : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        private BoardObjectDefinition _definition;

        public void Init(BoardObjectDefinition definition)
        {
            _definition = definition;
            spriteRenderer.sprite = _definition.sprite;
        }
    }
}
