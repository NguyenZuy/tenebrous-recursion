using System;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Zuy.TenebrousRecursion.Component;
using Zuy.TenebrousRecursion.Factory;
using Zuy.TenebrousRecursion.Utility;

namespace Zuy.TenebrousRecursion.System
{
    [RequireMatchingQueriesForUpdate]
    partial class SpawnSystem : SystemBase
    {
        private EntityQuery _nightmareQuery;
        private EntityQuery _gateQuery;

        protected override void OnCreate()
        {
            _nightmareQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<Nightmare>()
                .WithAll<GateConfig, WaveConfig, GateSpawnPositionConfig>()
                .Build(this);

            _gateQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Gate>()
                .Build(this);

            var enenmyArchetype = EntityManager.CreateArchetype(
                typeof(LocalTransform),
                typeof(LocalToWorld),
                typeof(Enemy),
                typeof(InstanceRendererData),
                typeof(MaterialOverrideOffset),
                typeof(SpriteFrameElement)
            );

            var gateArchetype = EntityManager.CreateArchetype(
                typeof(Gate)
            );

            SpawnFactory.Init(enenmyArchetype, gateArchetype);
        }

        protected override void OnUpdate()
        {
            var nightmare = _nightmareQuery.GetSingletonRW<Nightmare>().ValueRW;

            float deltaTime = SystemAPI.Time.DeltaTime;
            Unity.Mathematics.Random random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 999999));

            if (nightmare.isInitialized)
            {
                nightmare.elapsedTime += deltaTime;
                _nightmareQuery.SetSingleton(nightmare);
            }

            // Init in start of nightmare
            if (!nightmare.isInitialized)
            {
                nightmare.isInitialized = true;
                _nightmareQuery.SetSingleton(nightmare);

                var gateBuffer = _nightmareQuery.GetSingletonBuffer<GateConfig>(true);
                var gateSpawnPosBuffer = _nightmareQuery.GetSingletonBuffer<GateSpawnPositionConfig>();

                Utils.FilterGateWillSpawn(gateBuffer, nightmare.elapsedTime, out NativeArray<GateConfig> gatesWillSpawn);
                Utils.FilterGateSpawnPosNotUsed(ref gateSpawnPosBuffer, gatesWillSpawn.Length, random, out NativeArray<GateSpawnPositionConfig> gateSpawnPos);

                SpawnFactory.SpawnGate(EntityManager, gatesWillSpawn, gateSpawnPos);
                return;
            }

            var gates = _gateQuery.ToComponentDataArray<Gate>(Allocator.Temp);

            var waveBuffer = _nightmareQuery.GetSingletonBuffer<WaveConfig>(false);
            var nextWaveIndex = nightmare.GetNextWaveIndex();
            var nextWave = waveBuffer[nextWaveIndex];
            if (!nextWave.isSpawned && Utils.IsReachTime(nextWave.timeToAppear, nightmare.elapsedTime))
            {
                nextWave.isSpawned = true;
                waveBuffer[nextWaveIndex] = nextWave;
                SpawnFactory.SpawnWave(EntityManager, nextWave, gates, ref random);
            }
        }

        [BurstCompile]
        public static class Utils
        {
            [BurstCompile]
            public static bool IsReachTime(in float time, in double elapsedTime)
            {
                return time <= elapsedTime;
            }

            [BurstCompile]
            public static void FilterGateWillSpawn(in DynamicBuffer<GateConfig> buffer, in double elapsedTime, out NativeArray<GateConfig> gatesWillSpawn)
            {
                var raw = new NativeList<GateConfig>(Allocator.Temp);

                foreach (var element in buffer)
                {
                    if (IsReachTime(element.timeToAppear, elapsedTime))
                        raw.Add(element);
                }
                gatesWillSpawn = raw.ToArray(Allocator.Temp);

                raw.Dispose();
            }

            [BurstCompile]
            public static void FilterGateSpawnPosNotUsed(ref DynamicBuffer<GateSpawnPositionConfig> buffer, in int numberElementsToGet, in Unity.Mathematics.Random random, out NativeArray<GateSpawnPositionConfig> result)
            {
                result = new NativeArray<GateSpawnPositionConfig>(numberElementsToGet, Allocator.Temp);

                ConvertUtils.Shuffle(ref buffer, random);

                int index = 0;
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (!buffer[i].isUsed)
                    {
                        result[i] = buffer[i];
                        index++;
                    }

                    if (index == numberElementsToGet)
                        break;
                }
            }
        }
    }
}