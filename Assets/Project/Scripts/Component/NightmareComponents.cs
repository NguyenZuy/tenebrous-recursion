using Unity.Entities;
using Unity.Mathematics;

namespace Zuy.TenebrousRecursion.Component
{
    public struct Nightmare : IComponentData
    {
        public bool isInitialized;
        public float elapsedTime;

        public int curWaveIndex; // Cur wave index in dynamicBuffer

        public int GetNextWaveIndex()
        {
            return curWaveIndex++;
        }
    }

    public struct GateConfig : IBufferElementData
    {
        public int level; // 1-D, 2-C, 3-B, 4-A, 5-S
        public float timeToAppear; // From time to start the nightmare to now
    }

    public struct WaveConfig : IBufferElementData
    {
        public bool isSpawned;

        public float timeToAppear; // From time to start the nightmare to now

        public int id1;
        public int number1;

        public int id2;
        public int number2;

        public int id3;
        public int number3;

        public int requireGateLevel; // 0-None, 1-D, 2-C, 3-B, 4-A, 5-S

        public int TotalEnemy()
        {
            return number1 + number2 + number3;
        }
    }

    public struct GateSpawnPositionConfig : IBufferElementData
    {
        public float3 value;
        public bool isUsed;
    }

    public struct Gate : IComponentData
    {
        public int level; // 0-D, 1-C, 2-B, 3-A, 4-S
        public float3 localPos;
    }
}
