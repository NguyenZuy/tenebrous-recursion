using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Zuy.TenebrousRecursion.Constant
{
    public static class Direction
    {
        // Cardinal directions
        public static readonly int2 NORTH = new int2(0, 1);
        public static readonly int2 SOUTH = new int2(0, -1);
        public static readonly int2 EAST = new int2(1, 0);
        public static readonly int2 WEST = new int2(-1, 0);

        // Diagonal directions
        public static readonly int2 NORTH_EAST = new int2(1, 1);
        public static readonly int2 NORTH_WEST = new int2(-1, 1);
        public static readonly int2 SOUTH_EAST = new int2(1, -1);
        public static readonly int2 SOUTH_WEST = new int2(-1, -1);

        // Zero direction (no movement)
        public static readonly int2 NONE = new int2(0, 0);

        public static int2[] AllDirections()
        {
            return new int2[]
            {
                NORTH,
                SOUTH,
                EAST,
                WEST,

                NORTH_EAST,
                SOUTH_EAST,
                SOUTH_WEST,
                NORTH_WEST
            };
        }
    }
}
