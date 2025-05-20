using DanMacC.BubbleBurst.Game;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DanMacC.BubbleBurst.UI
{
    public class Leaderboard : MonoBehaviour
    {
        public const int LEADERBOARD_LENGTH = 5;
        public const string LEADERBOARD_ID_TEMPLATE = "leaderboard_data_{0}_{1}";

        [SerializeField] private Difficulty m_Difficulty;
        [SerializeField] private Transform m_EntryUIParent;
        [SerializeField] private LeaderboardEntry m_EntryPrefab;

        private List<int> m_Scores = new();
        private List<LeaderboardEntry> m_SpawnedEntries = new();

        public string GetEntryKey(int index) => string.Format(LEADERBOARD_ID_TEMPLATE, m_Difficulty.ToString(), (index + 1));

        public void Initialize(Difficulty difficulty)
        {
            m_Difficulty = difficulty;
        }

        public void SaveLeaderboard()
        {
            for (int i = 0; i < m_Scores.Count; i++)
            {
                PlayerPrefs.SetInt(GetEntryKey(i), m_Scores[i]);
            }
        }

        public void LoadLeaderboard()
        {
            m_Scores = new();
            for (int i = 0; i < LEADERBOARD_LENGTH; ++i)
            {
                int score = PlayerPrefs.GetInt(GetEntryKey(i), 0);
                m_Scores.Add(score);
            }

            GenerateUIEntries();
        }

        public void ClearUIEntries()
        {
            foreach(var entry in m_SpawnedEntries)
            {
                Destroy(entry.gameObject);
            }
            m_SpawnedEntries.Clear();
        }

        public void GenerateUIEntries()
        {
            ClearUIEntries();

            for (int i = 0; i < m_Scores.Count; ++i)
            {
                LeaderboardEntry newEntry = Instantiate(m_EntryPrefab, m_EntryUIParent);
                newEntry.Initialize(this, i + 1, m_Scores[i]);

                m_SpawnedEntries.Add(newEntry);
            }
        }

        public void UpdateUIEntries()
        {
            for (int i = 0; i < m_SpawnedEntries.Count; ++i)
            {
                m_SpawnedEntries[i].UpdateData(i + 1, m_Scores[i]);
            }
        }
    }
}