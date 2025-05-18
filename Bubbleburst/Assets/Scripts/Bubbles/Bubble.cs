using DanMacC.BubbleBurst.Interactions;
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

    public class Bubble : MonoBehaviour, IInteractable
    {
        public BubbleColour Colour => m_BubbleColour;

        [SerializeField] private BubbleColour m_BubbleColour;
        [SerializeField] private Transform m_Visuals;

        #region IInteractable
        public void OnTargetingStart()
        {
            m_Visuals.localScale = Vector3.one * 1.5f;
        }

        public void OnTargetingEnd()
        {
            m_Visuals.localScale = Vector3.one;
        }

        public void OnClicked()
        {
            m_Visuals.localScale = Vector3.one * 2.0f;
        }
        #endregion IInteractable
    }
}