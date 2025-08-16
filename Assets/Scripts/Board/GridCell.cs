using UnityEngine;

namespace Board
{
    public class GridCell
    {
        public Vector2Int ID { get; }
        public Vector3 WorldPosition { get; }
        private BoardObject _heldObject;

        public GridCell(Vector2Int id, Vector3 worldPosition)
        {
            ID = id;
            WorldPosition = worldPosition;
            _heldObject = null;
        }

        public void SetChildObject(BoardObject b)
        {
            _heldObject = b;
            _heldObject.parentCell = this;
            _heldObject.transform.position = WorldPosition;
        }

        public BoardObject GetChildObject()
        {
            return _heldObject;
        }
    }
}