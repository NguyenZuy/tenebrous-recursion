using UnityEngine;

namespace Zuy.TenebrousRecursion.ScriptableObject
{
    [CreateAssetMenu(fileName = "NightMareSO", menuName = "Scriptable Objects/NightMareSO")]
    public class NightmareSO : UnityEngine.ScriptableObject
    {
        public Gate[] gates;
        public Wave[] waves;
        public Transform[] gateSpawnPositions;
    }

    [SerializeField]
    public struct Gate
    {
        public int level; // 0-D, 1-C, 2-B, 3-A, 4-S
        public float timeToAppear; // From time to start the nightmare to now
    }

    [SerializeField]
    public struct Wave
    {
        public int id1;
        public int number1;

        public int id2;
        public int number2;

        public int id3;
        public int number3;
    }
}
