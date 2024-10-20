using System.Collections.Generic;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using Zuy.TenebrousRecursion.Utility;

namespace Zuy.TenebrousRecursion.Authoring
{
    public class GridAuthoring : MonoBehaviour
    {
        public Vector2Int gridSize = new Vector2Int(10, 10);
        public int cellDiameter = 1;
        public Color gridColor = Color.white;
        public bool showGridGeneration = false;
        public bool showCellIndices = true; // New option to toggle cell index display
        public Color cellIndexColor = Color.yellow; // Color for cell index text
        public Color impassibleColor;

        class Baker : Baker<GridAuthoring>
        {
            public override void Bake(GridAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);

                AddComponent(entity, new Component.Grid()
                {
                    gridSize = ConvertUtils.V2ToI2(authoring.gridSize),
                    cellDiameter = authoring.cellDiameter,
                });
            }
        }

        private void OnDrawGizmos()
        {
            if (showGridGeneration)
            {
                DrawGrid();
                DrawImpassibleCells(); // Draw the impassible cells

                if (showCellIndices)
                {
                    DrawCellIndices();
                }
            }
        }

        private void DrawImpassibleCells()
        {
            Gizmos.color = impassibleColor; // Color for impassible cells

            foreach (Transform child in transform)
            {
                var cellAuthoring = child.GetComponent<CellAuthoring>();
                if (cellAuthoring != null && cellAuthoring.isImpassible)
                {
                    Vector3 cellPosition = child.transform.position;
                    // Draw a filled cube to mark the impassible cell
                    Gizmos.DrawCube(cellPosition, new Vector3(cellDiameter, cellDiameter, 0.1f));
                }
            }
        }


        private void DrawGrid()
        {
            Gizmos.color = gridColor;

            Vector3 origin = transform.position;
            int numCellsX = Mathf.FloorToInt(gridSize.x / cellDiameter);
            int numCellsY = Mathf.FloorToInt(gridSize.y / cellDiameter);

            for (int x = 0; x <= numCellsX; x++)
            {
                float xPos = x * cellDiameter;
                Vector3 startPoint = origin + new Vector3(xPos, 0, 0);
                Vector3 endPoint = origin + new Vector3(xPos, numCellsY * cellDiameter, 0);
                Gizmos.DrawLine(startPoint, endPoint);
            }

            for (int y = 0; y <= numCellsY; y++)
            {
                float yPos = y * cellDiameter;
                Vector3 startPoint = origin + new Vector3(0, yPos, 0);
                Vector3 endPoint = origin + new Vector3(numCellsX * cellDiameter, yPos, 0);
                Gizmos.DrawLine(startPoint, endPoint);
            }
        }

        private void DrawCellIndices()
        {
            int numCellsX = Mathf.FloorToInt(gridSize.x / cellDiameter);
            int numCellsY = Mathf.FloorToInt(gridSize.y / cellDiameter);
            Vector3 origin = transform.position;

            for (int x = 0; x < numCellsX; x++)
            {
                for (int y = 0; y < numCellsY; y++)
                {
                    Vector3 cellCenter = origin + new Vector3(
                        (x + 0.5f) * cellDiameter,
                        (y + 0.5f) * cellDiameter,
                        0
                    );

                    // Draw the cell index
                    Handles.color = cellIndexColor;
                    Handles.Label(cellCenter, $"({x},{y})", GUI.skin.label);
                }
            }
        }

        public void GenerateGrid()
        {
            int numCellsX = Mathf.FloorToInt(gridSize.x / cellDiameter);
            int numCellsY = Mathf.FloorToInt(gridSize.y / cellDiameter);

            for (int x = 0; x < numCellsX; x++)
            {
                for (int y = 0; y < numCellsY; y++)
                {
                    GameObject cellObject = new GameObject($"Cell_{x}_{y}");
                    cellObject.transform.SetParent(transform);
                    cellObject.transform.localPosition = new Vector3(x * cellDiameter + cellDiameter / 2, y * cellDiameter + cellDiameter / 2, 0);
                }
            }

            // Update the actual size of the grid
            gridSize = new Vector2Int(numCellsX * cellDiameter, numCellsY * cellDiameter);
        }
    }
}