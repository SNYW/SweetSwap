using System.Collections.Generic;
using Board;
using Settings;
using UnityEngine;

namespace Managers
{
    public class GridManager : IManager
    {
        private GridPosition[,] _gridPositionMap;
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
            _gridPositionMap ??= new GridPosition[gridDimensions.x,gridDimensions.y];
            
            float offsetX = (gridDimensions.x - 1) / 2f;
            float offsetY = (gridDimensions.y - 1) / 2f;

            for (int x = 0; x < gridDimensions.x; x++)
            {
                for (int y = 0; y < gridDimensions.y; y++)
                {
                    float worldX = x - offsetX;
                    float worldY = y - offsetY;
                    Vector2Int cellId = new Vector2Int(x, y);
                    Vector3 worldPos = new Vector3(worldX, worldY, 0);
                    var newGridPosition = new GridPosition(cellId, worldPos);
                    _gridPositionMap[x,y] = newGridPosition;
                }
            }
        }
        
        private void InitialiseBoardObjects()
        {
            var basePrefab = _gameSettings.boardObjectSettings.baseObjectPrefab;
            var boardObjectSettings = _gameSettings.boardObjectSettings;
            var parent = GameObject.FindGameObjectWithTag("Board Object Parent");

            for (int x = 0; x < _gridPositionMap.Length; x++)
            {
                for (int y = 0; y < _gridPositionMap.Length; y++)
                {
                    var gridPosition = _gridPositionMap[x, y];
                    var newPrefab = Object.Instantiate(basePrefab, gridPosition.WorldPosition, Quaternion.identity, parent.transform);
                    var excludedDefinitions = GetExcludedDefinitions(gridPosition);
                    List<BoardObjectDefinition> allowedDefinitions = new List<BoardObjectDefinition>();
                
                    foreach (var def in boardObjectSettings.activeBoardObjects)
                    {
                        if (!excludedDefinitions.Contains(def))
                        {
                            allowedDefinitions.Add(def);
                        }
                    }

                    int randomIndex = Random.Range(0, allowedDefinitions.Count);
                    newPrefab.Init(allowedDefinitions[randomIndex]);

                    gridPosition.heldObject = newPrefab;
                }
            }
        }

        private List<BoardObjectDefinition> GetExcludedDefinitions(GridPosition position)
        {
            var returnList = new List<BoardObjectDefinition>();
            if(TryGetBoardObjectByCellId(position.CellId + Vector2Int.up, out var upObject)) returnList.Add(upObject.definition);
            if(TryGetBoardObjectByCellId(position.CellId + Vector2Int.down, out var downObject)) returnList.Add(downObject.definition);
            if(TryGetBoardObjectByCellId(position.CellId + Vector2Int.left, out var leftObject)) returnList.Add(leftObject.definition);
            if(TryGetBoardObjectByCellId(position.CellId + Vector2Int.right, out var rightObject)) returnList.Add(rightObject.definition);
            return returnList;
        }

        public bool TryGetBoardObjectByCellId(Vector2Int cellID, out BoardObject boardObject)
        {
            boardObject = _gridPositionMap[cellID.x,cellID.y].heldObject;
            
            return boardObject != null;
        }

        public bool RowHasMatches(int rowIndex, out List<GridPosition> matchedGridPositions)
        {
            matchedGridPositions = new List<GridPosition>();
            var candidateCells = new List<GridPosition>();

            for (int i = 0; i < _gridPositionMap.GetLength(0); i++)
            {
                candidateCells.Add(_gridPositionMap[i,rowIndex]);
            }
            
            var currentMatchList = new List<GridPosition>();
            
            for (int i = 0; i < candidateCells.Count-1; i++)
            {
                var currentCell = candidateCells[i];
                var nextCell = candidateCells[i+1];
                
                if (currentCell.heldObject.definition == nextCell.heldObject.definition)
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
            return pos.x < _gridPositionMap.GetLength(0) && pos.y < _gridPositionMap.GetLength(1);
        }

        public void Dispose()
        {
           _gridPositionMap = null;
        }
    }
}