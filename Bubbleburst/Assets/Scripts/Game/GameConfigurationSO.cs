using System.Collections;
using UnityEngine;

namespace DanMacC.BubbleBurst.Game
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    [CreateAssetMenu(fileName = "GameConfigurationSO", menuName = "ScriptableObjects/GameConfiguration", order = 1)]
    public class GameConfigurationSO : ScriptableObject
    {
        public Difficulty Difficulty => m_Difficulty;
        public Vector2Int BoardCellCount => m_BoardCellCount;
        public float BoardCellWorldSize => m_BoardCellWorldSize;

        [SerializeField] private Difficulty m_Difficulty;
        [SerializeField] private Vector2Int m_BoardCellCount;
        [SerializeField] private float m_BoardCellWorldSize = 1.0f;
    }
}