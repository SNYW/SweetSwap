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
        private readonly GameObject _boardObjectParent = GameObject.FindGameObjectWithTag("Board Object Parent");

        public BoardObject SpawnBoardObject(GridCell gridCell, Vector3 positionOffset)
        {
            var basePrefab = _gameSettings.boardObjectSettings.baseObjectPrefab;
            var boardObjectSettings = _gameSettings.boardObjectSettings;
            var allowedDefinitions = new List<BoardObjectDefinition>();
            var newPrefab = Object.Instantiate(basePrefab, gridCell.WorldPosition + positionOffset, Quaternion.identity, _boardObjectParent.transform);
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