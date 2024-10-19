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
        public uint mortonCode;
        public int2 gridIndex;

        public int bestCost; // Steps (unit is 1) to the target cell
        public int2 curDirection;

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
