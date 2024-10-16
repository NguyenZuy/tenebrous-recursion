using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace Zuy.TenebrousRecursion.Component
{
    public struct Enemy : IComponentData
    {
        public uint mortonCode;
        public Entity curCell;
        public float moveSpeed;
    }

    [MaterialProperty("_OffsetXYScaleZW")]
    public struct MaterialOverrideOffset : IComponentData
    {
        public float2 Offset;
        public float2 Scale;
    }

    public struct SpriteFrameElement : IBufferElementData
    {
        public float2 offset;
        public bool isLast;
    }
}
