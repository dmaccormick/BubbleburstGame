using DanMacC.BubbleBurst.Bubbles;
using DanMacC.BubbleBurst.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DanMacC.BubbleBurst.Interactions
{
    public class InteractionManager : MonoBehaviour
    {
        public const float DELAY_PER_BUBBLE_DISTANCE = 0.1f;

        /// <summary>
        /// The int represents the 'depth' of the bubble relative to the first one (the first one is 0)
        /// For every jump to a set of neighbours, the depth increases by 1
        /// This way, the bubbles that are further away can be popped later to make the animation more satisfying
        /// </summary>
        private Dictionary<Bubble, int> CurrentBubbleGroup = new();
        private Bubble CurrentMouseTarget;
        private Vector3 m_LastMousePosition;
        private bool m_IsAnimating = false;

        private void FixedUpdate()
        {
            // Only perform the raycast if the mouse has actually moved
            // No need to waste the calculation for the raycast otherwise
            // Also, don't do it if waiting for one of the animations to complete
            if (m_IsAnimating) return;
            if (Input.mousePosition == m_LastMousePosition) return;
            m_LastMousePosition = Input.mousePosition;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo))
            {
                if (hitInfo.collider.TryGetComponent<Bubble>(out var newBubble))
                {
                    // Also early-out if the mouse has moved but it is still on top of the same bubble
                    // Don't want to re-do the search process if not necessary
                    if (newBubble == CurrentMouseTarget) return;

                    HoverBubbleGroup(newBubble);
                }
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (CurrentBubbleGroup == null || CurrentBubbleGroup.Count == 0) return;

                if (CurrentBubbleGroup.Count > 1)
                {
                    PopBubbleGroup();
                }
                else
                {
                    CurrentMouseTarget.OnPopFailed();
                }
            }
        }

        public void UnhoverBubbleGroup()
        {
            foreach(var bubbleWithDepth in CurrentBubbleGroup)
            {
                var bubble = bubbleWithDepth.Key;
                bubble.OnUnhovered();
            }
            CurrentBubbleGroup.Clear();
        }

        public void HoverBubbleGroup(Bubble newTargetBubble)
        {
            var newBubbleGroup = newTargetBubble.FindBubbleGroup();

            // If the new bubble group includes the previous target bubble, there is no need to play the unhover sequence
            // This is because the same group is being selected, just with different depths
            if (CurrentMouseTarget != null && !newBubbleGroup.ContainsKey(CurrentMouseTarget))
            {
                UnhoverBubbleGroup();
            }

            CurrentMouseTarget = newTargetBubble;
            CurrentBubbleGroup = newBubbleGroup;

            foreach (var bubbleWithDepth in CurrentBubbleGroup)
            {
                var bubble = bubbleWithDepth.Key;
                bubble.OnHovered();
            }
        }

        public void PopBubbleGroup()
        {
            StartCoroutine(PopBubblesInSequence());
            IEnumerator PopBubblesInSequence()
            {
                m_IsAnimating = true;

                int currentDepth = 0;

                foreach (var bubbleWithDepth in CurrentBubbleGroup)
                {
                    var bubble = bubbleWithDepth.Key;
                    var depth = bubbleWithDepth.Value;

                    if (depth > currentDepth)
                    {
                        yield return new WaitForSeconds(DELAY_PER_BUBBLE_DISTANCE);
                        currentDepth++;
                    }

                    bubble.OnSelected();
                }

                m_IsAnimating = false;
                m_LastMousePosition = Vector2.zero; // Reset the mouse position to allow for raycasting since this selection is gone

                GameManager.Instance.RecordBubbleGroupPopped(CurrentBubbleGroup);
                CurrentBubbleGroup.Clear();
            }
        }
    }
}