using UnityEngine;

namespace Board
{
    public class SelectionIndicator : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer indicator;

        public void OnCellSelected(BoardObject targetObject)
        {
            transform.position = targetObject.transform.position;
            indicator.gameObject.SetActive(true);
        }
        
        public void OnCellDeselected()
        {
            indicator.gameObject.SetActive(false);
        }
    }
}