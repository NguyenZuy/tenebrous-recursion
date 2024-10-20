using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Zuy.TenebrousRecursion.Component;

namespace Zuy.TenebrousRecursion.Utility
{
    [BurstCompile]
    public static class GridUtils
    {
        [BurstCompile]
        public static void GetGridIndexByPos(in float2 pos, in float cellSize, out int2 result)
        {
            result = (int2)math.floor(pos / cellSize);
        }

        [BurstCompile]
        public static void GetGridIndexByPos(in float3 pos, in float cellSize, out int2 result)
        {
            ConvertUtils.F3ToF2(pos, out float2 f2Pos);
            result = (int2)math.floor(f2Pos / cellSize);
        }

        [BurstCompile]
        public static void GetCellByGridIndex(in NativeArray<Cell> cells, in int2 gridIndex, out Cell result)
        {
            result = cells[0];

            foreach (var cell in cells)
            {
                if (cell.gridIndex.Equals(gridIndex))
                {
                    result = cell;
                    return;
                }
            }
        }
    }
}
