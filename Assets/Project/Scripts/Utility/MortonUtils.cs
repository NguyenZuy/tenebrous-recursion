using System.Xml;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using Zuy.TenebrousRecursion.Component;

namespace Zuy.TenebrousRecursion.Utility
{
    [BurstCompile]
    public static class MortonUtils
    {
        [BurstCompile]
        public static uint Encode(in float2 value, in float cellSize)
        {
            int2 roundedValue = (int2)math.floor(value / cellSize);

            return (uint)((Part1By1(roundedValue.y) << 1) + Part1By1(roundedValue.x));
        }

        [BurstCompile]
        private static uint Part1By1(int n)
        {
            n &= 0x0000ffff;
            n = (n ^ (n << 8)) & 0x00ff00ff;
            n = (n ^ (n << 4)) & 0x0f0f0f0f;
            n = (n ^ (n << 2)) & 0x33333333;
            n = (n ^ (n << 1)) & 0x55555555;
            return (uint)n;
        }

        public static (int x, int y) Decode(uint code)
        {
            return (Compact1By1(code), Compact1By1(code >> 1));
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

        [BurstCompile]
        public static bool IsInsideCell(in uint mortonCheck, in Cell cell)
        {
            return mortonCheck == cell.mortonCode;
        }
    }
}
