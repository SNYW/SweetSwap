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
            _selectionIndicator = FindAnyObjectByType<SelectionIndicator>();
            _gridManager = Injection.GetManager<GridManager>();
            _timerManager = Injection.GetManager<TimerManager>();
            _settingsManager = Injection.GetManager<SettingsManager>();
            _allowInput = true;
        }

        private void Start()
        {
            _timerManager.ResetTimer();
            Instantiate(_settingsManager.ActiveSettings.selectionIndicatorPrefab);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnTap();
            }
        }

        private async void OnTap()
        {
            if (!_allowInput) return;
            _allowInput = false;

            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider != null && hit.collider.TryGetComponent<BoardObject>(out var component))
            {
                if (_selectedBoardObject == null)
                {
                    SelectCell(component);
                }
                else
                {
                    if (_selectedBoardObject.ParentCell.ID == component.ParentCell.ID)
                    {
                        _allowInput = true;
                        return;
                    }
                    
                    if (await _gridManager.TrySwap(_selectedBoardObject.ParentCell, component.ParentCell))
                    {
                        OnRelease();
                        await _gridManager.UpdateBoardState();
                    }
                    else
                    {
                        SelectCell(component);
                    }
                }
            }
            else
            {
                OnRelease();
            }

            _allowInput = true;
        }

        private void SelectCell(BoardObject cell)
        {
            _selectedBoardObject = cell;
            _selectionIndicator.OnCellSelected(cell);
        }

        private void OnRelease()
        {
            _selectedBoardObject = null;
            _selectionIndicator.OnCellDeselected();
        }

        private void OnDisable()
        {
            Injection.Dispose();
        }
    }
}