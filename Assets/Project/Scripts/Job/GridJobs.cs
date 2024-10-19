using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Zuy.TenebrousRecursion.Component;
using Zuy.TenebrousRecursion.Constant;

namespace Zuy.TenebrousRecursion.Job
{
    [BurstCompile]
    public partial struct GetInsideAgentsJob : IJobEntity
    {
        [ReadOnly][DeallocateOnJobCompletion] public NativeArray<Enemy> enemies;
        [ReadOnly][DeallocateOnJobCompletion] public NativeArray<Entity> enemyEntities;
        [ReadOnly] public float elapsedTime;

        void Execute(ref Cell cell, ref DynamicBuffer<CellMember> cellMembers, in Entity entity)
        {
            if (!(elapsedTime - cell.lastTimeToExecuteJob >= 2f) && cell.lastTimeToExecuteJob != 0f)
            {
                return;
            }

            cell.lastTimeToExecuteJob = elapsedTime;

            cellMembers.Clear();
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].curCell == entity)
                {
                    cellMembers.Add(new CellMember()
                    {
                        memberEntity = enemyEntities[i]
                    });
                }
            }
        }
    }

    // FFP - Flow Field Pathfinding
    [BurstCompile]
    public partial struct CalculateFFPIntegrationFieldJob : IJobChunk
    {
        [ReadOnly] public Cell playerCurCell;

        public ComponentTypeHandle<Cell> cellTypeHandle;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            NativeArray<Cell> cells = chunk.GetNativeArray(ref cellTypeHandle);

            var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);
            while (enumerator.NextEntityIndex(out int i))
            {
                Cell cell = cells[i];
                cell.bestCost = math.abs(playerCurCell.gridIndex.x - cell.gridIndex.x) + math.abs(playerCurCell.gridIndex.y - cell.gridIndex.y);

                cells[i] = cell;
            }
        }
    }

    [BurstCompile]
    public partial struct CalculateFFPFlowFieldJob : IJobChunk
    {
        public ComponentTypeHandle<Cell> cellTypeHandle;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            NativeArray<Cell> cells = chunk.GetNativeArray(ref cellTypeHandle);

            var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);
            while (enumerator.NextEntityIndex(out int i))
            {
                Cell cell = cells[i];
                GetNeighborCells(cell, cells, out NativeList<Cell> neighborCells);
                GetLastLowestBestCostCell(neighborCells, out Cell lowestBestCostCell);
                GetDirection(ref cell, lowestBestCostCell);

                cells[i] = cell;
                neighborCells.Dispose();
            }

            cells.Dispose();
        }

        void GetNeighborCells(in Cell rootCell, in NativeArray<Cell> cells, out NativeList<Cell> neighborCells)
        {
            neighborCells = new NativeList<Cell>(7, Allocator.Temp);

            int2 rootCellGridIndex = rootCell.gridIndex;
            var allDirections = Direction.AllDirections();

            foreach (var cell in cells)
            {
                foreach (var direction in allDirections)
                {
                    if (cell.gridIndex.Equals(rootCellGridIndex + direction))
                    {
                        neighborCells.AddNoResize(cell);
                        break;
                    }
                }
            }
        }

        void GetLastLowestBestCostCell(in NativeList<Cell> neighborCells, out Cell result)
        {
            result = new Cell();

            int bestCost = int.MaxValue;
            foreach (var cell in neighborCells)
            {
                if (cell.bestCost <= bestCost)
                {
                    bestCost = cell.bestCost;
                    result = cell;
                }
            }
        }

        void GetDirection(ref Cell cell, in Cell neighborWithLowestBestCostCell)
        {
            cell.curDirection = neighborWithLowestBestCostCell.gridIndex - cell.gridIndex;
        }
    }
}
