using System.Collections.Generic;
using System.Linq;
using Board;
using Settings;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace Managers
{
    public class GridManager : IManager
    {
        private Dictionary<Vector2, BoardObject> _boardState;
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
            _boardState ??= new Dictionary<Vector2, BoardObject>();
            var gridDimensions = _gameSettings.baseGridDimensions;

            float offsetX = (gridDimensions.x - 1) / 2f;
            float offsetY = (gridDimensions.y - 1) / 2f;

            for (int x = 0; x < gridDimensions.x; x++)
            {
                for (int y = 0; y < gridDimensions.y; y++)
                {
                    float worldX = x - offsetX;
                    float worldY = y - offsetY;
                    _boardState[new Vector2(worldX, worldY)] = null;
                }
            }
        }
        
        private void InitialiseBoardObjects()
        {
            var basePrefab = _gameSettings.boardObjectSettings.baseObjectPrefab;
            var parent = GameObject.FindGameObjectWithTag("Board Object Parent");

            foreach (var pos in _boardState.Keys)
            {
                var newPrefab = Object.Instantiate(basePrefab, new Vector3(pos.X, pos.Y, 0), Quaternion.identity, parent.transform);
                newPrefab.Init(_gameSettings.boardObjectSettings.activeBoardObjects[0]);
            }
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