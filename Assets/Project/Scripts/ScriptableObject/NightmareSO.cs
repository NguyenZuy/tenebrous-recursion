using System;
using UnityEngine;

namespace Zuy.TenebrousRecursion.ScriptableObject
{
    [CreateAssetMenu(fileName = "NightMareSO", menuName = "Scriptable Objects/NightMareSO")]
    public class NightmareSO : UnityEngine.ScriptableObject
    {
        [SerializeField] public Gate[] gates;
        [SerializeField] public Wave[] waves;
        public Vector3[] gateSpawnPositions;
    }

    [Serializable]
    public struct Gate
    {
        public int level; // 1-D, 2-C, 3-B, 4-A, 5-S
        public float timeToAppear; // From time to start the nightmare to now (seconds)
    }

    [Serializable]
    public struct Wave
    {
        public float timeToAppear; // From time to start the nightmare to now

        public int id1;
        public int number1;

        public int id2;
        public int number2;

        public int id3;
        public int number3;
    }
}
