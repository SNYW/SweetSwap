using System.Collections.Generic;
using System.Threading.Tasks;
using Board;
using Settings;
using UnityEngine;

namespace Managers
{
    public class GridManager : IManager
    {
        private GridCell[,] _gridCells;
        private GameSettings _gameSettings;
        private AnimationManager _animationManager;
        private ScoreManager _scoreManager;
        private BoardObjectFactory _boardObjectFactory;
        private Matcher _matcher;
        
        public void Init()
        {
            _matcher = new Matcher();
            _boardObjectFactory = new BoardObjectFactory();
        }

        public void PostInit()
        {
            _gameSettings = Injection.GetManager<SettingsManager>().ActiveSettings;
            _animationManager = Injection.GetManager<AnimationManager>();
            _scoreManager = Injection.GetManager<ScoreManager>();
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            InitialiseGridPositions();
            InitialiseBoardObjects();
        }
        
        private void InitialiseGridPositions()
        { 
            var gridDimensions = _gameSettings.baseGridDimensions;
            _gridCells ??= new GridCell[gridDimensions.x, gridDimensions.y];
    
            var offsetX = (gridDimensions.x - 1) / 2f;
            var offsetY = (gridDimensions.y - 1) / 2f;

            for (var y = 0; y < gridDimensions.y; y++)
            {
                for (var x = 0; x < gridDimensions.x; x++)
                {
                    var worldX = x - offsetX;
                    var worldY = gridDimensions.y - 1 - y - offsetY; 

                    var cellId = new Vector2Int(x, y);
                    var worldPos = new Vector3(worldX, worldY, 0);

                    var newGridCell = new GridCell(cellId, worldPos);
                    _gridCells[x, y] = newGridCell;
                }
            }
        }
        
        private void InitialiseBoardObjects()
        {
            for (var x = 0; x < _gridCells.GetLength(0); x++)
            {
                for (var y = 0; y < _gridCells.GetLength(1); y++)
                {
                    var gridCell = _gridCells[x, y];
                    _boardObjectFactory.SpawnBoardObject(gridCell, Vector3.zero);
                }
            }
        }
        
        public void ResetBoard()
        {
            foreach (var gridCell in _gridCells)
            {
                gridCell.GetChildObject()?.OnKill();
                gridCell.ClearChildObject();
            }
            
            InitialiseBoardObjects();
        }
        
        public bool TryGetBoardObjectByCellId(Vector2Int cellID, out BoardObject boardObject)
        {
            if (!IsValidGridPosition(cellID))
            {
                boardObject = null;
                return false;
            }
            
            boardObject = _gridCells[cellID.x,cellID.y].GetChildObject();
            
            return boardObject != null;
        }

        public async Task<bool> TrySwap(GridCell from, GridCell to)
        {
            if (Vector2Int.Distance(from.ID, to.ID) > 1) return false;
            
            await SwapCellObjects(from, to);

            //If move doesn't result in a swap, undo move
            if (!_matcher.HasMatches(_gridCells, out _))
            {
                await SwapCellObjects(to, from);
                return false;
            }

            await Task.Delay(100);
            return true;
        }

        private async Task SwapCellObjects(GridCell from, GridCell to)
        {
            var fromObject = from.GetChildObject();
            from.ClearChildObject();
            var toObject = to.GetChildObject();
            to.ClearChildObject();
            
            from.SetChildObject(toObject);
            to.SetChildObject(fromObject);
            await _animationManager.AnimateMove(new List<BoardObject> { from.GetChildObject(), to.GetChildObject()});
        }

        public async Task UpdateBoardState()
        {
            if(await HasGaps()) await UpdateBoardState();
            if (_matcher.HasMatches(_gridCells ,out var matches))
            {
                _scoreManager.AddScore(25*matches.Count);
                foreach (var gridCell in matches) 
                {
                    gridCell.GetChildObject()?.OnKill();
                    gridCell.ClearChildObject();
                }
                
                await Task.Delay(100);
                await UpdateBoardState();
            }
        }

        private async Task<bool> HasGaps()
        {
            var cellsToAnimate = new List<BoardObject>();
            
            for (var columnIndex = 0; columnIndex < _gridCells.GetLength(0); columnIndex++)
            {
                var columnCells = GetColumn(columnIndex);

                for (var y = columnCells.Count - 1; y >= 0; y--)
                {
                    var currentCell = columnCells[y];
                    if (y == 0 && currentCell.GetChildObject() == null)
                    {
                        cellsToAnimate.Add(_boardObjectFactory.SpawnBoardObject(currentCell, Vector3.up));
                    }

                    if (currentCell.GetChildObject() == null)
                    {
                        var cellAbove = columnCells[y - 1];
                        if(cellAbove.GetChildObject() == null) continue;

                        var childObject = cellAbove.GetChildObject();
                        
                        currentCell.SetChildObject(childObject);
                        cellsToAnimate.Add(childObject);
                    }
                }
            }
            if (cellsToAnimate.Count > 0) await _animationManager.AnimateMove(cellsToAnimate);
            return cellsToAnimate.Count > 0;
        }

        public List<GridCell> GetColumn(int index)
        {
            var candidateCells = new List<GridCell>();

            if (!IsValidGridPosition(new Vector2Int(index, 0))) return candidateCells;

            for (var i = 0; i < _gridCells.GetLength(1); i++)
            {
                candidateCells.Add(_gridCells[index, i]);
            }

            return candidateCells;
        }
        
        public List<GridCell> GetRow(int index)
        {
            var candidateCells = new List<GridCell>();

            if (!IsValidGridPosition(new Vector2Int(0, index))) return candidateCells;

            for (var i = 0; i < _gridCells.GetLength(0); i++)
            {
                candidateCells.Add(_gridCells[i, index]);
            }

            return candidateCells;
        }

        private bool IsValidGridPosition(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < _gridCells.GetLength(0) && pos.y >= 0 && pos.y < _gridCells.GetLength(1);
        }

        public void Dispose()
        {
           _gridCells = null;
        }
    }
}