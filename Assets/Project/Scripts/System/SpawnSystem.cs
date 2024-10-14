using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Zuy.TenebrousRecursion.Component;
using Zuy.TenebrousRecursion.Factory;

namespace Zuy.TenebrousRecursion.System
{
    [RequireMatchingQueriesForUpdate]
    partial class SpawnSystem : SystemBase
    {
        private EntityQuery _configManagerQuery;
        private EntityQuery _spawnManagerQuery;

        private EntityArchetype _archetype;

        protected override void OnCreate()
        {
            _configManagerQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<EnemyConfigElement>()
                .Build(this);

            _spawnManagerQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<SpawnEnemyManager>()
                .Build(this);

            _archetype = EntityManager.CreateArchetype(typeof(LocalTransform), typeof(LocalToWorld), typeof(Enemy), typeof(InstanceRendererData));
        }

        protected override void OnUpdate()
        {
            var configManager = _configManagerQuery.GetSingletonBuffer<EnemyConfigElement>(true);
            var spawnManager = _spawnManagerQuery.GetSingletonRW<SpawnEnemyManager>();

            if (spawnManager.ValueRO.isSpawn)
            {
                spawnManager.ValueRW.isSpawn = false;

                var random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(0, 9999999));
                //SpawnFactory.Create(EntityManager, new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(0, 9999999)), configManager[0].sampleEntity, 232);
                SpawnFactory.Create(EntityManager, _archetype, ref random, 21323, 3232);
            }
        }
    }
}