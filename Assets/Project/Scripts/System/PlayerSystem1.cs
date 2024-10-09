using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Zuy.TenebrousRecursion.Component;
using Zuy.TenebrousRecursion.Job;

namespace Zuy.TenebrousRecursion.System
{
    [BurstCompile]
    [UpdateBefore(typeof(GridSystem))]
    [RequireMatchingQueriesForUpdate]
    partial struct PlayerSystem1 : ISystem
    {
        private EntityQuery _gridQuery;
        private EntityQuery _cellQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _gridQuery = new EntityQueryBuilder(Allocator.Temp)
                            .WithAll<Grid>()
                            .Build(ref state);

            _cellQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Cell>()
                .Build(ref state);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var grid = _gridQuery.GetSingleton<Grid>();

            NativeArray<Cell> cells = _cellQuery.ToComponentDataArray<Cell>(Allocator.TempJob);
            NativeArray<Entity> cellEntities = _cellQuery.ToEntityArray(Allocator.TempJob);

            var encodeMortonJob = new EncodeMortonForPlayerJob()
            {
                cellSize = grid.cellSize
            };
            state.Dependency = encodeMortonJob.ScheduleParallel(state.Dependency);

            var getCurCellJob = new GetCurCellForPlayerJob()
            {
                cells = cells,
                cellEntities = cellEntities
            };
            state.Dependency = getCurCellJob.ScheduleParallel(state.Dependency);

            state.CompleteDependency();
        }
    }
}
