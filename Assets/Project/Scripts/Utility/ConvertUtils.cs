using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Zuy.TenebrousRecursion.Utility
{
    [BurstCompile]
    public static class ConvertUtils
    {
        [BurstCompile]
        public static void F3ToF2(in float3 value, out float2 result)
        {
            result = new float2(value.x, value.y);
        }

        public static Vector2 V3ToV2(Vector3 value)
        {
            return new Vector2(value.x, value.y);
        }

        // Fisher-Yates Shuffle
        [BurstCompile]
        public static void Shuffle<T>(ref DynamicBuffer<T> buffer, in Unity.Mathematics.Random random) where T : unmanaged
        {
            int n = buffer.Length;

            for (int i = n - 1; i > 0; i--)
            {
                // Pick a random index from 0 to i
                int j = random.NextInt(i + 1);

                // Swap array[i] with array[j]
                var temp = buffer[i];
                buffer[i] = buffer[j];
                buffer[j] = temp;
            }
        }
    }
}
