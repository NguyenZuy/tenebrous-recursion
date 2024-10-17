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

        private const float SPAWN_RADIUS_OF_GATE = 4f;

        public static void Init(in EntityArchetype enemyArchetype, in EntityArchetype gateArchetype)
        {
            _enemyArchetype = enemyArchetype;
            _gateArchetype = gateArchetype;
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

        public static void SpawnWave(in EntityManager entityManager, in WaveConfig waveConfig, in NativeArray<Gate> gates, in Random random)
        {
            NativeArray<Entity> entities = entityManager.CreateEntity(_enemyArchetype, waveConfig.TotalEnemy(), Allocator.Temp);

            int i = 0;
            CreateEnemies(ref i, waveConfig.id1, waveConfig.number1, entityManager, entities, gates, random);
            CreateEnemies(ref i, waveConfig.id2, waveConfig.number2, entityManager, entities, gates, random);
            CreateEnemies(ref i, waveConfig.id3, waveConfig.number3, entityManager, entities, gates, random);

            entities.Dispose();
            gates.Dispose();

            UnityEngine.Debug.Log("Success!");
        }

        static void CreateEnemies(ref int i, in int type, in int quantity, in EntityManager entityManager, in NativeArray<Entity> entities, in NativeArray<Gate> gates, in Random random)
        {
            ScriptableObject.EnemySO enemyConfig = ConfigManager.Instance.GetEnemeyConfigByType(type);
            if (enemyConfig == null)
                return;

            Entity entity;

            for (; i < quantity; i++)
            {
                entity = entities[i];
                CreateEnemy(entityManager, entity, random, gates, enemyConfig);
            }
        }

        static void CreateEnemy(in EntityManager entityManager, in Entity entity, in Random random, in NativeArray<Gate> gates, in ScriptableObject.EnemySO enemyConfig)
        {
            int indexGate = random.NextInt(0, gates.Length);
            Gate gate = gates[indexGate];

            float3 minBound = new float3(gate.localPos.x - SPAWN_RADIUS_OF_GATE, gate.localPos.y, gate.localPos.y - SPAWN_RADIUS_OF_GATE);
            float3 maxBound = new float3(gate.localPos.x + SPAWN_RADIUS_OF_GATE, gate.localPos.y, gate.localPos.y + SPAWN_RADIUS_OF_GATE);

            float3 newPos = random.NextFloat3(minBound, maxBound);

            entityManager.SetComponentData(entity, new LocalTransform()
            {
                Position = newPos,
                Scale = 1
            });

            entityManager.SetComponentData(entity, new Enemy()
            {
                moveSpeed = enemyConfig.moveSpeed
            });

            entityManager.SetSharedComponentManaged(entity, new InstanceRendererData()
            {
                CastShadows = ShadowCastingMode.Off,
                Material = enemyConfig.material,
                Mesh = enemyConfig.mesh,
                ReceiveShadows = false,
                SubMesh = 0,
                CullDistance = enemyConfig.cullingDistance,
            });

            var texelSize = enemyConfig.gridPixelSize;

            entityManager.SetComponentData(entity, new MaterialOverrideOffset
            {
                Offset = enemyConfig.spriteFrames.Length > 0 // offset
                    ? enemyConfig.spriteFrames[0].rect.position * texelSize
                    : float2.zero,
                Scale = new float2(0.1f, 0.2f) // scale
            });

            var frameElements = entityManager.GetBuffer<SpriteFrameElement>(entity);

            for (int j = 0; j < enemyConfig.spriteFrames.Length; j++)
            {
                bool isLast = j == enemyConfig.spriteFrames.Length - 1;
                frameElements.Add(new SpriteFrameElement
                {
                    offset = enemyConfig.spriteFrames[j].rect.position * texelSize,
                    isLast = isLast
                });
            }
        }
    }
}
