using System;
using CodeBase.Gameplay.General.Impl;
using CodeBase.ProjectContext.Services;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;

namespace CodeBase.Gameplay.VFX.Boosters {
    public class ShieldVfx : MonoBehaviour {
        [SerializeField] private ParticleSystem _destroyVfxPrefab;

        private IObjectPool<GameObject> _pool;
        private IInstantiateService _instantiateService;
        private Transform _transform;

        [Inject]
        public void Construct(IInstantiateService instantiateService) {
            _instantiateService = instantiateService;
        }

        private void Awake() => _transform = transform;

        public void Init(
            GameObject holder,
            ShieldModifier shieldModifier,
            IObjectPool<GameObject> objectPool
        ) {
            _pool = objectPool;
            var animator = holder.GetComponentInChildren<Animator>();
            if (animator == null) {
                _transform.parent = holder.transform;
                _transform.position = holder.transform.position;
            }
            else {
                var hipsTransform = animator.GetBoneTransform(HumanBodyBones.Hips);
                _transform.parent = hipsTransform;
                _transform.position = hipsTransform.position;    
            }
            
            shieldModifier.Removed += OnRemoved;
        }

        private void OnRemoved() {
            var destroyVfxObject = _instantiateService.Instantiate(_destroyVfxPrefab.gameObject);
            destroyVfxObject.transform.position = _transform.position;
            _pool.Release(gameObject);
        }
    }
}