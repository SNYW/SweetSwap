using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace Managers
{
    public class GridManager : IManager
    {
        private Vector2[,] _gridPositions;
        public void Init()
        {
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            InitialiseGridPositions();
        }
        
        private void InitialiseGridPositions()
        {
            var settings = Injection.GetManager<SettingsManager>().ActiveSettings;
            var gridDimensions = settings.baseGridDimensions;

            _gridPositions = new Vector2[gridDimensions.x, gridDimensions.y];

            // Calculate offset so grid is centered at (0,0)
            float offsetX = (gridDimensions.x - 1) / 2f;
            float offsetY = (gridDimensions.y - 1) / 2f;

            for (int x = 0; x < gridDimensions.x; x++)
            {
                for (int y = 0; y < gridDimensions.y; y++)
                {
                    float worldX = x - offsetX;
                    float worldY = y - offsetY;
                    _gridPositions[x, y] = new Vector2(worldX, worldY);
                }
            }
        }
        
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            for (int x = 0; x < _gridPositions.GetLength(0); x++)
            {
                for (int y = 0; y < _gridPositions.GetLength(1); y++)
                {
                    var pos = _gridPositions[x, y];
                    Gizmos.DrawSphere(new Vector3(pos.X, pos.Y, 0), 0.1f);
                }
            }
        }

        public void Dispose()
        {
            _gridPositions = null;
        }
    }
}