using Unity.Entities;

namespace Zuy.TenebrousRecursion.Component
{
    public struct Enemy : IComponentData
    {
        public uint mortonCode;
        public Entity curCell;
    }
}
