using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Zuy.TenebrousRecursion.Component;

namespace Zuy.TenebrousRecursion.System
{
    [RequireMatchingQueriesForUpdate]
    partial class SpawnSystem : SystemBase
    {
        private EntityQuery _spawnEnemyManagerQuery;

        protected override void OnCreate()
        {
            _spawnEnemyManagerQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<SpawnEnemyManager>()
                .Build(this);
        }

        protected override void OnUpdate()
        {
            var spawnEnemyManager = _spawnEnemyManagerQuery.GetSingleton<SpawnEnemyManager>();

            if (spawnEnemyManager.isSpawn)
            {
                spawnEnemyManager.isSpawn = false;
            }
        }
    }
}
