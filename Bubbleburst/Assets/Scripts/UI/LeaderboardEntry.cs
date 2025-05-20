using System.Collections;
using TMPro;
using UnityEngine;

namespace DanMacC.BubbleBurst.UI
{
    public class LeaderboardEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_TxtPosition;
        [SerializeField] private TextMeshProUGUI m_TxtScore;

        private Leaderboard m_Leaderboard;
        private int m_Position;
        private int m_Score;

        public void Initialize(Leaderboard leaderboard, int position, int score)
        {
            m_Leaderboard = leaderboard;

            UpdateData(position, score);
        }

        public void UpdateData(int position, int score)
        {
            m_Position = position;
            m_Score = score;

            UpdateVisuals();
        }

        public void UpdateVisuals()
        {
            m_TxtPosition.text = $"{m_Position.ToString()}.";
            m_TxtScore.text = (m_Score > 0) ? m_Score.ToString() : "- - -";
        }
    }
}