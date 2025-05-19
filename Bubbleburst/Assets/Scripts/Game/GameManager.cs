using DanMacC.BubbleBurst.Bubbles;
using DanMacC.BubbleBurst.Grid;
using DanMacC.BubbleBurst.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace DanMacC.BubbleBurst.Game
{
    public class GameManager : Singleton<GameManager>
    {
        public int NumGridCells => m_GridCellCount.x * m_GridCellCount.y;

        [Header("Grid Generation")]
        [SerializeField] private GridManager m_GridManager;
        [SerializeField] private Vector2Int m_GridCellCount = new Vector2Int(7, 7);
        [SerializeField] private float m_GridWorldCellSize = 1.0f;

        private int m_Score = 0;
        private int m_TotalNumPopped = 0;

        private void Start()
        {
            Assert.IsTrue(m_GridCellCount.x > 0 && m_GridCellCount.y > 0, "The grid needs to have valid dimensions!");
            m_GridManager.GenerateGrid(m_GridCellCount, m_GridWorldCellSize);
        }

        public void RecordBubbleGroupPopped(Dictionary<Bubble, int> poppedBubbles)
        {
            m_TotalNumPopped += poppedBubbles.Count;

            CalculateScore(poppedBubbles);

            if (m_TotalNumPopped == NumGridCells)
            {
                // Since new bubbles are not created, we can simply check the number of bubbles that have been popped to see if there are any left
                // This saves having to go through and check all of the cells to see if they are empty
                OnBoardCleared();
            }
            else
            {
                // Otherwise, we need to make the bubbles fall down with gravity and slide columns to the right
                // Then also need to check if there are any viable groups left. If not, the game is over
                MoveBubblesDown();
                MoveColumnsOver();
                CheckForGameOver();
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

            Debug.Log($"Clicked {n} bubbles, score increase is {scoreIncrease}, new score is {m_Score}");
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