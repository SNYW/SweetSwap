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
        public void Init()
        {
            _gameSettings = Injection.GetManager<SettingsManager>().ActiveSettings;
            _animationManager = Injection.GetManager<AnimationManager>();
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
    
            float offsetX = (gridDimensions.x - 1) / 2f;
            float offsetY = (gridDimensions.y - 1) / 2f;

            for (int y = 0; y < gridDimensions.y; y++)
            {
                for (int x = 0; x < gridDimensions.x; x++)
                {
                    float worldX = x - offsetX;
                    float worldY = gridDimensions.y - 1 - y - offsetY; 

                    Vector2Int cellId = new Vector2Int(x, y);
                    Vector3 worldPos = new Vector3(worldX, worldY, 0);

                    var newGridCell = new GridCell(cellId, worldPos);
                    _gridCells[x, y] = newGridCell;
                }
            }
        }
        
        private void InitialiseBoardObjects()
        {
            for (int x = 0; x < _gridCells.GetLength(0); x++)
            {
                for (int y = 0; y < _gridCells.GetLength(1); y++)
                {
                    var gridCell = _gridCells[x, y];
                    SpawnBoardObject(gridCell, Vector3.zero);
                }
            }
        }

        private BoardObject SpawnBoardObject(GridCell gridCell, Vector3 positionOffset)
        {
            var basePrefab = _gameSettings.boardObjectSettings.baseObjectPrefab;
            var boardObjectSettings = _gameSettings.boardObjectSettings;
            var parent = GameObject.FindGameObjectWithTag("Board Object Parent");
            var allowedDefinitions = new List<BoardObjectDefinition>();
            var newPrefab = Object.Instantiate(basePrefab, gridCell.WorldPosition + positionOffset, Quaternion.identity, parent.transform);
            var excludedDefinitions = GetExcludedDefinitions(gridCell);
                
            foreach (var def in boardObjectSettings.activeBoardObjects)
            {
                if (!excludedDefinitions.Contains(def))
                {
                    allowedDefinitions.Add(def);
                }
            }

            int randomIndex = Random.Range(0, allowedDefinitions.Count);
            newPrefab.Init(allowedDefinitions[randomIndex]);
            gridCell.SetChildObject(newPrefab);
            allowedDefinitions.Clear();
            return newPrefab;
        }

        private List<BoardObjectDefinition> GetExcludedDefinitions(GridCell cell)
        {
            var returnList = new List<BoardObjectDefinition>();
            if(TryGetBoardObjectByCellId(cell.ID + Vector2Int.up, out var upObject)) returnList.Add(upObject.definition);
            if(TryGetBoardObjectByCellId(cell.ID + Vector2Int.down, out var downObject)) returnList.Add(downObject.definition);
            if(TryGetBoardObjectByCellId(cell.ID + Vector2Int.left, out var leftObject)) returnList.Add(leftObject.definition);
            if(TryGetBoardObjectByCellId(cell.ID + Vector2Int.right, out var rightObject)) returnList.Add(rightObject.definition);
            return returnList;
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

            var fromObject = from.GetChildObject();
            from.ClearChildObject();
            var toObject = to.GetChildObject();
            to.ClearChildObject();
            
            from.SetChildObject(toObject);
            to.SetChildObject(fromObject);

            await _animationManager.AnimateMove(new List<BoardObject> { from.GetChildObject(), to.GetChildObject()});
            await Task.Delay(100);
            
            return true;
        }

        public async Task UpdateBoardState()
        {
            if(await HasGaps()) await UpdateBoardState();
            if(await HasMatches()) await UpdateBoardState();
        }

        private async Task<bool> HasGaps()
        {
            var cellsToAnimate = new List<BoardObject>();
            
            for (int columnIndex = 0; columnIndex < _gridCells.GetLength(0); columnIndex++)
            {
                var columnCells = GetColumn(columnIndex);

                for (int y = columnCells.Count - 1; y >= 0; y--)
                {
                    var currentCell = columnCells[y];
                    if (y == 0 && currentCell.GetChildObject() == null)
                    {
                        cellsToAnimate.Add(SpawnBoardObject(currentCell, Vector3.up));
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

        private async Task<bool> HasMatches()
        {
            var matches = new List<GridCell>();
            
            for (int y = 0; y < _gridCells.GetLength(1); y++)
            {
               matches.AddRange(GetMatches(GetRow(y)));
            }
            
            for (int x = 0; x < _gridCells.GetLength(0); x++)
            {
                matches.AddRange(GetMatches(GetColumn(x)));
            }

            foreach (var gridCell in matches)
            {
                gridCell.DestroyChild();
            }
            await Task.Delay(100);
            
            return matches.Count > 0;
        }

        public List<GridCell> GetMatches(List<GridCell> cells)
        {
            var matches = new List<GridCell>();
            if (cells.Count < 3) return matches;

            List<GridCell> streak = new List<GridCell> { cells[0] };

            for (int i = 1; i < cells.Count; i++)
            {
                if (cells[i].GetChildObject().definition == cells[i - 1].GetChildObject().definition)
                {
                    streak.Add(cells[i]);
                }
                else
                {
                    if (streak.Count >= 3) matches.AddRange(streak);

                    streak.Clear();
                    streak.Add(cells[i]);
                }
            }

            if (streak.Count >= 3) matches.AddRange(streak);

            return matches;
        }

        private List<GridCell> GetColumn(int index)
        {
            var candidateCells = new List<GridCell>();

            if (!IsValidGridPosition(new Vector2Int(index, 0))) return candidateCells;

            for (int i = 0; i < _gridCells.GetLength(1); i++)
            {
                candidateCells.Add(_gridCells[index, i]);
            }

            return candidateCells;
        }
        
        private List<GridCell> GetRow(int index)
        {
            var candidateCells = new List<GridCell>();

            if (!IsValidGridPosition(new Vector2Int(0, index))) return candidateCells;

            for (int i = 0; i < _gridCells.GetLength(0); i++)
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