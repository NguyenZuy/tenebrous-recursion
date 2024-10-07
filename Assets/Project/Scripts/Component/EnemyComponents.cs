using Unity.Entities;

namespace Zuy.TenebrousRecursion.Component
{
    public struct Enemy : IComponentData
    {
        public int morton;
        public Entity curCell;
    }
}
