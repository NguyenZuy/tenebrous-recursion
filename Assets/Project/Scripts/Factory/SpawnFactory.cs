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
    public static class SpawnFactory
    {
        private static EntityArchetype _enemyArchetype;
        private static EntityArchetype _gateArchetype;

        public static void Init(in EntityArchetype enemyArchetype)
        {
            _enemyArchetype = enemyArchetype;
        }

        public static void SpawnGate(in EntityManager entityManager, in NativeArray<GateConfig> gatesWillSpawn, in NativeArray<GateSpawnPositionConfig> spawnPos)
        {
            NativeArray<Entity> entities = entityManager.CreateEntity(_gateArchetype, gatesWillSpawn.Length, Allocator.Temp);

            for (int i = 0; i < gatesWillSpawn.Length; i++)
            {
                entityManager.SetComponentData(entities[i], new Gate()
                {
                    level = gatesWillSpawn[i].level,
                    localPos = spawnPos[i].value
                });
            };
        }

        public static void SpawnWave(in EntityManager entityManager, in WaveConfig waveConfig)
        {
            NativeArray<Entity> entities = entityManager.CreateEntity(_gateArchetype, waveConfig.TotalEnemy(), Allocator.Temp);

            int i = 0;

            for (; i < waveConfig.number1; i++)
            {

            }
        }

        public static void StartNextWave()
        {

        }

        //     public static void Create(in EntityManager entityManager, in EntityArchetype archetype, in int type, in int number)
        //     {
        //         NativeArray<Entity> entities = entityManager.CreateEntity(archetype, 1023, Allocator.Temp);
        //         var enemyConfigsDict = ConfigManager.Instance.enemyConfigsDict;
        //         var enemyConfig = enemyConfigsDict[type];

        //         foreach (var entity in entities)
        //         {
        //             entityManager.SetComponentData(entity, new Enemy()
        //             {
        //                 moveSpeed = enemyConfig.moveSpeed
        //             });

        //             entityManager.SetSharedComponentManaged(entity, new InstanceRendererData()
        //             {
        //                 CastShadows = ShadowCastingMode.Off,
        //                 Material = enemyConfig.material,
        //                 Mesh = enemyConfig.mesh,
        //                 ReceiveShadows = false,
        //                 SubMesh = 0,
        //                 CullDistance = enemyConfig.cullingDistance,
        //             });

        //             float3 newPos = random.NextFloat3(new float3(0, 0, -1), new float3(10, 10, -1));
        //             entityManager.SetComponentData(entity, new LocalTransform()
        //             {
        //                 Position = newPos,
        //                 Scale = 1
        //             });
        //             random.NextUInt();  // This updates the state internally

        //             var texelSize = ConfigManager.Instance.textureSheet.texelSize;

        //             entityManager.AddComponentData(entity, new MaterialOverrideOffset
        //             {
        //                 Offset = ConfigManager.Instance.spriteFrames.Length > 0 // offset
        //                 ? ConfigManager.Instance.spriteFrames[0].rect.position * texelSize
        //                 : float2.zero,
        //                 Scale = new float2(0.1f, 0.2f) // scale
        //             });

        //             var frameElements = entityManager.AddBuffer<SpriteFrameElement>(entity);

        //             for (int i = 0; i < ConfigManager.Instance.spriteFrames.Length; i++)
        //             {
        //                 bool isLast = i == ConfigManager.Instance.spriteFrames.Length - 1;
        //                 frameElements.Add(new SpriteFrameElement
        //                 {
        //                     offset = ConfigManager.Instance.spriteFrames[i].rect.position * texelSize,
        //                     isLast = isLast
        //                 });
        //             }
        //         }
        //         entities.Dispose();
        //     }

        //     public static void Create(in EntityManager entityManager, in EntityArchetype archetype, in Random random, in int number, in int type)
        //     {
        //         NativeArray<Entity> entities = entityManager.CreateEntity(archetype, 1023, Allocator.Temp);
        //         var enemyConfigsDict = ConfigManager.Instance.enemyConfigsDict;
        //         var enemyConfig = enemyConfigsDict[type];

        //         foreach (var entity in entities)
        //         {
        //             entityManager.SetComponentData(entity, new Enemy()
        //             {
        //                 moveSpeed = enemyConfig.moveSpeed
        //             });

        //             entityManager.SetSharedComponentManaged(entity, new InstanceRendererData()
        //             {
        //                 CastShadows = ShadowCastingMode.Off,
        //                 Material = enemyConfig.material,
        //                 Mesh = enemyConfig.mesh,
        //                 ReceiveShadows = false,
        //                 SubMesh = 0,
        //                 CullDistance = enemyConfig.cullingDistance,
        //             });

        //             float3 newPos = random.NextFloat3(new float3(0, 0, -1), new float3(10, 10, -1));
        //             entityManager.SetComponentData(entity, new LocalTransform()
        //             {
        //                 Position = newPos,
        //                 Scale = 1
        //             });
        //             random.NextUInt();  // This updates the state internally

        //             var texelSize = ConfigManager.Instance.textureSheet.texelSize;

        //             entityManager.AddComponentData(entity, new MaterialOverrideOffset
        //             {
        //                 Offset = ConfigManager.Instance.spriteFrames.Length > 0 // offset
        //                 ? ConfigManager.Instance.spriteFrames[0].rect.position * texelSize
        //                 : float2.zero,
        //                 Scale = new float2(0.1f, 0.2f) // scale
        //             });

        //             var frameElements = entityManager.AddBuffer<SpriteFrameElement>(entity);

        //             for (int i = 0; i < ConfigManager.Instance.spriteFrames.Length; i++)
        //             {
        //                 bool isLast = i == ConfigManager.Instance.spriteFrames.Length - 1;
        //                 frameElements.Add(new SpriteFrameElement
        //                 {
        //                     offset = ConfigManager.Instance.spriteFrames[i].rect.position * texelSize,
        //                     isLast = isLast
        //                 });
        //             }
        //         }
        //         entities.Dispose();
        //     }
        // }
    }
}
