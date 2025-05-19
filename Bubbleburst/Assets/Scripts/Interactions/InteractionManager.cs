using DanMacC.BubbleBurst.Bubbles;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DanMacC.BubbleBurst.Interactions
{
    public class InteractionManager : MonoBehaviour
    {
        /// <summary>
        /// The int represents the 'depth' of the bubble relative to the first one (the first one is 0)
        /// For every jump to a set of neighbours, the depth increases by 1
        /// This way, the bubbles that are further away can be popped later to make the animation more satisfying
        /// </summary>
        private List<(Bubble, int)> CurrentBubbleGroup = new();
        private Bubble CurrentMouseTarget;
        private Vector3 m_LastMousePosition;

        private void FixedUpdate()
        {
            // Only perform the raycast if the mouse has actually moved
            // No need to waste the calculation for the raycast otherwise
            if (Input.mousePosition == m_LastMousePosition) return;
            m_LastMousePosition = Input.mousePosition;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo))
            {
                if (hitInfo.collider.TryGetComponent<Bubble>(out var newBubble))
                {
                    // Also early-out if the mouse has moved but it is still on top of the same bubble
                    // Don't want to re-do the search process if not necessary
                    if (newBubble == CurrentMouseTarget) return;
                    CurrentMouseTarget = newBubble;

                    HoverBubbleGroup(CurrentMouseTarget);
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
                    foreach (var bubbleWithDepth in CurrentBubbleGroup)
                    {
                        var bubble = bubbleWithDepth.Item1;
                        bubble.OnSelected();
                    }
                    CurrentBubbleGroup.Clear();
                }
                else
                {
                    CurrentMouseTarget.OnSelectionFailed();
                }
            }
        }

        public void UnhoverBubbleGroup()
        {
            foreach(var bubbleWithDepth in CurrentBubbleGroup)
            {
                var bubble = bubbleWithDepth.Item1;
                bubble.OnUnhovered();
            }
            CurrentBubbleGroup.Clear();
        }

        public void HoverBubbleGroup(Bubble startingBubble)
        {
            UnhoverBubbleGroup();

            CurrentBubbleGroup = startingBubble.FindBubbleGroup();
            foreach (var bubbleWithDepth in CurrentBubbleGroup)
            {
                var bubble = bubbleWithDepth.Item1;
                bubble.OnHovered();
            }

            // TODO: Remove this
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in CurrentBubbleGroup)
            {
                stringBuilder.AppendLine($"{item.Item1.ToString()}, {item.Item2}");
            }
            Debug.Log(stringBuilder.ToString());
        }
    }
}