using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace Zuy.TenebrousRecursion.Utility
{
    [BurstCompile]
    public static class MortonUtils
    {
        [BurstCompile]
        public static uint PosToMorton(in float2 pos)
        {
            // Convert float coordinates to integers
            int2 intPosition = (int2)math.clamp(pos * 65535, 0, 65535);
            return (uint)(InterleaveZero(intPosition.x) | (InterleaveZero(intPosition.y) << 1));
        }

        [BurstCompile]
        private static uint InterleaveZero(int value)
        {
            uint x = (uint)value;
            x &= 0x0000ffff;                  // Keep only the lower 16 bits
            x = (x | (x << 8)) & 0x00FF00FF;  // Interleave zeros
            x = (x | (x << 4)) & 0x0F0F0F0F;  // Interleave zeros
            x = (x | (x << 2)) & 0x33333333;  // Interleave zeros
            x = (x | (x << 1)) & 0x55555555;  // Interleave zeros
            return x;
        }

        // Method to convert Morton code back to float2
        [BurstCompile]
        public static void MortonToPos(uint mortonCode, out float2 result)
        {
            int x = DeinterleaveZero(mortonCode);
            int y = DeinterleaveZero(mortonCode >> 1);

            result = new float2(x, y) / 65535;
        }

        [BurstCompile]
        private static int DeinterleaveZero(uint morton)
        {
            morton &= 0x55555555;
            morton = (morton | (morton >> 1)) & 0x33333333;
            morton = (morton | (morton >> 2)) & 0x0F0F0F0F;
            morton = (morton | (morton >> 4)) & 0x00FF00FF;
            morton = (morton | (morton >> 8)) & 0x0000FFFF;
            return (int)morton;
        }
    }
}
