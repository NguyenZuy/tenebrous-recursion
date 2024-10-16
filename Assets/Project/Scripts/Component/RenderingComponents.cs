using System;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

namespace Zuy.TenebrousRecursion.Component
{
    [Serializable]
    public struct InstanceRendererData : ISharedComponentData, IEquatable<InstanceRendererData>
    {
        public Mesh Mesh;
        public Material Material;
        public int SubMesh;

        public ShadowCastingMode CastShadows;
        public bool ReceiveShadows;
        public float CullDistance;
        public int InstanceShaderPropertyId;

        public bool Equals(InstanceRendererData other)
        {
            return
                Mesh == other.Mesh &&
                Material == other.Material &&
                SubMesh == other.SubMesh &&
                CastShadows == other.CastShadows &&
                ReceiveShadows == other.ReceiveShadows &&
                CullDistance == other.CullDistance &&
                InstanceShaderPropertyId == other.InstanceShaderPropertyId;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            if (!ReferenceEquals(Mesh, null)) hash ^= Mesh.GetHashCode();
            if (!ReferenceEquals(Material, null)) hash ^= Material.GetHashCode();
            hash ^= SubMesh.GetHashCode();
            hash ^= CastShadows.GetHashCode();
            hash ^= ReceiveShadows.GetHashCode();
            hash ^= CullDistance.GetHashCode();
            hash ^= InstanceShaderPropertyId.GetHashCode();
            return hash;
        }
    }
}
