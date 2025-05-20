using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DanMacC.BubbleBurst.Utilities
{
    /// <summary>
    /// Simple Vector3Tween class that will always lerp between Start and End
    /// Normally, I would use a library like DoTween but not every project has it
    /// In the future, this would be expanded with different easing styles, as it only lerps for now
    /// Also, would likely have a centralized TweenManager system that handles starting/stopping tweens automatically
    /// </summary>
    public class Vector3Tween
    {
        public bool IsComplete => m_CurrentTime >= Duration;

        public float Duration;

        public Action<Vector3> OnUpdate;
        public Action<Vector3Tween> OnComplete;

        public Vector3 Start;
        public Vector3 End;

        private float m_CurrentTime = 0.0f;
        
        public void StartTween(Vector3 start, Vector3 end, float speed, Action<Vector3> onUpdate = null, Action<Vector3Tween> onComplete = null)
        {
            Start = start;
            End = end;
            Duration = Vector3.Distance(start, end) / speed;

            OnUpdate = onUpdate;
            OnComplete = onComplete;

            m_CurrentTime = 0.0f;
        }

        public virtual Vector3 UpdateTween(float dt)
        {
            m_CurrentTime = Mathf.Clamp(m_CurrentTime + dt, 0.0f, Duration);
            float lerpT = m_CurrentTime / Duration;

            Vector3 updatedValue = Vector3.Lerp(Start, End, lerpT);

            OnUpdate?.Invoke(updatedValue);
            if (lerpT >= 1.0f)
            {
                OnComplete?.Invoke(this);
            }

            return updatedValue;
        }
    }
}