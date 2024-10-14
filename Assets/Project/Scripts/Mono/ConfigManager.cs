using UnityEngine;
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
    }
}
