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
    public partial struct EncodeMortonForEnemyJob : IJobEntity
    {
        [ReadOnly] public float cellSize;

        void Execute(ref Enemy enemy, in LocalTransform localTransform)
        {
            ConvertUtils.F3ToF2(localTransform.Position, out float2 pos);
            enemy.mortonCode = MortonUtils.Encode(pos, cellSize);
        }
    }

    [BurstCompile]
    public partial struct GetCurCellForEnemyJob : IJobEntity
    {
        [ReadOnly][DeallocateOnJobCompletion] public NativeArray<Cell> cells;
        [ReadOnly][DeallocateOnJobCompletion] public NativeArray<Entity> cellEntities;

        void Execute(ref Enemy enemy)
        {
            uint mortonCode = enemy.mortonCode;
            for (int i = 0; i < cells.Length; i++)
            {
                if (MortonUtils.IsInsideCell(mortonCode, cells[i]))
                {
                    enemy.curCell = cellEntities[i];
                    return;
                }
            }
        }
    }

    [BurstCompile]
    public partial struct SpriteAnimatedJob : IJobEntity
    {
        [ReadOnly] public double elapsedTime;

        public void Execute(ref MaterialOverrideOffset materialOverrideOffset, in Enemy enemy, in DynamicBuffer<SpriteFrameElement> spriteFrames)
        {
            int frameIndex = (int)(elapsedTime * 20) % spriteFrames.Length;
            var frame = spriteFrames[frameIndex];
            materialOverrideOffset = new MaterialOverrideOffset
            {
                Offset = frame.offset,
                Scale = materialOverrideOffset.Scale
            };

        }
    }
}