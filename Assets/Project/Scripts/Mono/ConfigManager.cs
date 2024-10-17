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

        public EnemySO GetEnemeyConfigByType(int type)
        {
            EnemySO result = null;

            foreach (var config in _enemyConfigs)
            {
                if (config.type == type)
                {
                    result = config;
                    break;
                }
            }

            return result;
        }
    }
}
