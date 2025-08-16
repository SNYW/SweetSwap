using Board;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private SelectionIndicator _selectionIndicator;
        private BoardObject _selectedBoardObject;
        private GridManager _gridManager;

        private bool _allowInput;
        
        private void Awake()
        {
            Injection.Init();
            _selectionIndicator = FindAnyObjectByType<SelectionIndicator>();
            _gridManager = Injection.GetManager<GridManager>();
            _allowInput = false;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnTap();
            }
        }

        private void OnTap()
        {
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
                    if (_gridManager.TrySwap(_selectedBoardObject.parentCell, component.parentCell))
                    {
                        OnRelease();
                        // Check Board State
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