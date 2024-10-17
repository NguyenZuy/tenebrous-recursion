using System.Collections.Generic;
using System.Collections.ObjectModel;
using GluonGui.Dialog;
using UnityEngine;
using Zuy.TenebrousRecursion.ScriptableObject;
using Zuy.Workspace.Base;

namespace Zuy.TenebrousRecursion.Mono
{
    public class ConfigManager : BaseSingleton<ConfigManager>
    {
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

        public EnemySO GetEnemeyConfigByType(int type)
        {
            if (enemyConfigsDict.TryGetValue(type, out EnemySO result))
                return result;
            else
                return null;
        }
    }
}
