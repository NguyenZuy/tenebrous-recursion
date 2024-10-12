using Unity.Entities;

namespace Zuy.TenebrousRecursion.Component
{
    public struct SpawnEnemyManager : IComponentData
    {
        public bool isSpawn;
        public int number;
        public int type;
    }

    public struct PooledTag : IComponentData { }
}
