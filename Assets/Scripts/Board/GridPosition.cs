using UnityEngine;

namespace Board
{
    public class GridPosition
    {
        public Vector2Int CellId { get; }
        public Vector3 WorldPosition { get; }

        public BoardObject heldObject;

        public GridPosition(Vector2Int cellId, Vector3 worldPosition)
        {
            CellId = cellId;
            WorldPosition = worldPosition;
            heldObject = null;
        }
    }
}