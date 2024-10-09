using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Zuy.TenebrousRecursion.Component;

namespace Zuy.TenebrousRecursion.Hybrid
{
    public class CameraFollow : MonoBehaviour
    {
        private float3 offset = new float3(0f, 0f, -10f);
        private float smoothTime = 0.25f;
        private Vector3 velocity = Vector3.zero;

        private World _world;
        private EntityManager _entityManager;
        private LocalTransform _playerLocalTransform;
        private EntityQuery _playerQuery;

        private void Start()
        {
            _world = World.DefaultGameObjectInjectionWorld;
            _entityManager = _world.EntityManager;

            _playerQuery = _entityManager.CreateEntityQuery(typeof(Player), typeof(LocalTransform));

        }

        private void LateUpdate()
        {
            if (_playerQuery.TryGetSingleton(out _playerLocalTransform))
            {
                Vector3 targetPosition = _playerLocalTransform.Position + offset;
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            }
        }
    }

}
