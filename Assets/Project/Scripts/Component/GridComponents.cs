using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Zuy.TenebrousRecursion.Component
{
    public struct Grid : IComponentData
    {
        public int2 gridSize;
        public int cellDiameter;
    }

    public struct Cell : IComponentData
    {
        public uint mortonCode;
        public int2 gridIndex;


        [MarshalAs(UnmanagedType.U1)] public bool isImpassible;

        public int cost;
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
