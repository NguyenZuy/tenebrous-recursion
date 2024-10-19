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

                float cellSize = authoring.transform.parent.GetComponent<GridAuthoring>().cellDiameter;

                AddComponent(entity, new Cell()
                {
                    mortonCode = MortonUtils.Encode(pos, cellSize),
                    gridIndex = (int2)math.floor(pos / cellSize)
                });

                AddBuffer<CellMember>(entity);
            }
        }
    }
}
