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

        [Header("Central Panel")]
        [SerializeField] private GameObject m_CenterPanelRoot;
        [SerializeField] private GameObject m_GameOverPanel;
        [SerializeField] private GameObject m_VictoryPanel;
        [SerializeField] private GameObject m_LeaderboardUpdatedText;
        [SerializeField] private GameObject m_LeaderboardNotUpdatedText;

        public void SetupLeaderboard(Difficulty difficulty)
        {
            m_Leaderboard.Initialize(difficulty);
            m_Leaderboard.LoadLeaderboard();

            m_CenterPanelRoot.SetActive(false);
            m_GameOverPanel.SetActive(false);
            m_VictoryPanel.SetActive(false);
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

        public void ShowGameOverUI()
        {
            m_CenterPanelRoot.SetActive(true);
            m_GameOverPanel.SetActive(true);
        }

        public void ShowVictoryUI()
        {
            m_CenterPanelRoot.SetActive(true);
            m_VictoryPanel.SetActive(true);
        }

        public void RecordLeaderboardScore(int newScore)
        {
            bool newHighScore = m_Leaderboard.RecordScore(newScore);
            m_LeaderboardUpdatedText.SetActive(newHighScore);
            m_LeaderboardNotUpdatedText.SetActive(!newHighScore);
        }

        public void OnMainMenuButtonPressed()
        {
            SceneManager.LoadScene(m_MenuSceneID);
        }
    }
}