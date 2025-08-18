using System.Threading.Tasks;
using Board;
using UI;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private GameEndPanel gameEndPanel;
        
        private SelectionIndicator _selectionIndicator;
        private BoardObject _selectedBoardObject;
        private GridManager _gridManager;
        private TimerManager _timerManager;
        private SettingsManager _settingsManager;
        private ScoreManager _scoreManager;
        
        private bool _allowInput;
        private bool _isPlaying;
        private bool _isAnimating;
        private Vector2 _tapStartPos;

        private void Awake()
        {
            _allowInput = true;
            _isPlaying = true;
            gameEndPanel.gameObject.SetActive(false);
        }

        private void Start()
        {
            _gridManager = Injection.GetManager<GridManager>();
            _timerManager = Injection.GetManager<TimerManager>();
            _settingsManager = Injection.GetManager<SettingsManager>();
            _scoreManager = Injection.GetManager<ScoreManager>();
            _timerManager.ResetTimer();
            _timerManager.OnTimerFinished += OnTimerFinished;
            _selectionIndicator = Instantiate(_settingsManager.ActiveSettings.selectionIndicatorPrefab);
        }

        public void ResetGame()
        {
            gameEndPanel.gameObject.SetActive(false);
            _selectedBoardObject = null;
            _selectionIndicator.OnCellDeselected();
            _timerManager.ResetTimer();
            _scoreManager.ResetScore();
            _gridManager.ResetBoard();
            _allowInput = true;
            _isPlaying = true;
            _isAnimating = false;
        }

        private void Update()
        {
            if (!_allowInput || !_isPlaying) return;
            
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
                _ = isSwipeDistance ? OnSwipe() : OnTap();
            }
        }

        private async Task OnTap()
        {
            _allowInput = false;

            if (!TrySelectBoardObject(Camera.main.ScreenToWorldPoint(Input.mousePosition), out var endObject)) return;
                
            if (_selectedBoardObject.ParentCell.ID == endObject.ParentCell.ID)
            {
                _allowInput = true;
                return;
            }
                
            _selectionIndicator.OnCellDeselected();

            _isAnimating = true;
            await _gridManager.SwapCells(_selectedBoardObject.ParentCell, endObject.ParentCell);

            ExitTap();
        }
        
        private async Task OnSwipe()
        {
            _allowInput = false;

            var swipeEndPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            if (!TrySelectBoardObject(swipeEndPoint,out var endObject)){ExitTap(); return;}
            if (_selectedBoardObject == endObject){ExitTap(); return;}
            
            _selectionIndicator.OnCellDeselected();
            await _gridManager.SwapCells(_selectedBoardObject.ParentCell, endObject.ParentCell);
            ExitTap();
        }

        private void ExitTap()
        {
            _tapStartPos = Vector2.zero;
            _selectedBoardObject = null;
            _selectionIndicator.OnCellDeselected();
            _allowInput = _isPlaying;
            _isAnimating = false;
            if(!_isPlaying) EndGame();
        }

        private bool TrySelectBoardObject(Vector2 position, out BoardObject boardObject)
        {
            boardObject = null;
            var hit = Physics2D.Raycast(position, Vector2.zero);
            return hit.collider != null && hit.collider.TryGetComponent(out boardObject);
        }
        
        private void OnTimerFinished()
        {
            _isPlaying = false;
            if(!_isAnimating) EndGame();
        }

        private void EndGame()
        {
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
            Injection.Dispose();
        }
    }
}