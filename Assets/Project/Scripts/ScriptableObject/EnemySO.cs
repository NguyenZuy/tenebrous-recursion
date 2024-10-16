using UnityEngine;

namespace Zuy.TenebrousRecursion.ScriptableObject
{
    [CreateAssetMenu(fileName = "EnemySO", menuName = "Scriptable Objects/EnemySO")]
    public class EnemySO : UnityEngine.ScriptableObject
    {
        public int type; // also is id
        public float hp;
        public float defense;
        public float moveSpeed;
        public float atk;

        public Material material;
        public Mesh mesh;
        public float cullingDistance;

        public float gridPixelSize;
        public Texture2D textureSheet;
        public Sprite[] spriteFrames;
    }
}
