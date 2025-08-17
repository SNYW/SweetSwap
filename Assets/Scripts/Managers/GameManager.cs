using Board;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private SelectionIndicator _selectionIndicator;
        private BoardObject _selectedBoardObject;
        private GridManager _gridManager;
        private TimerManager _timerManager;
        private SettingsManager _settingsManager;
        private bool _allowInput;

        private void Awake()
        {
            Injection.Init();
            _allowInput = true;
        }

        private void Start()
        {
            _gridManager = Injection.GetManager<GridManager>();
            _timerManager = Injection.GetManager<TimerManager>();
            _settingsManager = Injection.GetManager<SettingsManager>();
            _timerManager.ResetTimer();
            Instantiate(_settingsManager.ActiveSettings.selectionIndicatorPrefab);
            _selectionIndicator = FindAnyObjectByType<SelectionIndicator>();
        }

        private void Update()
        {
            if (_allowInput && Input.GetMouseButtonDown(0)) OnTap();
        }

        private async void OnTap()
        {
            _allowInput = false;

            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

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
                    
                if (await _gridManager.TrySwap(_selectedBoardObject.ParentCell, component.ParentCell))
                {
                    _selectedBoardObject = null;
                    await _gridManager.UpdateBoardState();
                }
            }

            _selectedBoardObject = null;
            _allowInput = true;
        }

        private void SelectCell(BoardObject cell)
        {
            _selectedBoardObject = cell;
            _selectionIndicator.OnCellSelected(cell);
        }

        private void OnDisable()
        {
            Injection.Dispose();
        }
    }
}