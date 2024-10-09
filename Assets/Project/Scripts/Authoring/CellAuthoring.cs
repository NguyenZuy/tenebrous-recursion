using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Zuy.TenebrousRecursion.Component;
using Zuy.TenebrousRecursion.Utility;

namespace Zuy.TenebrousRecursion.Authoring
{
    class CellAuthoring : MonoBehaviour
    {
        // public ulong mortonCode;


        // void Start()
        // {
        //     Vector3 localPosition = transform.position;
        //     int gridX = Mathf.FloorToInt(localPosition.x / transform.parent.GetComponent<GridAuthoring>().cellSize);
        //     int gridY = Mathf.FloorToInt(localPosition.y / transform.parent.GetComponent<GridAuthoring>().cellSize);
        //     mortonCode = MortonCode.Encode(gridX, gridY);
        // }

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
