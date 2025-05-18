using System.Collections;
using UnityEngine;

namespace DanMacC.BubbleBurst.Interactions
{
    /// <summary>
    /// Interface that will be detected by the InteractionManager
    /// Requires the object to have a collider since the InteractionManager uses raycasts
    /// In this game, it's only the bubbles but in a larger game, this interface would have more value
    /// </summary>
    public interface IInteractable
    {
        public void OnTargetingStart();
        public void OnTargetingEnd();

        public void OnClicked();
    }
}