using System.Collections.Generic;
using System.Linq;
using Board;
using Settings;
using UnityEngine;

namespace Managers
{
    public class GridManager : IManager
    {
        private Dictionary<GridPosition, BoardObject> _boardState;
        private Dictionary<Vector2, GridPosition> _gridPositionMap;
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
            _boardState ??= new Dictionary<GridPosition, BoardObject>();
            _gridPositionMap ??= new Dictionary<Vector2, GridPosition>();
            var gridDimensions = _gameSettings.baseGridDimensions;

            float offsetX = (gridDimensions.x - 1) / 2f;
            float offsetY = (gridDimensions.y - 1) / 2f;

            for (int x = 0; x < gridDimensions.x; x++)
            {
                for (int y = 0; y < gridDimensions.y; y++)
                {
                    float worldX = x - offsetX;
                    float worldY = y - offsetY;
                    Vector2 cellId = new Vector2(x, y);
                    Vector3 worldPos = new Vector3(worldX, worldY, 0);
                    var newGridPosition = new GridPosition(cellId, worldPos);
                    _boardState[newGridPosition] = null;
                    _gridPositionMap[cellId] = newGridPosition;
                }
            }
        }
        
        private void InitialiseBoardObjects()
        {
            var basePrefab = _gameSettings.boardObjectSettings.baseObjectPrefab;
            var boardObjectSettings = _gameSettings.boardObjectSettings;
            var parent = GameObject.FindGameObjectWithTag("Board Object Parent");

            foreach (var gridPosition in _gridPositionMap.Values)
            {
                var newPrefab = Object.Instantiate(basePrefab, gridPosition.WorldPosition, Quaternion.identity, parent.transform);
                var excludedDefinitions = GetExcludedDefinitions(gridPosition);
                var allowedDefinitions = boardObjectSettings.activeBoardObjects.Where(def => !excludedDefinitions.Contains(def)).ToList();
                newPrefab.Init(allowedDefinitions[Random.Range(0, allowedDefinitions.Count)]);
                _boardState[gridPosition] = newPrefab;
            }
        }

        private List<BoardObjectDefinition> GetExcludedDefinitions(GridPosition position)
        {
            var returnList = new List<BoardObjectDefinition>();
            if(TryGetBoardObjectByCellId(position.CellId + Vector2.up, out var upObject)) returnList.Add(upObject.definition);
            if(TryGetBoardObjectByCellId(position.CellId + Vector2.down, out var downObject)) returnList.Add(downObject.definition);
            if(TryGetBoardObjectByCellId(position.CellId + Vector2.left, out var leftObject)) returnList.Add(leftObject.definition);
            if(TryGetBoardObjectByCellId(position.CellId + Vector2.right, out var rightObject)) returnList.Add(rightObject.definition);
            return returnList;
        }

        public bool TryGetBoardObjectByCellId(Vector2 cellID, out BoardObject boardObject)
        {
            boardObject = null;
            return _gridPositionMap.TryGetValue(cellID, out var gridPosition) && _boardState.TryGetValue(gridPosition, out boardObject) && boardObject != null;
        }
        
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            foreach (var key in _boardState.Keys)
            {
                //Gizmos.DrawSphere(new Vector3(key.X, key.Y, 0), 0.1f);
            }
        }

        public void Dispose()
        {
           _boardState.Clear();
           _boardState = null;
        }
    }
}