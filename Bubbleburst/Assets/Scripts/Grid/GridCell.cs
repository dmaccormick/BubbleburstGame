using UnityEngine;
using System.Text;
using System.Collections.Generic;
using DanMacC.BubbleBurst.Bubbles;
using System.Linq;

namespace DanMacC.BubbleBurst.Grid
{
    public class GridCell : MonoBehaviour
    {
        public bool IsEmpty => m_Bubble == null;
        public bool IsFilled => !IsEmpty;
        public Vector3 WorldPosition => transform.position;
        public Vector2Int GridCoord => m_GridCoord;
        public Dictionary<Vector2Int, GridCell>.ValueCollection NeighbourCells => m_Neighbours.Values;
        public Bubble Bubble => m_Bubble;
        public Vector3 BubbleAnchorPosition => m_BubbleAnchor.position;

        [SerializeField] private Transform m_BubbleAnchor;

        private GridManager m_Manager;
        private Vector2Int m_GridCoord = new Vector2Int(0, 0);
        private Bubble m_Bubble;

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

            GenerateBubble();
        }

        public void SetNeighbour(Vector2Int direction, GridCell cell)
        {
            m_Neighbours[direction] = cell;
        }

        public void GenerateBubble()
        {
            int bubbleColourIndex = Random.Range(0, (int)BubbleColour.Count);
            Bubble bubblePrefab = m_Manager.GetBubblePrefab((BubbleColour)bubbleColourIndex);

            m_Bubble = Instantiate(bubblePrefab);
            AttachBubble(m_Bubble, true);
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

        public Bubble RemoveBubble()
        {
            Bubble previousBubble = m_Bubble;
            m_Bubble = null;

            return previousBubble;
        }

        public void AttachBubble(Bubble bubble, bool snapToAnchor)
        {
            m_Bubble = bubble;
            m_Bubble.transform.SetParent(m_BubbleAnchor, !snapToAnchor);

            m_Bubble.Initialize(this);
        }
    }
}