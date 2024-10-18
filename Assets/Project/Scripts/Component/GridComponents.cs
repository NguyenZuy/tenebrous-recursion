using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Zuy.TenebrousRecursion.Component
{
    public struct Grid : IComponentData
    {
        public float cellSize;
    }

    public struct Cell : IComponentData
    {
        // float2 minCorner = center - halfSize;
        // float2 maxCorner = center + halfSize;

        // ulong pointMorton = PointToMorton(point);
        // ulong minMorton = PointToMorton(minCorner);
        // ulong maxMorton = PointToMorton(maxCorner);

        // public uint minMorton;
        // public uint maxMorton;

        public uint mortonCode;

        public float lastTimeToExecuteJob;
    }

    public struct CellMember : IBufferElementData
    {
        public Entity memberEntity;
    }

    public struct Test : IComponentData
    {
        public Test1 test1;
    }

    public struct Test1
    {
        NativeArray<Entity> entities;
    }
}
