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
        void Execute(ref Enemy enemy, in LocalTransform localTransform)
        {
            ConvertUtils.F3ToF2(localTransform.Position, out float2 pos);
            UnityEngine.Debug.Log("After: " + pos);
            enemy.morton = MortonUtils.PosToMorton(pos);
            UnityEngine.Debug.Log("Morton Code: " + MortonUtils.PosToMorton(pos));
        }
    }

    [BurstCompile]
    public partial struct GetCurCellJob : IJobEntity
    {
        [ReadOnly][DeallocateOnJobCompletion] public NativeArray<Cell> cells;
        [ReadOnly][DeallocateOnJobCompletion] public NativeArray<Entity> cellEntities;

        void Execute(ref Enemy enemy)
        {
            int morton = enemy.morton;
            for (int i = 0; i < cells.Length; i++)
            {
                if (IsInsideCell(morton, cells[i]))
                {
                    enemy.curCell = cellEntities[i];
                    return;
                }
            }
        }

        bool IsInsideCell(in int mortonCheck, in Cell cell)
        {
            UnityEngine.Debug.Log($"Check {mortonCheck} with {cell.minMorton} and {cell.maxMorton}");
            return mortonCheck >= cell.minMorton && mortonCheck <= cell.maxMorton;
        }
    }
}