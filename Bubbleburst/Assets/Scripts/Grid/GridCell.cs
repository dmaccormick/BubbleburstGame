using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DanMacC.BubbleBurst.Grid
{
    public class GridCell : MonoBehaviour
    {
        public bool IsEmpty => m_CurrentObject == null;
        public bool IsFilled => !IsEmpty;
        public Vector3 WorldPosition => transform.position;
        public Vector2Int GridCoord => m_GridCoord;

        private GridManager m_Manager;
        private Vector2Int m_GridCoord = new Vector2Int(0, 0);
        private GridObject m_CurrentObject;

        private Dictionary<Vector2Int, GridCell> m_Neighbours = new()
        {
            { Vector2Int.up, null },
            { Vector2Int.right, null },
            { Vector2Int.down, null },
            { Vector2Int.left, null },
        };

        public void Initialize(GridManager manager, int x, int y) => Initialize(manager, new Vector2Int(x, y));
        public void Initialize(GridManager manager, Vector2Int coord)
        {
            m_Manager = manager;
            m_GridCoord = coord;

            gameObject.name = $"Cell [{m_GridCoord.x}][{m_GridCoord.y}]";
        }

        public void SetNeighbour(Vector2Int direction, GridCell cell)
        {
            m_Neighbours[direction] = cell;
        }

        [ContextMenu("OutputDebugInfo()")]
        public void OutputDebugInfo()
        {
            StringBuilder debugString = new StringBuilder();

            debugString.AppendLine(gameObject.name);
            foreach(var kvp in m_Neighbours)
            {
                string neighbourName = (kvp.Value != null) ? kvp.Value.name : "null";
                debugString.AppendLine($"{kvp.Key} -> {neighbourName}");
            }
            
            Debug.Log(debugString.ToString());
        }
    }
}