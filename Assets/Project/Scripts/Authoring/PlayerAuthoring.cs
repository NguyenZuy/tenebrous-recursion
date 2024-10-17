using Unity.Entities;
using UnityEngine;
using Zuy.TenebrousRecursion.Component;

namespace Zuy.TenebrousRecursion.Authoring
{
    class PlayerAuthoring : MonoBehaviour
    {
        class Baker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

                AddComponent(entity, new Player()
                {
                    moveSpeed = 8f,
                });
            }
        }
    }
}
