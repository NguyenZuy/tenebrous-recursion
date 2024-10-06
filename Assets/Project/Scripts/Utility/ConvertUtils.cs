using Unity.Burst;
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
    }
}
