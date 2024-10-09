using UnityEngine;
using Zuy.Workspace.Base;
using Zuy.Workspace.MobileController;

namespace Zuy.TenebrousRecursion.Hybrid
{
    public class PlayerInputBridge : BaseSingleton<PlayerInputBridge>
    {
        public UniversalButton inputMove;

        private Vector3 _cachedInput;
        public Vector3 CachedInput => _cachedInput;

        private bool _lerpStopping = false;

        private float _moveSpeed;
        public float MoveSpeed
        {
            get { return _moveSpeed; }
            set { _moveSpeed = value; }
        }

        void Update()
        {
            if (inputMove.isFingerDown)
            {
                _cachedInput = inputMove.direction;
                transform.forward = _cachedInput;
            }
            else
            {
                if (_lerpStopping)
                {
                    _cachedInput = Vector3.Lerp(_cachedInput, Vector3.zero, _moveSpeed * Time.deltaTime);
                }
                else
                {
                    _cachedInput = Vector3.zero;
                }
            }
        }
    }
}