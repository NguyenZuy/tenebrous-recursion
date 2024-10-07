using Unity.Entities;
using Unity.Mathematics;

namespace Zuy.TenebrousRecursion.Component
{
    public struct Cell : IComponentData
    {
        // float2 minCorner = center - halfSize;
        // float2 maxCorner = center + halfSize;

        // ulong pointMorton = PointToMorton(point);
        // ulong minMorton = PointToMorton(minCorner);
        // ulong maxMorton = PointToMorton(maxCorner);

        public int minMorton;
        public int maxMorton;

        public float lastTimeToExecuteJob;
    }

    public struct CellMember : IBufferElementData
    {
        public Entity memberEntity;
    }
}
