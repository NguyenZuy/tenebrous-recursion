using Unity.Entities;
using UnityEngine;
using Zuy.TenebrousRecursion.Component;

namespace Zuy.TenebrousRecursion.Authoring
{
    class SpawnManagerAuthoring : MonoBehaviour
    {
        class Baker : Baker<SpawnManagerAuthoring>
        {
            public override void Bake(SpawnManagerAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);

                AddComponent(entity, new SpawnEnemyManager()
                {
                    isSpawn = true
                });
            }
        }
    }
}
