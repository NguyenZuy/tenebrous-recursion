using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Rendering;
using Zuy.TenebrousRecursion.Component;
using Zuy.TenebrousRecursion.Mono;

namespace Zuy.TenebrousRecursion.Factory
{
    [BurstCompile]
    public static class SpawnFactory
    {
        public static void Create(in EntityManager entityManager, in EntityArchetype archetype, ref Random random, in int number, in int type)
        {
            NativeArray<Entity> entities = entityManager.CreateEntity(archetype, 1023, Allocator.Persistent);

            foreach (var entity in entities)
            {
                entityManager.SetSharedComponentManaged(entity, new InstanceRendererData()
                {
                    CastShadows = ShadowCastingMode.Off,
                    Material = ConfigManager.Instance.material,
                    Mesh = ConfigManager.Instance.mesh,
                    ReceiveShadows = false,
                    SubMesh = 0,
                    CullDistance = ConfigManager.Instance.cullingDistance,
                    //InstanceShaderPropertyId = Shader.PropertyToID("_FrameRange")
                });

                float3 newPos = random.NextFloat3(new float3(0, 0, -1), new float3(10, 10, -1));
                entityManager.SetComponentData(entity, new LocalTransform()
                {
                    Position = newPos,
                    Scale = 1
                });
                random.NextUInt();  // This updates the state internally

                var texelSize = ConfigManager.Instance.textureSheet.texelSize;

                entityManager.AddComponentData(entity, new MaterialOverrideOffset
                {
                    Offset = ConfigManager.Instance.spriteFrames.Length > 0 // offset
                    ? ConfigManager.Instance.spriteFrames[0].rect.position * texelSize
                    : float2.zero,
                    Scale = new float2(0.1f, 0.2f) // scale
                });

                var frameElements = entityManager.AddBuffer<SpriteFrameElement>(entity);

                for (int i = 0; i < ConfigManager.Instance.spriteFrames.Length; i++)
                {
                    bool isLast = i == ConfigManager.Instance.spriteFrames.Length - 1;
                    frameElements.Add(new SpriteFrameElement
                    {
                        offset = ConfigManager.Instance.spriteFrames[i].rect.position * texelSize,
                        isLast = isLast
                    });
                }
            }
            entities.Dispose();
        }
    }
}
