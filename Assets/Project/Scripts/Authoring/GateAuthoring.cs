using Unity.Entities;
using UnityEngine;
using Zuy.TenebrousRecursion.Component;

namespace Zuy.TenebrousRecursion.Authoring
{
    class GateAuthoring : MonoBehaviour
    {
        class Baker : Baker<GateAuthoring>
        {
            public override void Bake(GateAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);

                // AddComponent(entity, new Gate()
                // {

                // });
            }
        }
    }

}
