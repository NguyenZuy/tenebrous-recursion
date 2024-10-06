using Unity.Entities;
using UnityEngine;
using Zuy.TenebrousRecursion.Component;
using Zuy.TenebrousRecursion.Utility;

namespace Zuy.TenebrousRecursion.Authoring
{
    class CellAuthoring : MonoBehaviour
    {
        public Vector2 size;

        private void OnDrawGizmos()
        {
            // Set the color for the Gizmo
            Gizmos.color = Color.cyan;

            // Calculate the center and size
            Vector3 center = transform.position;
            Vector3 size3D = new Vector3(size.x, size.y, 0);

            // Draw a wire cube (which will appear as a rectangle in 2D)
            Gizmos.DrawWireCube(center, size3D);
        }

        class Baker : Baker<CellAuthoring>
        {
            public override void Bake(CellAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);

                Vector2 halfSize = authoring.size / 2f;
                Vector2 center = ConvertUtils.V3ToV2(authoring.transform.position);

                Vector2 minCorner = center - halfSize;
                Vector2 maxCorner = center + halfSize;

                AddComponent(entity, new Cell()
                {
                    minMorton = MortonUtils.PosToMorton(minCorner),
                    maxMorton = MortonUtils.PosToMorton(maxCorner)
                });

                AddBuffer<CellMember>(entity);
            }
        }
    }
}
