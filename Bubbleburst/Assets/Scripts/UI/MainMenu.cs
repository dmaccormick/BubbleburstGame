using DanMacC.BubbleBurst.Game;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DanMacC.BubbleBurst.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private string m_GameSceneID = "1_Game";
        [SerializeField] private Leaderboard[] m_Leaderboards;
        [SerializeField] private GameConfigurationSO[] m_GameConfigs;

        private void Start()
        {
            foreach(var leaderboard in m_Leaderboards)
            {
                leaderboard.LoadLeaderboard();
            }
        }

        public void LoadDifficulty(int difficultyIndex)
        {
            GameManager.SelectedDifficulty = (Difficulty)difficultyIndex;
            SceneManager.LoadScene(m_GameSceneID);
        }
    }
}