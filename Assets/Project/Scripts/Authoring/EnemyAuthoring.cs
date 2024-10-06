using Unity.Entities;
using UnityEngine;
using Zuy.TenebrousRecursion.Component;

namespace Zuy.TenebrousRecursion.Authoring
{
    class EnemyAuthoring : MonoBehaviour
    {
        class Baker : Baker<EnemyAuthoring>
        {
            public override void Bake(EnemyAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

                AddComponent(entity, new Enemy()
                {

                });
            }
        }
    }
}
