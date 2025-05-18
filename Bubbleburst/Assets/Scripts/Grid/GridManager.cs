using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace DanMacC.BubbleBurst.Grid
{
    public class GridManager : MonoBehaviour
    {
        public float HalfCellWorldSize => m_GridCellWorldSize * 0.5f;

        [SerializeField] private Transform m_CenterAnchor;
        [SerializeField] private Transform m_CellParent;

        [SerializeField] private GridCell m_CellPrefab;

        [SerializeField] private Vector2Int m_GridCellCount = new Vector2Int(5, 5);
        [SerializeField] private float m_GridCellWorldSize = 1.0f;

        private GridCell[,] m_Cells;

        private void Awake()
        {
            GenerateGrid();
        }

        /// <summary>
        /// Generates the grid from the bottom left
        /// Meaning the bottom left will be (0, 0) and the top will be (width, height)
        /// The grid is centered around the Transform assigned in the inspector (presumably the camera focus point)
        /// </summary>
        public void GenerateGrid()
        {
            CleanUpGrid();

            GenerateCells();
            LinkCellNeighbours();
        }

        private void GenerateCells()
        {
            m_Cells = new GridCell[m_GridCellCount.x, m_GridCellCount.y];

            Vector3 centerWorldPosition = m_CenterAnchor.position;
            Vector3 bottomLeftPosition = CalcBottomLeftWorldPos();

            // Create all of the cells
            for (int x = 0; x < m_GridCellCount.x; ++x)
            {
                for (int y = 0; y < m_GridCellCount.y; ++y)
                {
                    Vector3 cellPosition = bottomLeftPosition + new Vector3(x * m_GridCellWorldSize, y * m_GridCellWorldSize, 0.0f);

                    GridCell cellInstance = Instantiate(m_CellPrefab, cellPosition, Quaternion.identity, m_CellParent);
                    cellInstance.Initialize(this, x, y);

                    m_Cells[x, y] = cellInstance;
                }
            }
        }

        private void LinkCellNeighbours()
        {
            for (int x = 0; x < m_GridCellCount.x; ++x)
            {
                for (int y = 0; y < m_GridCellCount.y; ++y)
                {
                    GridCell cell = m_Cells[x, y];

                    if (TryGetCell(cell.GridCoord + Vector2Int.up, out var upNeighbour))        cell.SetNeighbour(Vector2Int.up, upNeighbour);
                    if (TryGetCell(cell.GridCoord + Vector2Int.right, out var rightNeighbour))  cell.SetNeighbour(Vector2Int.right, rightNeighbour);
                    if (TryGetCell(cell.GridCoord + Vector2Int.down, out var downNeighbour))    cell.SetNeighbour(Vector2Int.down, downNeighbour);
                    if (TryGetCell(cell.GridCoord + Vector2Int.left, out var leftNeighbour))    cell.SetNeighbour(Vector2Int.left, leftNeighbour);
                }
            }

        }

        public void CleanUpGrid()
        {
            if (m_Cells == null) return;

            foreach (GridCell cell in m_Cells)
            {
                if (cell != null)
                {
                    Destroy(cell.gameObject);
                }
            }
        }

        public bool TryGetCell(Vector2Int coord, out GridCell cell) => TryGetCell(coord.x, coord.y, out cell);
        public bool TryGetCell(int x, int y, out GridCell cell)
        {
            if (x >= 0 && x < m_Cells.GetLength(0))
            {
                if (y >= 0 && y < m_Cells.GetLength(1))
                {
                    cell = m_Cells[x, y];
                    return true;
                }
            }

            cell = null;
            return false;
        }

        /// <summary>
        /// Bottom left world position algorithm:
        // - Starts at the center of the grid in world space
        // - Move left half the grid size, but subtract 1 since we start in the middle already
        // - If the grid is even, subtract an extra 1/2 step so it lands in the actual center of the cell, not the edge
        // ---> This is because the starting center point will be on the middle edge, not the middle cell center
        /// </summary>
        private Vector3 CalcBottomLeftWorldPos()
        {
            Vector3 bottomLeftPosition = new Vector3();

            int cellSpacesToMoveX = m_GridCellCount.x / 2;
            bottomLeftPosition.x = m_CenterAnchor.position.x - (cellSpacesToMoveX * m_GridCellWorldSize);
            bottomLeftPosition.x -= (m_GridCellCount.x.IsEven()) ? HalfCellWorldSize : 0.0f;

            int cellSpacesToMoveY = m_GridCellCount.y / 2;
            bottomLeftPosition.y = m_CenterAnchor.position.y - (cellSpacesToMoveX * m_GridCellWorldSize);
            bottomLeftPosition.y -= (m_GridCellCount.y.IsEven()) ? HalfCellWorldSize : 0.0f;

            bottomLeftPosition.z = m_CenterAnchor.position.z;

            return bottomLeftPosition;
        }
    }
}