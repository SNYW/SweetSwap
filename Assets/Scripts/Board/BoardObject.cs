using Settings;
using UnityEngine;

namespace Board
{
    public class BoardObject : MonoBehaviour
    {
        public BoardObjectDefinition definition;
        private GameObject _visuals;
        public GridCell parentCell;
        public void Init(BoardObjectDefinition definition)
        {
            this.definition = definition;
            _visuals = Instantiate(this.definition.visualPrefab, transform.position, Quaternion.identity, transform);
            
            _visuals.GetComponent<Animator>().speed = 0;
            Invoke(nameof(PlayDelayedAwake), Random.Range(0, 0.3f));
        }

        private void PlayDelayedAwake()
        {
            _visuals.GetComponent<Animator>().speed = 1;
        }
    }
}
