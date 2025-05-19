using DanMacC.BubbleBurst.Bubbles;
using DanMacC.BubbleBurst.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DanMacC.BubbleBurst.Game
{
    public class GameManager : Singleton<GameManager>
    {
        private int m_Score = 0;

        public void RecordBubbleGroupPopped(List<(Bubble, int)> poppedBubbles)
        {
            CalculateScore(poppedBubbles);
        }

        /// <summary>
        /// Increase the score according to the algorith in the game design rules
        /// Score += n (n + 1), where n is the number of bubbles
        /// </summary>
        public void CalculateScore(List<(Bubble, int)> poppedBubbles)
        {
            int n = poppedBubbles.Count;
            int scoreIncrease = (n * (n + 1));

            m_Score += scoreIncrease;

            Debug.Log($"Clicked {n} bubbles, score increase is {scoreIncrease}, new score is {m_Score}");
        }
    }
}