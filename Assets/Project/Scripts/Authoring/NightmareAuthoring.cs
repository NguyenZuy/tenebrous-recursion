using Unity.Entities;
using UnityEngine;
using Zuy.TenebrousRecursion.Component;
using Zuy.TenebrousRecursion.ScriptableObject;

namespace Zuy.TenebrousRecursion.Authoring
{
    // Each scene has a singleton this
    class NightmareAuthoring : MonoBehaviour
    {
        public NightmareSO nightmareSO;

        class Baker : Baker<NightmareAuthoring>
        {
            public override void Bake(NightmareAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);

                AddComponent(entity, new Nightmare()
                {

                });

                var gateBuffer = AddBuffer<Component.GateConfig>(entity);
                foreach (var gate in authoring.nightmareSO.gates)
                {
                    gateBuffer.Add(new Component.GateConfig()
                    {
                        level = gate.level,
                        timeToAppear = gate.timeToAppear
                    });
                }

                var waveBuffer = AddBuffer<Component.WaveConfig>(entity);
                foreach (var wave in authoring.nightmareSO.waves)
                {
                    waveBuffer.Add(new Component.WaveConfig()
                    {
                        timeToAppear = wave.timeToAppear,

                        id1 = wave.id1,
                        number1 = wave.number1,

                        id2 = wave.id2,
                        number2 = wave.number2,

                        id3 = wave.id3,
                        number3 = wave.number3,
                    });
                }

                var gateSpawnPosBuffer = AddBuffer<Component.GateSpawnPositionConfig>(entity);
                foreach (var spawnPos in authoring.nightmareSO.gateSpawnPositions)
                {
                    gateSpawnPosBuffer.Add(new Component.GateSpawnPositionConfig()
                    {
                        value = spawnPos
                    });
                }
            }
        }
    }
}
