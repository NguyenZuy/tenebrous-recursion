using Unity.Entities;

namespace Zuy.TenebrousRecursion.Component
{
    public struct Player : IComponentData
    {
        public float moveSpeed;
        public uint mortonCode;
        public Entity curCell;
    }
}