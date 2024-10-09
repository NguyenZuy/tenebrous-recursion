using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Zuy.TenebrousRecursion.Component;
using Zuy.TenebrousRecursion.Hybrid;

namespace Zuy.TenebrousRecursion.System
{
    partial class PlayerSystem : SystemBase
    {
        private EntityQuery _playerQuery;

        protected override void OnCreate()
        {
            _playerQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<Player, LocalTransform>()
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
                localTransform.ValueRW.Position = localTransform.ValueRW.Position + (cachedInput * player.ValueRW.moveSpeed * SystemAPI.Time.DeltaTime);
            }
        }
    }
}