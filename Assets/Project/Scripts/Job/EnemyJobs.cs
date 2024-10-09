using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Zuy.TenebrousRecursion.Component;
using Zuy.TenebrousRecursion.Utility;

namespace Zuy.TenebrousRecursion.Job
{
    [BurstCompile]
    public partial struct EncodeMortonJob : IJobEntity
    {
        [ReadOnly] public float cellSize;

        void Execute(ref Enemy enemy, in LocalTransform localTransform)
        {
            ConvertUtils.F3ToF2(localTransform.Position, out float2 pos);
            enemy.morton = MortonUtils.Encode(pos, cellSize);
        }
    }

    [BurstCompile]
    public partial struct GetCurCellJob : IJobEntity
    {
        [ReadOnly][DeallocateOnJobCompletion] public NativeArray<Cell> cells;
        [ReadOnly][DeallocateOnJobCompletion] public NativeArray<Entity> cellEntities;

        void Execute(ref Enemy enemy)
        {
            ulong morton = enemy.morton;
            for (int i = 0; i < cells.Length; i++)
            {
                enemy.curCell = IsInsideCell(morton, cells[i]) ? enemy.curCell = cellEntities[i] : Entity.Null;
            }
        }

        bool IsInsideCell(in ulong mortonCheck, in Cell cell)
        {
            return mortonCheck == cell.mortonCode;
        }
    }
}