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
            
            var startPos = transform.position;
            var elapsed = 0f;
            var duration = 0.1f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                t = t * t * (3f - 2f * t);
                transform.position = Vector3.Lerp(startPos, ParentCell.WorldPosition, t);
                await Task.Yield();
            }

            transform.position = ParentCell.WorldPosition;
        }

        public void OnKill()
        {
            Destroy(GetComponent<Collider2D>());
            var rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 3;
            var randomForce = new Vector2(
                Random.Range(-2f, 2f), 
                Random.Range(5f, 8f)
            );
            rb.AddForce(randomForce, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-1000, 1000));

            foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
            {
                spriteRenderer.sortingOrder += 10;
            }
            
            Destroy(gameObject, 4);
        }

        private void PlayDelayedAwake()
        {
            _visuals.GetComponent<Animator>().speed = 1;
        }
    }
}
