using Unity.Entities;
using UnityEngine;
using Zuy.TenebrousRecursion.Component;
using Zuy.TenebrousRecursion.ScriptableObject;

namespace Zuy.TenebrousRecursion.Authoring
{
    class WaveManager : MonoBehaviour
    {
        public WaveSO[] waveSOs;

        class Baker : Baker<WaveManager>
        {
            public override void Bake(WaveManager authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);

                // AddComponent(entity, new Component.WaveManager()
                // {

                // });

                // var buffer = AddBuffer<Wave>(entity);
                // foreach (var wave in authoring.waveSOs)
                // {
                //     buffer.Add(new Wave()
                //     {
                //         id1 = wave.id1,
                //         quantity1 = wave.quantity1,

                //         id2 = wave.id2,
                //         quantity2 = wave.quantity2,

                //         id3 = wave.id3,
                //         quantity3 = wave.quantity3
                //     });
                // }
            }
        }
    }
}
