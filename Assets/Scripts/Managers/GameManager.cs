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

        private void Awake()
        {
            Injection.Init();
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
            Instantiate(_settingsManager.ActiveSettings.selectionIndicatorPrefab);
            _selectionIndicator = FindAnyObjectByType<SelectionIndicator>();
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
        }

        private void Update()
        {
            if (_isPlaying && _allowInput && Input.GetMouseButtonDown(0)) OnTap();
        }

        private async void OnTap()
        {
            _allowInput = false;

            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider != null && hit.collider.TryGetComponent<BoardObject>(out var component))
            {
                if (_selectedBoardObject == null)
                {
                    SelectCell(component);
                    _allowInput = true;
                    return;
                }
                
                if (_selectedBoardObject.ParentCell.ID == component.ParentCell.ID)
                {
                    _allowInput = true;
                    return;
                }
                
                _selectionIndicator.OnCellDeselected();

                _isAnimating = true;
                if (await _gridManager.TrySwap(_selectedBoardObject.ParentCell, component.ParentCell))
                {
                    _selectedBoardObject = null;
                    await _gridManager.UpdateBoardState();
                }
            }

            _selectedBoardObject = null;
            _allowInput = _isPlaying;
            _isAnimating = false;
            if(!_isPlaying) EndGame();
        }

        private void SelectCell(BoardObject cell)
        {
            _selectedBoardObject = cell;
            _selectionIndicator.OnCellSelected(cell);
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