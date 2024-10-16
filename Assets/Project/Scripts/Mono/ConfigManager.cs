using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Zuy.TenebrousRecursion.ScriptableObject;
using Zuy.Workspace.Base;

namespace Zuy.TenebrousRecursion.Mono
{
    public class ConfigManager : BaseSingleton<ConfigManager>
    {
        public Material material;
        public Mesh mesh;
        public float cullingDistance;

        public float gridPixelSize;
        public Texture2D textureSheet;
        public Sprite[] spriteFrames;

        [SerializeField] private EnemySO[] _enemyConfigs;

        public ReadOnlyDictionary<int, EnemySO> enemyConfigsDict;

        void Start()
        {
            Dictionary<int, EnemySO> builderDict = new Dictionary<int, EnemySO>();

            foreach (var so in _enemyConfigs)
            {
                builderDict.Add(so.type, so);
            }

            this.enemyConfigsDict = new ReadOnlyDictionary<int, EnemySO>(builderDict);
        }
    }
}
