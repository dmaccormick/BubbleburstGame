using DanMacC.BubbleBurst.Grid;
using DanMacC.BubbleBurst.Interactions;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DanMacC.BubbleBurst.Bubbles
{
    public enum BubbleColour
    {
        Red,
        Green,
        Blue,
        Yellow,

        Count,
        None
    }

    public class Bubble : MonoBehaviour
    {
        public BubbleColour Colour => m_BubbleColour;

        [SerializeField] private BubbleColour m_BubbleColour;
        [SerializeField] private Transform m_Visuals;

        private GridCell m_GridCell; 

        public void Initialize(GridCell gridCell)
        {
            m_GridCell = gridCell;
        }

        /// <summary>
        /// Performs a breadth-first search to get all of the bubbles that are connected to this one with the same colour. Also stores their depths
        /// The depths in this case represent how far they are from the clicked bubble. This is used for the popping anim sequence
        /// The goal of the animation sequence is to have the bubbles pop closest to furthest from the click
        /// It should be a fun way of showing the combo'ing effect
        /// </summary>
        public List<(Bubble, int)> FindBubbleGroup()
        {
            Queue<GridCell> cellsToProcess = new();
            cellsToProcess.Enqueue(m_GridCell);

            Dictionary<GridCell, int> visitedCellDepths = new Dictionary<GridCell, int>();
            visitedCellDepths.Add(m_GridCell, 0);

            List<(Bubble, int)> matchedBubblesWithDepth = new();
            matchedBubblesWithDepth.Add((this, 0));

            while (cellsToProcess.Count > 0)
            {
                GridCell cell = cellsToProcess.Dequeue();
                int currentDepth = visitedCellDepths[cell];

                foreach (var neighbourCell in cell.NeighbourCells)
                {
                    if (neighbourCell == null) continue;

                    if (!visitedCellDepths.ContainsKey(neighbourCell))
                    {
                        visitedCellDepths.Add(neighbourCell, currentDepth + 1);

                        if (!neighbourCell.IsEmpty && neighbourCell.Bubble.Colour == m_BubbleColour)
                        {
                            cellsToProcess.Enqueue(neighbourCell);
                            matchedBubblesWithDepth.Add((neighbourCell.Bubble, currentDepth + 1));
                        }
                    }
                }
            }

            return matchedBubblesWithDepth;
        }

        public override string ToString()
        {
            return $"{m_GridCell.GridCoord} - {m_BubbleColour}";
        }

        public void OnHovered()
        {
            transform.localScale = Vector3.one * 1.5f;
        }

        public void OnUnhovered()
        {
            transform.localScale = Vector3.one * 1.0f;
        }

        public void OnSelected()
        {
            m_GridCell.RemoveBubble();
            Destroy(this.gameObject);
        }

        public void OnSelectionFailed()
        {
            Debug.Log("Cannot delete a single bubble!");
        }
    }
}