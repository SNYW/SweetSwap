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
            b.ParentCell?.ClearChildObject();
            _heldObject = b;
            _heldObject.ParentCell = this;
        }

        public void ClearChildObject()
        {
            if (_heldObject.ParentCell == this) _heldObject.ParentCell = null;
            _heldObject = null;
        }

        public BoardObject GetChildObject()
        {
            return _heldObject;
        }

        public void DestroyChild()
        {
            if(_heldObject == null) return;
            _heldObject.ParentCell = null;
            Object.Destroy(_heldObject.gameObject);
            _heldObject = null;
        }
    }
}