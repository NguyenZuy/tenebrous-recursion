using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Zuy.TenebrousRecursion.Component;
using Zuy.TenebrousRecursion.Job;

namespace Zuy.TenebrousRecursion.System
{
    [BurstCompile]
    [UpdateBefore(typeof(GridSystem))]
    [RequireMatchingQueriesForUpdate]
    partial struct EnemySystem : ISystem
    {
        private EntityQuery _gridQuery;
        private EntityQuery _cellQuery;
        private EntityQuery _playerQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _gridQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Grid>()
                .Build(ref state);

            _cellQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Cell>()
                .Build(ref state);

            _playerQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Player>()
                .WithAllRW<LocalTransform>()
                .Build(ref state);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var grid = _gridQuery.GetSingleton<Grid>();

            NativeArray<Cell> cells = _cellQuery.ToComponentDataArray<Cell>(Allocator.TempJob);
            NativeArray<Entity> cellEntities = _cellQuery.ToEntityArray(Allocator.TempJob);

            var encodeMortonJob = new EncodeMortonForEnemyJob()
            {
                cellSize = grid.cellDiameter
            };
            state.Dependency = encodeMortonJob.ScheduleParallel(state.Dependency);

            var getCurCellJob = new GetCurCellForEnemyJob()
            {
                cells = cells,
                cellEntities = cellEntities
            };
            state.Dependency = getCurCellJob.ScheduleParallel(state.Dependency);

            var animatedJob = new SpriteAnimatedJob()
            {
                elapsedTime = SystemAPI.Time.ElapsedTime
            };
            state.Dependency = animatedJob.ScheduleParallel(state.Dependency);

            var player = _playerQuery.GetSingleton<LocalTransform>();

            var movementJob = new MovementJob()
            {
                deltaTime = SystemAPI.Time.DeltaTime,
                playerPos = player.Position
            };
            state.Dependency = movementJob.ScheduleParallel(state.Dependency);

            state.CompleteDependency();
        }
    }
}
