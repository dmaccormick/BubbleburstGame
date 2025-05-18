using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DanMacC.BubbleBurst.Interactions
{
    public class InteractionManager : MonoBehaviour
    {
        private Vector3 m_LastMousePosition;
        private IInteractable m_CurrentTarget;

        private void FixedUpdate()
        {
            // Only perform the raycast if the mouse has actually moved
            // No need to waste the calculation for the raycast otherwise
            if (Input.mousePosition == m_LastMousePosition) return;
            m_LastMousePosition = Input.mousePosition;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo))
            {
                if (hitInfo.collider.TryGetComponent<IInteractable>(out var interactable))
                {
                    if (interactable == m_CurrentTarget) return;

                    if (m_CurrentTarget != null)
                    {
                        m_CurrentTarget.OnTargetingEnd();
                    }

                    m_CurrentTarget = interactable;
                    m_CurrentTarget.OnTargetingStart();
                }
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (m_CurrentTarget != null)
                {
                    m_CurrentTarget.OnClicked();
                }
            }
        }
    }
}