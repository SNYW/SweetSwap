using System;
using System.Threading.Tasks;
using Board;
using UI;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private enum GameState
        {
            Loading,
            Playing,
            AwaitingAnimations,
            Ended
        }
        
        [SerializeField]
        private GameEndPanel gameEndPanel;
        
        private SelectionIndicator _selectionIndicator;
        private BoardObject _selectedBoardObject;
        private GridManager _gridManager;
        private TimerManager _timerManager;
        private SettingsManager _settingsManager;
        private ScoreManager _scoreManager;
        private ObjectPoolManager _objectPoolManager;
        
        private Vector2 _tapStartPos;
        private GameState _gameState;

        private void Awake()
        {
            UpdateGameState(GameState.Loading);
            gameEndPanel.gameObject.SetActive(false);
            _gridManager = Injection.GetManager<GridManager>();
            _timerManager = Injection.GetManager<TimerManager>();
            _settingsManager = Injection.GetManager<SettingsManager>();
            _scoreManager = Injection.GetManager<ScoreManager>();
            _objectPoolManager = Injection.GetManager<ObjectPoolManager>();
            _gridManager.InitializeGrid();
            _scoreManager.ResetScore();
            _timerManager.ResetTimer();
            _timerManager.OnTimerFinished += OnTimerFinished;
            _selectionIndicator = Instantiate(_settingsManager.ActiveSettings.selectionIndicatorPrefab);
            UpdateGameState(GameState.Playing);
        }

        public void ResetGame()
        {
            gameEndPanel.gameObject.SetActive(false);
            _selectedBoardObject = null;
            _selectionIndicator.OnCellDeselected();
            _timerManager.ResetTimer();
            _scoreManager.ResetScore();
            _gridManager.ResetBoard();
            UpdateGameState(GameState.Playing);
        }

        private void Update()
        {
            if (_gameState != GameState.Playing) return;

            if (Input.GetMouseButtonDown(0))
            {
                _tapStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (_selectedBoardObject == null && TrySelectBoardObject(_tapStartPos, out _selectedBoardObject))
                {
                    _selectionIndicator.OnCellSelected(_selectedBoardObject);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                var tapEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var isSwipeDistance = Vector2.Distance(_tapStartPos, tapEndPos) > 0.45f;
                RunSafeAsync(isSwipeDistance ? OnSwipe() : OnTap());
            }
        }

        private void UpdateGameState(GameState newState)
        {
            _gameState = newState;
        }

        private async Task OnTap()
        {
            try
            {
                UpdateGameState(GameState.AwaitingAnimations);
                
                if (_selectedBoardObject?.ParentCell == null)
                {
                    ExitTap();
                    return;
                }
                
                if (_selectedBoardObject == null || !TrySelectBoardObject(Camera.main.ScreenToWorldPoint(Input.mousePosition), out var endObject))
                {
                    ExitTap();
                    return;
                }
                
                if (_selectedBoardObject?.ParentCell?.ID == endObject?.ParentCell?.ID)
                {
                    UpdateGameState(GameState.Playing);
                    return;
                }
                
                _selectionIndicator.OnCellDeselected();
                await _gridManager.SwapCells(_selectedBoardObject.ParentCell, endObject.ParentCell);
                ExitTap();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[OnTap] Exception: {ex}");
                throw;
            }
        }
        
        private async Task OnSwipe()
        {
            try
            {
                if(_selectedBoardObject?.ParentCell == null)
                {
                    ExitTap();
                    return;
                }
                
                UpdateGameState(GameState.AwaitingAnimations);
                var swipeEndPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var swipeDirection = GetSwipeDirection(_tapStartPos, swipeEndPoint);
                
                if (!_gridManager.TryGetBoardObjectByCellId(_selectedBoardObject.ParentCell.ID + swipeDirection, out BoardObject endObject))
                {
                    ExitTap();
                    return;
                }
                
                _selectionIndicator.OnCellDeselected();
                await _gridManager.SwapCells(_selectedBoardObject.ParentCell, endObject.ParentCell);
                ExitTap();
            }
            catch (Exception e)
            {
                Debug.LogError($"[OnSwipe] Exception: {e}");
                throw;
            }
        }

        private static Vector2Int GetSwipeDirection(Vector2 swipeStart, Vector2 swipeEnd)
        {
            var swipeVector = swipeEnd - swipeStart;
            if (swipeVector == Vector2.zero) return Vector2Int.zero;

            if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
            {
                return swipeVector.x > 0 ? Vector2Int.right : Vector2Int.left;
            }
            
            return swipeVector.y > 0 ? Vector2Int.down : Vector2Int.up;
        }

        private void ExitTap()
        {
            _tapStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _selectedBoardObject = null;
            _selectionIndicator.OnCellDeselected();
            if (_gameState == GameState.Ended)
            {
                EndGame();
                return;
            }
            UpdateGameState(GameState.Playing);
        }

        private bool TrySelectBoardObject(Vector2 position, out BoardObject boardObject)
        {
            boardObject = null;
            var hit = Physics2D.Raycast(position, Vector2.zero, _settingsManager.ActiveSettings.boardObjectSettings.boardObjectLayerMask);
            return hit.collider != null && hit.collider.TryGetComponent(out boardObject);
        }
        
        private void OnTimerFinished()
        {
            if (_gameState == GameState.Playing)
            {
                EndGame();
                return;
            }
            UpdateGameState(GameState.Ended);
        }

        private void EndGame()
        {
            UpdateGameState(GameState.Ended);
            _selectionIndicator.OnCellDeselected();
            _selectedBoardObject = null;
            
            var previousScore = PlayerPrefs.GetInt("Score", 0);
            var currentScore = _scoreManager.GetScore();

            var isHighScore = currentScore > previousScore;
            if(isHighScore) PlayerPrefs.SetInt("Score", currentScore);
            
            gameEndPanel.gameObject.SetActive(true);
            gameEndPanel.Init(isHighScore, currentScore.ToString());
        }

        private void OnDisable()
        {
            _timerManager.OnTimerFinished -= OnTimerFinished;
            _objectPoolManager.ClearPools();
        }

        private async void RunSafeAsync(Task task)
        {
            try
            {
                await task;
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameManager] Unhandled Exception in async method: {e}");
                throw;
            }
        }
    }
}
