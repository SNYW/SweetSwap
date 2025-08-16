using System;
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
            _allowInput = true;
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
            try
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
            catch (Exception e)
            {
                Debug.LogError(e); 
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