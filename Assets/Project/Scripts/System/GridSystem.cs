using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Zuy.TenebrousRecursion.Component;
using Zuy.TenebrousRecursion.Job;

namespace Zuy.TenebrousRecursion.System
{
    [BurstCompile]
    [UpdateAfter(typeof(EnemySystem))]
    [RequireMatchingQueriesForUpdate]
    partial struct GridSystem : ISystem
    {
        private EntityQuery _enemyQuery;
        private EntityQuery _playerQuery;
        private EntityQuery _cellQuery1;
        private EntityQuery _cellQuery2;
        private EntityQuery _cellQuery3;

        private ComponentTypeHandle<Cell> _cellTypeHandle;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _enemyQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Enemy>()
                .Build(ref state);

            _playerQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Player>()
                .Build(ref state);

            _cellQuery1 = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Cell>()
                .Build(ref state);

            _cellQuery2 = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<Cell>()
                .Build(ref state);

            _cellQuery3 = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<Cell, CellMember>()
                .Build(ref state);

            _cellTypeHandle = state.GetComponentTypeHandle<Cell>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _cellTypeHandle.Update(ref state);

            float elapsedTime = (float)SystemAPI.Time.ElapsedTime;

            NativeArray<Enemy> enemies = _enemyQuery.ToComponentDataArray<Enemy>(Allocator.TempJob);
            NativeArray<Entity> enemyEntities = _enemyQuery.ToEntityArray(Allocator.TempJob);

            NativeArray<Entity> cellEntities = _cellQuery1.ToEntityArray(Allocator.TempJob);

            var player = _playerQuery.GetSingleton<Player>();
            int indexOfPlayerCurCell = Utils.GetCellIndexByEntity(player.curCell, cellEntities);
            Cell playerCurCell = SystemAPI.GetComponentRO<Cell>(cellEntities[indexOfPlayerCurCell]).ValueRO;

            NativeArray<Cell> allCells = _cellQuery1.ToComponentDataArray<Cell>(Allocator.TempJob);

            var getInsideAgentsJob = new GetInsideAgentsJob()
            {
                enemies = enemies,
                enemyEntities = enemyEntities,
                elapsedTime = elapsedTime
            };
            state.Dependency = getInsideAgentsJob.ScheduleParallel(_cellQuery3, state.Dependency);

            var calculateFFPIntegrationFieldJob = new CalculateFFPIntegrationFieldJob()
            {
                playerCurCell = playerCurCell,
                cellTypeHandle = _cellTypeHandle,
            };
            state.Dependency = calculateFFPIntegrationFieldJob.ScheduleParallel(_cellQuery2, state.Dependency);

            var calculateFFPFlowFieldJob = new CalculateFFPFlowFieldJob()
            {
                allCells = allCells,
                cellTypeHandle = _cellTypeHandle,
            };
            state.Dependency = calculateFFPFlowFieldJob.ScheduleParallel(_cellQuery2, state.Dependency);

            state.CompleteDependency();

            // enemies.Dispose();
            // enemyEntities.Dispose();
            // cellEntities.Dispose();
        }
    }

    [BurstCompile]
    static class Utils
    {
        [BurstCompile]
        public static int GetCellIndexByEntity(in Entity entityCheck, in NativeArray<Entity> entities)
        {
            int i = -1;
            foreach (var entity in entities)
            {
                i++;
                if (entity == entityCheck)
                    return i;
            }
            return i;
        }
    }
}
