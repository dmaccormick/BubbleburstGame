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
        public const string ANIM_ON_HOVERED = "OnHovered";
        public const string ANIM_ON_UNHOVERED = "OnUnhovered";
        public const string ANIM_ON_POP_FAILED = "OnPopFailed";

        public const float VFX_SIZE_MAX_STEP_INCREASES = 5;
        public const float VFX_SIZE_INCREASE_PER_DEPTH = 0.5f;

        public BubbleColour Colour => m_BubbleColour;

        [SerializeField] private Animator m_Animator;
        [SerializeField] private BubbleColour m_BubbleColour;
        [SerializeField] private Transform m_Visuals;
        [SerializeField] private GameObject m_DestroyVFX;

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
        public Dictionary<Bubble, int> FindBubbleGroup()
        {
            Queue<GridCell> cellsToProcess = new();
            cellsToProcess.Enqueue(m_GridCell);

            Dictionary<GridCell, int> visitedCellDepths = new Dictionary<GridCell, int>();
            visitedCellDepths.Add(m_GridCell, 0);

            Dictionary<Bubble, int> matchedBubblesWithDepth = new();
            matchedBubblesWithDepth.Add(this, 0);

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
                            matchedBubblesWithDepth.Add(neighbourCell.Bubble, currentDepth + 1);
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
            m_Animator.SetTrigger(ANIM_ON_HOVERED);
        }

        public void OnUnhovered()
        {
            m_Animator.ResetTrigger(ANIM_ON_HOVERED);
            m_Animator.SetTrigger(ANIM_ON_UNHOVERED);
        }

        public void OnPopped(int depth)
        {
            m_GridCell.RemoveBubble();

            // NOTE: Pooling these VFX would be more efficient, especially for mobile
            int destroyVFXSize = (int)Mathf.Min(VFX_SIZE_MAX_STEP_INCREASES, depth);
            GameObject destroyedVFX = Instantiate(m_DestroyVFX, transform.position, Quaternion.identity, null);
            destroyedVFX.transform.localScale = Vector3.one * (1.0f + (depth * VFX_SIZE_INCREASE_PER_DEPTH));

            Destroy(this.gameObject);
        }

        public void OnPopFailed()
        {
            m_Animator.SetTrigger(ANIM_ON_POP_FAILED);
        }
    }
}