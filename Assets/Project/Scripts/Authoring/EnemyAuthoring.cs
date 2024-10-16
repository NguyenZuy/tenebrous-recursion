using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using Zuy.TenebrousRecursion.Component;
using Zuy.TenebrousRecursion.Utility;

namespace Zuy.TenebrousRecursion.Authoring
{
    class EnemyAuthoring : MonoBehaviour
    {
        public float gridPixelSize;
        public Texture2D textureSheet;
        public Sprite[] spriteFrames;
        class Baker : Baker<EnemyAuthoring>
        {
            public override void Bake(EnemyAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

                AddComponent(entity, new Enemy()
                {
                    //moveSpeed = 2f
                });

                var texelSize = authoring.textureSheet.texelSize;

                AddComponent(entity, new MaterialOverrideOffset
                {
                    Offset = authoring.spriteFrames.Length > 0 // offset
                    ? authoring.spriteFrames[0].rect.position * texelSize
                    : float2.zero,
                    Scale = new float2(texelSize * authoring.gridPixelSize) // scale
                });

                var frameElements = AddBuffer<SpriteFrameElement>(entity);

                for (int i = 0; i < authoring.spriteFrames.Length; i++)
                {
                    bool isLast = i == authoring.spriteFrames.Length - 1;
                    frameElements.Add(new SpriteFrameElement
                    {
                        offset = authoring.spriteFrames[i].rect.position * texelSize,
                        isLast = isLast
                    });
                }
            }
        }
    }

    public struct SpriteFrameElement : IBufferElementData
    {
        public float2 offset;
        public bool isLast;
    }
}
