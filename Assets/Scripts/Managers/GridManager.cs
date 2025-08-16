using System.Collections.Generic;
using Board;
using Settings;
using UnityEngine;

namespace Managers
{
    public class GridManager : IManager
    {
        private GridCell[,] _gridCells;
        private GameSettings _gameSettings;
        public void Init()
        {
            _gameSettings = Injection.GetManager<SettingsManager>().ActiveSettings;
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
            _gridCells ??= new GridCell[gridDimensions.x,gridDimensions.y];
            
            float offsetX = (gridDimensions.x - 1) / 2f;
            float offsetY = (gridDimensions.y - 1) / 2f;

            for (int x = 0; x < gridDimensions.x; x++)
            {
                for (int y = gridDimensions.y - 1; y >= 0; y--)
                {
                    float worldX = x - offsetX;
                    float worldY = y - offsetY;
                    Vector2Int cellId = new Vector2Int(x, y);
                    Vector3 worldPos = new Vector3(worldX, worldY, 0);
                    var newGridCell = new GridCell(cellId, worldPos);
                    _gridCells[x,y] = newGridCell;
                }
            }
        }
        
        private void InitialiseBoardObjects()
        {
            var basePrefab = _gameSettings.boardObjectSettings.baseObjectPrefab;
            var boardObjectSettings = _gameSettings.boardObjectSettings;
            var parent = GameObject.FindGameObjectWithTag("Board Object Parent");
            var allowedDefinitions = new List<BoardObjectDefinition>();

            for (int x = 0; x < _gridCells.GetLength(0); x++)
            {
                for (int y = 0; y < _gridCells.GetLength(1); y++)
                {
                    var gridCell = _gridCells[x, y];
                    var newPrefab = Object.Instantiate(basePrefab, gridCell.WorldPosition, Quaternion.identity, parent.transform);
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
                }
            }
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

        public bool TrySwap(GridCell from, GridCell to)
        {
            if (Vector2Int.Distance(from.ID, to.ID) > 1) return false;

            var fromObject = from.GetChildObject();
            var toObject = to.GetChildObject();
            
            from.SetChildObject(toObject);
            to.SetChildObject(fromObject);
            
            return true;
        }

        public bool RowHasMatches(int rowIndex, out List<GridCell> matchedGridPositions)
        {
            matchedGridPositions = new List<GridCell>();
            var candidateCells = new List<GridCell>();

            for (int i = 0; i < _gridCells.GetLength(0); i++)
            {
                candidateCells.Add(_gridCells[i,rowIndex]);
            }
            
            var currentMatchList = new List<GridCell>();
            
            for (int i = 0; i < candidateCells.Count-1; i++)
            {
                var currentCell = candidateCells[i];
                var nextCell = candidateCells[i+1];
                
                if (currentCell.GetChildObject().definition == nextCell.GetChildObject().definition)
                {
                    currentMatchList.Add(currentCell);
                }
                else
                {
                    if (currentMatchList.Count >= 3)
                    {
                        matchedGridPositions.AddRange(currentMatchList);
                    }
                    currentMatchList.Clear();
                }
            }
            
            return matchedGridPositions.Count > 0;
        }
        
        public bool ColumnHasMatches(int columnIndex, out List<GridCell> matchedGridPositions)
        {
            matchedGridPositions = new List<GridCell>();
            var candidateCells = new List<GridCell>();

            for (int i = 0; i < _gridCells.GetLength(1); i++)
            {
                candidateCells.Add(_gridCells[columnIndex, i]);
            }
            
            var currentMatchList = new List<GridCell>();
            
            for (int i = 0; i < candidateCells.Count-1; i++)
            {
                var currentCell = candidateCells[i];
                var nextCell = candidateCells[i+1];
                
                if (currentCell.GetChildObject().definition == nextCell.GetChildObject().definition)
                {
                    currentMatchList.Add(currentCell);
                }
                else
                {
                    if (currentMatchList.Count >= 3)
                    {
                        matchedGridPositions.AddRange(currentMatchList);
                    }
                    currentMatchList.Clear();
                }
            }
            
            return matchedGridPositions.Count > 0;
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