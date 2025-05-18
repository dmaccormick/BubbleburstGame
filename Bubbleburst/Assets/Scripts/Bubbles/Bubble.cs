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
    }
}