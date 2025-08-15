using UnityEngine;

namespace Board
{
    public class GridPosition
    {
        public Vector2 CellId { get; }
        public Vector3 WorldPosition { get; }

        public GridPosition(Vector2 cellId, Vector3 worldPosition)
        {
            CellId = cellId;
            WorldPosition = worldPosition;
        }
    }
}