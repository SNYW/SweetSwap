using System;
using System.Threading.Tasks;
using Settings;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Board
{
    public class BoardObject : MonoBehaviour
    {
        public BoardObjectDefinition definition;
        public TMP_Text debugText;
        private GameObject _visuals;
        public GridCell ParentCell;
        
        public void Init(BoardObjectDefinition definition)
        {
            this.definition = definition;
            _visuals = Instantiate(this.definition.visualPrefab, transform.position, Quaternion.identity, transform);
            
            _visuals.GetComponent<Animator>().speed = 0;
            Invoke(nameof(PlayDelayedAwake), Random.Range(0, 0.3f));
        }

        private void Update()
        {
            debugText.gameObject.SetActive(Input.GetKey(KeyCode.Space));
            debugText.text = ParentCell != null ? $"[{ParentCell.ID.x},{ParentCell.ID.y}]" : "";
        }
        
        public async Task Move()
        {
            if (transform.position == ParentCell.WorldPosition) return;
            
            Vector3 startPos = transform.position;
            float elapsed = 0f;
            float duration = 0.3f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, ParentCell.WorldPosition, elapsed/ duration);
                await Task.Yield();
            }

            transform.position = ParentCell.WorldPosition;
        }

        private void PlayDelayedAwake()
        {
            _visuals.GetComponent<Animator>().speed = 1;
        }
    }
}
