using Unity.Entities;
using UnityEngine;
using Zuy.TenebrousRecursion.Component;

namespace Zuy.TenebrousRecursion.Authoring
{
    class ConfigManagerAuthoring : MonoBehaviour
    {
        public GameObject enemySample1;

        class Baker : Baker<ConfigManagerAuthoring>
        {
            public override void Bake(ConfigManagerAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);

                var enemyConfigBuffer = AddBuffer<EnemyConfigElement>(entity);
                enemyConfigBuffer.Add(new EnemyConfigElement()
                {
                    type = 0,
                    sampleEntity = GetEntity(authoring.enemySample1, TransformUsageFlags.Dynamic),
                });
            }
        }
    }
}
