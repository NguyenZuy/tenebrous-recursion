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
        private EntityQuery _spawnManagerQuery;
        private EntityQuery _nightmareQuery;

        protected override void OnCreate()
        {
            _spawnManagerQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<SpawnEnemyManager>()
                .Build(this);

            _nightmareQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<GateConfig, WaveConfig, GateSpawnPositionConfig>()
                .Build(this);

            var enenmyArchetype = EntityManager.CreateArchetype(
                typeof(LocalTransform),
                typeof(LocalToWorld),
                typeof(Enemy),
                typeof(InstanceRendererData)
            );

            var gateArchetype = EntityManager.CreateArchetype(
                typeof(Gate)
            );

            SpawnFactory.Init(enenmyArchetype);
        }

        protected override void OnUpdate()
        {
            var spawnManager = _spawnManagerQuery.GetSingletonRW<SpawnEnemyManager>().ValueRW;
            var nightmare = _nightmareQuery.GetSingletonRW<Nightmare>().ValueRW;

            float deltaTime = SystemAPI.Time.DeltaTime;
            Unity.Mathematics.Random random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 999999));

            if (nightmare.isInitialized)
                nightmare.elapsedTime += deltaTime;

            // Init in start of nightmare
            if (!nightmare.isInitialized)
            {
                nightmare.isInitialized = true;

                var gateBuffer = _nightmareQuery.GetSingletonBuffer<GateConfig>(true);
                var gateSpawnPosBuffer = _nightmareQuery.GetSingletonBuffer<GateSpawnPositionConfig>();

                Utils.FilterGateWillSpawn(gateBuffer, nightmare.elapsedTime, out NativeArray<GateConfig> gatesWillSpawn);
                Utils.FilterGateSpawnPosNotUsed(ref gateSpawnPosBuffer, gatesWillSpawn.Length, random, out NativeArray<GateSpawnPositionConfig> gateSpawnPos);

                SpawnFactory.SpawnGate(EntityManager, gatesWillSpawn, gateSpawnPos);
                return;
            }

            var waveBuffer = _nightmareQuery.GetSingletonBuffer<WaveConfig>(true);
            var nextWave = waveBuffer[nightmare.GetNextWaveIndex()];
            if (Utils.IsReachTime(nextWave.timeToAppear, nightmare.elapsedTime))
            {

            }

            // var waveBuffer = _waveManagerQuery.GetSingletonBuffer<Wave>(true);

            // if (spawnManager.ValueRO.isSpawn)
            // {
            //     int nextWaveIndex = waveManager.GetNextWaveIndex();
            //     if (nextWaveIndex < waveBuffer.Length)
            //     {
            //         Wave nextWave = waveBuffer[nextWaveIndex];
            //         CreateWave(nextWave);
            //     }

            //     spawnManager.ValueRW.isSpawn = false;
            // }
        }

        // private void CreateWave(Wave wave)
        // {
        //     if (wave.id1 != 0)
        //         SendCreate(wave.id1, wave.quantity1);

        //     if (wave.id2 != 0)
        //         SendCreate(wave.id2, wave.quantity2);

        //     if (wave.id3 != 0)
        //         SendCreate(wave.id3, wave.quantity3);
        // }

        // private void SendCreate(int type, int quantity)
        // {
        //     // Use the cached random instance
        //     SpawnFactory.Create(EntityManager, _enemyArchetype, type, quantity);
        // }

        [BurstCompile]
        public static class Utils
        {
            [BurstCompile]
            public static bool IsReachTime(in float time, in double elapsedTime)
            {
                return time >= elapsedTime;
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