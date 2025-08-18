using System;
using System.Threading;
using System.Threading.Tasks;
using Managers;
using Settings;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Board
{
    public class BoardObject : MonoBehaviour, IPooledObject
    {
        public BoardObjectDefinition definition;
        public GridCell ParentCell;

        private GameObject _visuals;
        private ObjectPoolManager _objectPoolManager;
        private EffectsManager _effectsManager;
        private Collider2D _collider;
        private Rigidbody2D _rigidbody;
        private SpriteRenderer[] _spriteRenderers;
        private Animator _animator;
        private CancellationTokenSource _cts;

        public void Init(BoardObjectDefinition definition)
        {
            if (_visuals != null) Destroy(_visuals);
            this.definition = definition;
            _visuals = Instantiate(definition.visualPrefab, transform.position, Quaternion.identity, transform);
            _spriteRenderers = _visuals.GetComponentsInChildren<SpriteRenderer>();
            _animator = _visuals.GetComponent<Animator>();

            _ = PlayDelayedAwake();
        }

        public async Task Move()
        {
            if (ParentCell == null) return;
            if (transform.position == ParentCell.WorldPosition) return;

            var startPos = transform.position;
            var currentTime = 0f;
            var duration = 0.2f;

            try
            {
                while (currentTime < duration && !_cts.Token.IsCancellationRequested)
                {
                    currentTime += Time.deltaTime;
                    var progress = Mathf.Clamp01(currentTime / duration);
                    transform.position = Vector3.Lerp(startPos, ParentCell.WorldPosition, progress);
                    await Task.Yield();
                }
            }
            catch (OperationCanceledException) { }

            if (!_cts.Token.IsCancellationRequested) transform.position = ParentCell.WorldPosition;
        }

        public void OnKill()
        {
            ApplyDeathPhysics();
            _effectsManager.GenerateEffect(definition.matchEffectName, ParentCell.WorldPosition);
            foreach (var sr in _spriteRenderers) sr.sortingOrder += 10;
            _ = DelayedDisable(TimeSpan.FromSeconds(3));
        }

        private void ApplyDeathPhysics()
        {
            _collider.enabled = false;
            _rigidbody.gravityScale = 3;
            var randomForce = new Vector2(
                Random.Range(-2f, 2f),
                Random.Range(5f, 8f)
            );
            _rigidbody.AddForce(randomForce, ForceMode2D.Impulse);
            _rigidbody.AddTorque(Random.Range(-1000, 1000));
        }

        private async Task PlayDelayedAwake()
        {
            if (_animator == null) return;

            _animator.speed = 0;
            try
            {
                await Task.Delay(Random.Range(0, 300), _cts.Token);
            }
            catch (OperationCanceledException) { }
            if (!_cts.Token.IsCancellationRequested)
                _animator.speed = 1;
        }

        private async Task DelayedDisable(TimeSpan delay)
        {
            try
            {
                await Task.Delay(delay, _cts.Token);
            }
            catch (OperationCanceledException) { }
            if (!_cts.Token.IsCancellationRequested)
                OnDeactivate();
        }

        public void OnCreate()
        {
            _collider = GetComponent<Collider2D>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _objectPoolManager = Injection.GetManager<ObjectPoolManager>();
            _effectsManager = Injection.GetManager<EffectsManager>();
            gameObject.SetActive(false);
        }

        public void OnActivate()
        {
            _cts = new CancellationTokenSource();

            gameObject.SetActive(true);

            _collider.enabled = true;
            _rigidbody.gravityScale = 0;
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.angularVelocity = 0;
            transform.rotation = Quaternion.identity;
            if (_animator != null) _animator.speed = 1;
        }

        public void OnDeactivate()
        {
            _objectPoolManager.GetPool(ObjectPoolType.BoardObject).ReturnObject(this);
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }
    }
}