using DanMacC.BubbleBurst.Bubbles;
using DanMacC.BubbleBurst.Grid;
using DanMacC.BubbleBurst.UI;
using DanMacC.BubbleBurst.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace DanMacC.BubbleBurst.Game
{
    public class GameManager : Singleton<GameManager>
    {
        public static Difficulty SelectedDifficulty;

        public int NumGridCells => m_ActiveConfig.BoardCellCount.x * m_ActiveConfig.BoardCellCount.y;
        public GameConfigurationSO ActiveConfiguration => m_ActiveConfig;

        [SerializeField] private GameConfigurationSO[] m_GameConfigurations;
        [SerializeField] private GridManager m_GridManager;
        [SerializeField] private GameUI m_GameUI;

        private int m_Score = 0;
        private int m_NumMovesMade = 0;
        private int m_TotalNumPopped = 0;
        private GameConfigurationSO m_ActiveConfig;

        private void Start()
        {
            m_ActiveConfig = m_GameConfigurations.First(config => config.Difficulty == SelectedDifficulty);
            Assert.IsNotNull(m_ActiveConfig, $"Could not find a matching configuration for the selected difficulty: {SelectedDifficulty}");

            m_GridManager.GenerateGrid(m_ActiveConfig.BoardCellCount, m_ActiveConfig.BoardCellWorldSize);

            m_GameUI.UpdateGemCountUI(NumGridCells);
            m_GameUI.UpdateScoreUI(m_Score);
            m_GameUI.UpdateMoveCountUI(m_NumMovesMade);
        }

        public void RecordBubbleGroupPopped(Dictionary<Bubble, int> poppedBubbles)
        {
            m_TotalNumPopped += poppedBubbles.Count;
            m_GameUI.UpdateGemCountUI(NumGridCells - m_TotalNumPopped);

            m_NumMovesMade++;
            m_GameUI.UpdateMoveCountUI(m_NumMovesMade);

            CalculateScore(poppedBubbles);

            if (m_TotalNumPopped >= NumGridCells)
            {
                // Since new bubbles are not created, we can simply check the number of bubbles that have been popped to see if there are any left
                // This saves having to go through and check all of the cells to see if they are empty
                OnBoardCleared();
            }
            else
            {
                // Otherwise, we need to make the bubbles fall down with gravity and slide columns to the right
                // Then also need to check if there are any viable groups left. If not, the game is over
                StartCoroutine(DoBubblePopSequence());
                IEnumerator DoBubblePopSequence()
                {
                    MoveBubblesDown();
                    yield return new WaitUntil(() => !TweenManager.Instance.IsAnimating);

                    MoveColumnsOver();
                    yield return new WaitUntil(() => !TweenManager.Instance.IsAnimating);

                    CheckForGameOver();
                }
            }
        }

        /// <summary>
        /// Increase the score according to the algorith in the game design rules
        /// Score += n (n + 1), where n is the number of bubbles
        /// </summary>
        public void CalculateScore(Dictionary<Bubble, int> poppedBubbles)
        {
            int n = poppedBubbles.Count;
            int scoreIncrease = (n * (n + 1));

            m_Score += scoreIncrease;

            m_GameUI.UpdateScoreUI(m_Score);
        }

        public void MoveBubblesDown()
        {
            m_GridManager.MoveBubblesDown();
        }

        public void MoveColumnsOver()
        {
            m_GridManager.MoveBubblesRight();
        }

        public void CheckForGameOver()
        {
            if (!m_GridManager.CheckForValidMoves())
            {
                OnGameOver();
            }
        }

        public void OnBoardCleared()
        {
            Debug.Log("<color=green>YOU WIN!</color>");
        }

        public void OnGameOver()
        {
            Debug.Log($"<color=red>GAME OVER! Final Score = {m_Score}</color>");
        }
    }
}