using System;
using UnityEngine;

namespace Zuy.TenebrousRecursion.ScriptableObject
{
    [CreateAssetMenu(fileName = "WaveSO", menuName = "Scriptable Objects/WaveSO")]
    public class WaveSO : UnityEngine.ScriptableObject
    {
        public int id1;
        public int quantity1;
        public int id2;
        public int quantity2;
        public int id3;
        public int quantity3;
    }
}
