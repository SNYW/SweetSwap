using System.Collections.Generic;
using Managers;
using Settings;
using UnityEngine;

namespace Board
{
    public class BoardObjectFactory
    {
        private readonly GameSettings _gameSettings = Injection.GetManager<SettingsManager>().ActiveSettings;
        private readonly GridManager _gridManager = Injection.GetManager<GridManager>();
        private readonly ObjectPoolManager _objectPoolManager = Injection.GetManager<ObjectPoolManager>();
        private readonly Transform _boardObjectParent = GameObject.FindGameObjectWithTag("Board Object Parent").transform;

        public BoardObject SpawnBoardObject(GridCell gridCell, Vector3 positionOffset)
        {
            var boardObjectSettings = _gameSettings.boardObjectSettings;
            var allowedDefinitions = new List<BoardObjectDefinition>();
            var newPrefab = _objectPoolManager.GetObject<BoardObject>(ObjectPoolType.BoardObject);
            newPrefab.transform.position = gridCell.WorldPosition + positionOffset;
            newPrefab.transform.parent = _boardObjectParent;
            
            var excludedDefinitions = GetExcludedDefinitions(gridCell);
            foreach (var def in boardObjectSettings.activeBoardObjects)
            {
                if (!excludedDefinitions.Contains(def))
                {
                    allowedDefinitions.Add(def);
                }
            }

            var randomIndex = Random.Range(0, allowedDefinitions.Count);
            newPrefab.Init(allowedDefinitions[randomIndex]);
            gridCell.SetChildObject(newPrefab);
            allowedDefinitions.Clear();
            return newPrefab;
        }
        
        private List<BoardObjectDefinition> GetExcludedDefinitions(GridCell cell)
        {
            var returnList = new List<BoardObjectDefinition>();
            if(_gridManager.TryGetBoardObjectByCellId(cell.ID + Vector2Int.up, out var upObject)) returnList.Add(upObject.definition);
            if(_gridManager.TryGetBoardObjectByCellId(cell.ID + Vector2Int.down, out var downObject)) returnList.Add(downObject.definition);
            if(_gridManager.TryGetBoardObjectByCellId(cell.ID + Vector2Int.left, out var leftObject)) returnList.Add(leftObject.definition);
            if(_gridManager.TryGetBoardObjectByCellId(cell.ID + Vector2Int.right, out var rightObject)) returnList.Add(rightObject.definition);
            return returnList;
        }
    }
}