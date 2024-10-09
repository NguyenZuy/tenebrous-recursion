using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Zuy.TenebrousRecursion.Authoring
{
    public class GridAuthoring : MonoBehaviour
    {
        public Vector2 size = new Vector2(10, 10);
        public float cellSize = 1f;
        public Color gridColor = Color.white;
        public Transform checker;
        public bool showGridGeneration = false;

        private List<GameObject> _cellObjects = new List<GameObject>();

        class Baker : Baker<GridAuthoring>
        {
            public override void Bake(GridAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);

                AddComponent(entity, new Component.Grid()
                {
                    cellSize = authoring.cellSize,
                });
            }
        }

        private void OnDrawGizmos()
        {
            if (showGridGeneration)
                DrawGrid();
        }

        private void DrawGrid()
        {
            Gizmos.color = gridColor;

            Vector3 origin = transform.position;
            int numCellsX = Mathf.FloorToInt(size.x / cellSize);
            int numCellsY = Mathf.FloorToInt(size.y / cellSize);

            for (int x = 0; x <= numCellsX; x++)
            {
                float xPos = x * cellSize;
                Vector3 startPoint = origin + new Vector3(xPos, 0, 0);
                Vector3 endPoint = origin + new Vector3(xPos, numCellsY * cellSize, 0);
                Gizmos.DrawLine(startPoint, endPoint);
            }

            for (int y = 0; y <= numCellsY; y++)
            {
                float yPos = y * cellSize;
                Vector3 startPoint = origin + new Vector3(0, yPos, 0);
                Vector3 endPoint = origin + new Vector3(numCellsX * cellSize, yPos, 0);
                Gizmos.DrawLine(startPoint, endPoint);
            }
        }

        public void GenerateGrid()
        {
            int numCellsX = Mathf.FloorToInt(size.x / cellSize);
            int numCellsY = Mathf.FloorToInt(size.y / cellSize);

            for (int x = 0; x < numCellsX; x++)
            {
                for (int y = 0; y < numCellsY; y++)
                {
                    GameObject cellObject = new GameObject($"Cell_{x}_{y}");
                    cellObject.transform.SetParent(transform);
                    cellObject.transform.localPosition = new Vector3(x * cellSize + cellSize / 2, y * cellSize + cellSize / 2, 0);

                    CellAuthoring cellAuthoring = cellObject.AddComponent<CellAuthoring>();
                    uint mortonCode = MortonCode.Encode(x, y);
                    // cellAuthoring.mortonCode = mortonCode;

                    _cellObjects.Add(cellObject);
                }
            }

            // Update the actual size of the grid
            size = new Vector2(numCellsX * cellSize, numCellsY * cellSize);
        }
        public bool IsEntityInsideCell(Vector3 entityPosition, uint cellMortonCode)
        {
            int gridX = Mathf.FloorToInt(entityPosition.x / cellSize);
            int gridY = Mathf.FloorToInt(entityPosition.y / cellSize);
            uint entityMortonCode = MortonCode.Encode(gridX, gridY);
            return entityMortonCode == cellMortonCode;
        }

    }
}

public static class MortonCode
{
    public static uint Encode(int x, int y)
    {
        return (uint)((Part1By1(y) << 1) + Part1By1(x));
    }

    public static (int x, int y) Decode(uint code)
    {
        return (Compact1By1(code), Compact1By1(code >> 1));
    }

    private static uint Part1By1(int n)
    {
        n &= 0x0000ffff;
        n = (n ^ (n << 8)) & 0x00ff00ff;
        n = (n ^ (n << 4)) & 0x0f0f0f0f;
        n = (n ^ (n << 2)) & 0x33333333;
        n = (n ^ (n << 1)) & 0x55555555;
        return (uint)n;
    }

    private static int Compact1By1(uint n)
    {
        n &= 0x55555555;
        n = (n ^ (n >> 1)) & 0x33333333;
        n = (n ^ (n >> 2)) & 0x0f0f0f0f;
        n = (n ^ (n >> 4)) & 0x00ff00ff;
        n = (n ^ (n >> 8)) & 0x0000ffff;
        return (int)n;
    }
}
