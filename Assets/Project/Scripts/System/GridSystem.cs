using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Zuy.TenebrousRecursion.Component;
using Zuy.TenebrousRecursion.Job;

namespace Zuy.TenebrousRecursion.System
{
    [BurstCompile]
    [UpdateAfter(typeof(EnemySystem))]
    partial struct GridSystem : ISystem
    {
        private EntityQuery _enemyQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _enemyQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Enemy>()
                .Build(ref state);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            NativeArray<Enemy> enemies = _enemyQuery.ToComponentDataArray<Enemy>(Allocator.TempJob);
            NativeArray<Entity> enemyEntities = _enemyQuery.ToEntityArray(Allocator.TempJob);

            var getInsideAgentsJob = new GetInsideAgentsJob()
            {
                enemies = enemies,
                enemyEntities = enemyEntities,
                elapsedTime = (float)SystemAPI.Time.ElapsedTime
            };

            state.Dependency = getInsideAgentsJob.ScheduleParallel(state.Dependency);
        }
    }
}
