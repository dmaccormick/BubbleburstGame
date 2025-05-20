using DanMacC.BubbleBurst.Game;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DanMacC.BubbleBurst.UI
{
    public class GameUI : MonoBehaviour
    {
        [Header("Left Sidebar")]
        [SerializeField] private string m_MenuSceneID = "0_Menu";
        [SerializeField] private Leaderboard m_Leaderboard;

        [Header("Right Sidebar")]
        [SerializeField] private TextMeshProUGUI m_TxtScore;
        [SerializeField] private TextMeshProUGUI m_GemCount;
        [SerializeField] private TextMeshProUGUI m_MoveCount;

        public void SetupLeaderboard(Difficulty difficulty)
        {
            m_Leaderboard.Initialize(difficulty);
            m_Leaderboard.LoadLeaderboard();
        }

        public bool RecordLeaderboardScore(int newScore)
        {
            return m_Leaderboard.RecordScore(newScore);
        }

        public void UpdateScoreUI(int newScore)
        {
            m_TxtScore.text = newScore.ToString();
        }

        public void UpdateGemCountUI(int newGemCount)
        {
            m_GemCount.text = newGemCount.ToString();
        }

        public void UpdateMoveCountUI(int newMoveCount)
        {
            m_MoveCount.text = newMoveCount.ToString();
        }

        public void OnMainMenuButtonPressed()
        {
            SceneManager.LoadScene(m_MenuSceneID);
        }
    }
}