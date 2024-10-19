using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Zuy.TenebrousRecursion.Component;

public class GridDebug : MonoBehaviour
{
    public bool displayGrid;
    public Sprite iconTarget;
    public Sprite iconArrow;
    public Sprite iconImpassible;
    public GameObject prefabIcon;

    private World _world;
    private EntityManager _entityManager;
    private EntityQuery _cellQuery;
    private EntityQuery _gridQuery;

    private int2 drawGridSize;
    private float drawCellRadius;
    private int drawCellDiameter;

    private Zuy.TenebrousRecursion.Component.Grid grid;

    private Cell[] _cells;

    private void Start()
    {
        _world = World.DefaultGameObjectInjectionWorld;
        _entityManager = _world.EntityManager;

        _cellQuery = _entityManager.CreateEntityQuery(typeof(Cell));
        _gridQuery = _entityManager.CreateEntityQuery(typeof(Zuy.TenebrousRecursion.Component.Grid));
    }

    void Update()
    {
        if (!displayGrid)
            return;

        if (_gridQuery.TryGetSingleton(out grid))
        {
            drawGridSize = grid.gridSize;
            drawCellRadius = grid.cellDiameter / 2f;
            drawCellDiameter = grid.cellDiameter;

            if (_cellQuery.CalculateEntityCount() > 0)
            {
                NativeArray<Cell> cells = _cellQuery.ToComponentDataArray<Cell>(Allocator.TempJob);
                _cells = cells.ToArray();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DrawIcon();
        }
    }

    private void DrawIcon()
    {
        if (transform.childCount == 0)
        {
            for (int i = 0; i < _cells.Length; i++)
            {
                Instantiate(prefabIcon, transform);
            }
        }

        for (int i = 0; i < _cells.Length; i++)
        {
            var cell = _cells[i];

            var obj = transform.GetChild(i);
            var spriteRenderer = obj.GetComponent<SpriteRenderer>();

            obj.transform.position = new Vector3(cell.gridIndex.x * drawCellDiameter + drawCellRadius, cell.gridIndex.y * drawCellDiameter + drawCellRadius, 0);

            if (cell.bestCost == 0)
                spriteRenderer.sprite = iconTarget;
            else if (cell.bestCost > 0)
            {
                spriteRenderer.sprite = iconArrow;

                int angle = (cell.curDirection.x, cell.curDirection.y) switch
                {
                    (0, 1) => 0,
                    (0, -1) => 180,
                    (1, 0) => 270,
                    (-1, 0) => 90,
                    (1, 1) => 315,
                    (-1, 1) => 45,
                    (1, -1) => 225,
                    (-1, -1) => 135,
                    (0, 0) => 0,
                    _ => 0
                };
                obj.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
            // else if (cell.cost)
            //     spriteRenderer.sprite = iconImpassible;
        }
    }

    private void OnDrawGizmos()
    {
        if (displayGrid)
        {
            Gizmos.color = Color.white;
            for (int x = 0; x < drawGridSize.x; x++)
            {
                for (int y = 0; y < drawGridSize.y; y++)
                {
                    Vector3 center = new Vector3(drawCellRadius * 2 * x + drawCellRadius, drawCellRadius * 2 * y + drawCellRadius, 0);
                    Vector3 size = Vector3.one * drawCellRadius * 2;
                    Gizmos.DrawWireCube(center, size);
                }
            }
        }
    }
}
