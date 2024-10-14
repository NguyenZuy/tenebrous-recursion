using System;
using Unity.Entities;
using UnityEngine;

namespace Zuy.TenebrousRecursion.Component
{
    public struct EnemyConfigElement : IBufferElementData
    {
        public int type;
        public Entity sampleEntity;
    }
}
