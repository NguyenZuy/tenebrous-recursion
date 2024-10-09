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
    public partial struct EncodeMortonForPlayerJob : IJobEntity
    {
        [ReadOnly] public float cellSize;

        void Execute(ref Player player, in LocalTransform localTransform)
        {
            ConvertUtils.F3ToF2(localTransform.Position, out float2 pos);
            player.mortonCode = MortonUtils.Encode(pos, cellSize);
        }
    }

    [BurstCompile]
    public partial struct GetCurCellForPlayerJob : IJobEntity
    {
        [ReadOnly][DeallocateOnJobCompletion] public NativeArray<Cell> cells;
        [ReadOnly][DeallocateOnJobCompletion] public NativeArray<Entity> cellEntities;

        void Execute(ref Player player)
        {
            uint mortonCode = player.mortonCode;
            for (int i = 0; i < cells.Length; i++)
            {
                if (MortonUtils.IsInsideCell(mortonCode, cells[i]))
                {
                    player.curCell = cellEntities[i];
                    return;
                }
            }
        }
    }
}
