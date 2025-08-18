using System.Threading.Tasks;
using Managers;
using Settings;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Board
{
    public class BoardObject : MonoBehaviour, IPooledObject
    {
        public BoardObjectDefinition definition;
        public TMP_Text debugText;
        private GameObject _visuals;
        public GridCell ParentCell;
        private ObjectPoolManager _objectPoolManager;
        
        public void Init(BoardObjectDefinition definition)
        {
            if(_visuals != null) Destroy(_visuals.gameObject);
            _objectPoolManager = Injection.GetManager<ObjectPoolManager>();
            this.definition = definition;
            _visuals = Instantiate(this.definition.visualPrefab, transform.position, Quaternion.identity, transform);
            _ = PlayDelayedAwake();
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
            var currentTime = 0f;
            var duration = 0.2f;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                var progress = Mathf.Clamp01(currentTime / duration);
                transform.position = Vector3.Lerp(startPos, ParentCell.WorldPosition, progress);
                await Task.Yield();
            }

            transform.position = ParentCell.WorldPosition;
        }

        public void OnKill()
        {
            var col = GetComponent<Collider2D>();
            var rb = GetComponent<Rigidbody2D>();

            col.isTrigger = true;
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

            _ = DelayedDisable(3000);
        }

        private async Task PlayDelayedAwake()
        {
            _visuals.GetComponent<Animator>().speed = 0;
            await Task.Delay(Random.Range(0, 300));
            _visuals.GetComponent<Animator>().speed = 1;
        }
        
        private async Task DelayedDisable(int delayTime)
        {
            await Task.Delay(delayTime);
            OnDeactivate();
        }

        public void OnCreate()
        {
           gameObject.SetActive(false);
        }

        public void OnActivate()
        {
            gameObject.SetActive(true);
            var col = GetComponent<Collider2D>();
            var rb = GetComponent<Rigidbody2D>();

            col.isTrigger = false;
            rb.gravityScale = 0;
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        public void OnDeactivate()
        {
            _objectPoolManager.GetPool(ObjectPoolType.BoardObject).ReturnObject(this);
            gameObject.SetActive(false);
        }
    }
}
