using UnityEngine;
using System.Collections.Generic;

namespace DanMacC.BubbleBurst.Utilities
{
    public class TweenManager : Singleton<TweenManager>
    {
        public bool IsAnimating => m_ActiveTweens.Count > 0;

        private List<Vector3Tween> m_ActiveTweens = new();

        public Vector3Tween CreateTween()
        {
            Vector3Tween newTween = new Vector3Tween();
            m_ActiveTweens.Add(newTween);

            return newTween;
        }

        private void Update()
        {
            // Update any active tweens and then clear the ones that are now completed   
            foreach (var tween in m_ActiveTweens)
            {
                tween.UpdateTween(Time.deltaTime);
            }

            m_ActiveTweens.RemoveAll(tween => tween.IsComplete);
        }
    }
}