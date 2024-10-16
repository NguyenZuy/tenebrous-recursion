using Unity.Entities;

namespace Zuy.TenebrousRecursion.Component
{
    public struct SpawnEnemyManager : IComponentData
    {
        public bool isInitialize; // Mark to start the episode
    }

    public struct PooledTag : IComponentData { }
}
