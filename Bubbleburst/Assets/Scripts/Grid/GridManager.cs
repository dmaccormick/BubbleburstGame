using UnityEngine;
using DanMacC.BubbleBurst.Bubbles;
using System.Linq;
using DanMacC.BubbleBurst.Utilities.Extensions;
using DanMacC.BubbleBurst.Game;
using System.Collections.Generic;
using DanMacC.BubbleBurst.Utilities;

namespace DanMacC.BubbleBurst.Grid
{
    public class GridManager : MonoBehaviour
    {
        public const float GEM_FALL_SPEED = 5.0f;
        public const float GEM_COLUMN_SLIDE_SPEED = 5.0f;

        public float HalfCellWorldSize => m_GridCellWorldSize * 0.5f;

        [Header("Grid References")]
        [SerializeField] private Transform m_CenterAnchor;
        [SerializeField] private Transform m_CellParent;
        [SerializeField] private GridCell m_CellPrefab;

        [Header("Bubbles")]
        [SerializeField] private Bubble[] m_BubblePrefabs;

        private Vector2Int m_GridCellCount;
        private float m_GridCellWorldSize;
        private GridCell[,] m_Cells;

        /// <summary>
        /// Generates the grid from the bottom left
        /// Meaning the bottom left will be (0, 0) and the top will be (width, height)
        /// The grid is centered around the Transform assigned in the inspector (presumably the camera focus point)
        /// </summary>
        public void GenerateGrid(Vector2Int gridCellCount, float cellWorldSize)
        {
            m_GridCellCount = gridCellCount;
            m_GridCellWorldSize = cellWorldSize;

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

                    if (TryGetCell(cell.GridCoord + Vector2Int.up, out var upNeighbour)) cell.SetNeighbour(Vector2Int.up, upNeighbour);
                    if (TryGetCell(cell.GridCoord + Vector2Int.right, out var rightNeighbour)) cell.SetNeighbour(Vector2Int.right, rightNeighbour);
                    if (TryGetCell(cell.GridCoord + Vector2Int.down, out var downNeighbour)) cell.SetNeighbour(Vector2Int.down, downNeighbour);
                    if (TryGetCell(cell.GridCoord + Vector2Int.left, out var leftNeighbour)) cell.SetNeighbour(Vector2Int.left, leftNeighbour);
                }
            }
        }

        private void CleanUpGrid()
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
        /// Get the bubble prefab associated with the requested colour
        /// Can return null if there is no match found
        /// </summary>
        public Bubble GetBubblePrefab(BubbleColour colour)
        {
            return m_BubblePrefabs.FirstOrDefault(x => x.Colour == colour);
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

            int cellSpacesToMoveX = (m_GridCellCount.x - 1) / 2;
            bottomLeftPosition.x = m_CenterAnchor.position.x - (cellSpacesToMoveX * m_GridCellWorldSize);
            bottomLeftPosition.x -= (m_GridCellCount.x.IsEven()) ? HalfCellWorldSize : 0.0f;

            int cellSpacesToMoveY = (m_GridCellCount.y - 1) / 2;
            bottomLeftPosition.y = m_CenterAnchor.position.y - (cellSpacesToMoveX * m_GridCellWorldSize);
            bottomLeftPosition.y -= (m_GridCellCount.y.IsEven()) ? HalfCellWorldSize : 0.0f;

            bottomLeftPosition.z = m_CenterAnchor.position.z;

            return bottomLeftPosition;
        }

        /// <summary>
        /// Go through all of the bubbles in the grid and move them down with gravity if needed
        /// The bottom row would never need to drop so can skip over that and start on the second row
        /// Then work upwards so all bubbles would only need to move a maximum of once
        /// </summary>
        public void MoveBubblesDown()
        {
            for (int x = 0; x < m_Cells.GetLength(0); ++x)
            {
                for (int y = 1; y < m_Cells.GetLength(1); ++y)
                {
                    // Skip over this cell if there is no bubble in it
                    var startingCell = m_Cells[x, y];
                    if (startingCell.IsEmpty) continue;

                    // Start searching down from this point and find the next space to drop down to if applicable
                    var targetCell = startingCell;
                    for (int checkingY = y - 1; checkingY >= 0; --checkingY)
                    {
                        var checkingCell = m_Cells[x, checkingY];

                        if (checkingCell.IsEmpty)
                        {
                            // There is a space here so it becomes the new target
                            targetCell = checkingCell;
                        }
                        else
                        {
                            // There is not a space here so we are done searching
                            break;
                        }
                    }

                    // If we found a new space to move to, transfer the bubble there
                    if (targetCell != startingCell)
                    {
                        var bubble = startingCell.RemoveBubble();
                        targetCell.AttachBubble(bubble, false);

                        Vector3Tween bubbleMoveTween = TweenManager.Instance.CreateTween();
                        bubbleMoveTween.StartTween(
                            bubble.transform.position,
                            targetCell.BubbleAnchorPosition,
                            GEM_FALL_SPEED,
                            tweenedPos => bubble.transform.position = tweenedPos);
                    }
                }
            }
        }

        /// <summary>
        /// Go through all the columns in the grid and then shift them to the right if there is space
        /// Similar to the rows, can start one column over since the right-most column definitely can't move
        /// </summary>
        public void MoveBubblesRight()
        {
            // -2 since length would be out of bounds AND start from one column shifted over
            for (int x = m_Cells.GetLength(0) - 2; x >= 0; --x)
            {
                // If this column is empty, there is nothing to move
                if (IsColumnEmpty(x)) continue;

                // Check the columns to the right of this one to see if they are empty
                int targetX = x;
                for (int checkingColIndex = targetX + 1; checkingColIndex < m_Cells.GetLength(0); ++checkingColIndex)
                {
                    if (IsColumnEmpty(checkingColIndex))
                    {
                        // This column is empty so becomes a possible new spot to go to
                        targetX = checkingColIndex;
                    }
                    else
                    {
                        // This column has at least one bubble in it and so cannot be moved into
                        break;
                    }
                }

                // If there is a new target column, transfer all the bubbles there
                if (x != targetX)
                {
                    for (int y = 0; y < m_Cells.GetLength(1); ++y)
                    {
                        var startingCell = m_Cells[x, y];
                        var targetCell = m_Cells[targetX, y];

                        if (!startingCell.IsEmpty)
                        {
                            var bubble = startingCell.RemoveBubble();
                            targetCell.AttachBubble(bubble, false);

                            Vector3Tween bubbleMoveTween = TweenManager.Instance.CreateTween();
                            bubbleMoveTween.StartTween(
                                bubble.transform.position,
                                targetCell.BubbleAnchorPosition,
                                GEM_COLUMN_SLIDE_SPEED,
                                tweenedPos => bubble.transform.position = tweenedPos);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Bubbles will always fall down towards the bottom
        /// So, can simply check if the bottom row has a bubble to determine if the column is empty or not
        /// </summary>
        public bool IsColumnEmpty(int x) => m_Cells[x, 0].IsEmpty;

        /// <summary>
        /// Do a search of the board to see if any of the remaining cells hold bubbles which can be grouped
        /// Start from the bottom right of the board because the bubbles shift down and right. Therefore, the most likely place to have bubbles left in the end game
        /// This should reduce the amount of cells searched dramatically
        /// Similarly, exit as soon as a valid move has been found instead of continuing to search
        /// Also, keep track of which cells have been searched already to avoid doing extra work
        /// </summary>
        public bool CheckForValidMoves()
        {
            HashSet<GridCell> searchedCells = new();

            for (int x = m_Cells.GetLength(0) - 1; x >= 0; --x)
            {
                for (int y = 0; y < m_Cells.GetLength(1); ++y)
                {
                    var cell = m_Cells[x, y];

                    if (searchedCells.Contains(cell)) continue;
                    searchedCells.Add(cell);

                    if (cell.IsEmpty) continue;

                    foreach(var neighbourCell in cell.NeighbourCells)
                    {
                        searchedCells.Add(neighbourCell);

                        if (CheckForMatchWithCell(cell, neighbourCell))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool CheckForMatchWithCell(GridCell checkingCell, GridCell targetCell)
        {
            return targetCell != null
                && !targetCell.IsEmpty
                && checkingCell.Bubble.Colour == targetCell.Bubble.Colour;
        }
    }
}