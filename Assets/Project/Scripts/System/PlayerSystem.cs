using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Zuy.TenebrousRecursion.Component;
using Zuy.TenebrousRecursion.Hybrid;
using Zuy.TenebrousRecursion.Mono;
using Zuy.TenebrousRecursion.Utility;

namespace Zuy.TenebrousRecursion.System
{
    partial class PlayerSystem : SystemBase
    {
        private EntityQuery _playerQuery;
        private EntityQuery _gridQuery;
        private EntityQuery _cellQuery;

        protected override void OnCreate()
        {
            _playerQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<Player, LocalTransform>()
                .Build(this);

            _gridQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Grid>()
                .Build(this);

            _cellQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Cell>()
                .Build(this);

            RequireForUpdate(_playerQuery);
        }

        protected override void OnUpdate()
        {
            var player = _playerQuery.GetSingletonRW<Player>();
            var localTransform = _playerQuery.GetSingletonRW<LocalTransform>();

            float3 cachedInput = PlayerInputBridge.Instance.CachedInput;

            if (!cachedInput.Equals(float3.zero))
            {
                var grid = _gridQuery.GetSingleton<Grid>();
                var cells = _cellQuery.ToComponentDataArray<Cell>(Allocator.Temp);

                float3 newPos = localTransform.ValueRW.Position + (cachedInput * player.ValueRW.moveSpeed * SystemAPI.Time.DeltaTime);
                GridUtils.GetGridIndexByPos(newPos, grid.cellDiameter, out int2 gridIndex);
                GridUtils.GetCellByGridIndex(cells, gridIndex, out Cell targetCell);

                if (!targetCell.isImpassible)
                    localTransform.ValueRW.Position = localTransform.ValueRW.Position + (cachedInput * player.ValueRW.moveSpeed * SystemAPI.Time.DeltaTime);
            }
        }
    }
}