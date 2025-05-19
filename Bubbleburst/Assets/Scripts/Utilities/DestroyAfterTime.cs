using System.Collections;
using UnityEngine;

namespace DanMacC.BubbleBurst.Utilities
{
    public class DestroyAfterTime : MonoBehaviour
    {
        [SerializeField] private float m_Lifetime = 1.0f;

        private float m_TimeSoFar = 0.0f;

        private void Update()
        {
            m_TimeSoFar += Time.deltaTime;
            if (m_TimeSoFar > m_Lifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}