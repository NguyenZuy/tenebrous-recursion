using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Zuy.TenebrousRecursion.Component;
using Zuy.TenebrousRecursion.Utility;

namespace Zuy.TenebrousRecursion.Authoring
{
    class CellAuthoring : MonoBehaviour
    {
        class Baker : Baker<CellAuthoring>
        {
            public override void Bake(CellAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);

                ConvertUtils.F3ToF2(authoring.transform.position, out float2 pos);

                AddComponent(entity, new Cell()
                {
                    mortonCode = MortonUtils.Encode(pos, authoring.transform.parent.GetComponent<GridAuthoring>().cellSize)
                });

                AddBuffer<CellMember>(entity);
            }
        }
    }
}
