using System;
using System.Threading;
using System.Threading.Tasks;
using CodeBase.ProjectContext.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace CodeBase.Gameplay.Enemies.AI {
    public class PistolAttacker : MonoBehaviour, IAttacker {
        [SerializeField][Min(0)] private float _fireSpeed;
        [SerializeField][Min(0)] private float _bulletSpeed;
        [SerializeField][Min(0)] private int _damage;
        [SerializeField][Min(0)] private int _fireRange;
        [SerializeField] private AssetReference _bulletAsset;
        [SerializeField] private Transform _fireOrigin;
        [SerializeField] private LayerMask _enemy;
        [SerializeField] private LayerMask _obstacle;
        
        private ITimeProvider _timeProvider;
        private IInstantiateService _instantiateService;
        private IAssetProvider _assetProvider;
        private CancellationTokenSource _attackCancel;
        private CancellationToken _destroyToken;
        private float _lastAttackTime = float.NegativeInfinity;
        private GameObject _bulletPrefab;

        public bool IsCharged => _attackCancel != null;
        
        private bool IsAttackCooldownPassed => (_timeProvider.Time - _lastAttackTime) > 1 / _fireSpeed;
        
        [Inject]
        public void Construct(
            ITimeProvider timeProvider,
            IInstantiateService instantiateService,
            IAssetProvider assetProvider
        ) {
            _timeProvider = timeProvider;
            _instantiateService = instantiateService;
            _assetProvider = assetProvider;
        }

        private void Start() {
            _destroyToken = this.GetCancellationTokenOnDestroy();
            LoadAssetsAsync().Forget();
        }

        private void OnDestroy() {
            Cancel();
        }

        public void Attack() {
            if(!IsAttackCooldownPassed || IsCharged) return;

            _attackCancel = new CancellationTokenSource();
            ExecuteAttack().Forget();
        }

        public bool CheckTargetInRange(Transform target) {
            foreach (var enemyCollider in Physics.OverlapSphere(transform.position, _fireRange, _enemy, QueryTriggerInteraction.Collide)) {
                if (enemyCollider.transform == target) {
                    Ray checkingRay = new Ray(transform.position, transform.position.DirectionTo(target.position));
                    return !Physics.Raycast(checkingRay, _fireRange, _obstacle);
                }
            }
            
            return false;
        }

        public void Cancel() {
            _attackCancel?.Cancel();
        }

        private async UniTaskVoid ExecuteAttack() {
            while (_bulletPrefab == null) {
                await UniTask.Yield();
                if (_destroyToken.IsCancellationRequested) return;
            }

            try {
                GameObject bulletObject = _instantiateService.Instantiate(_bulletPrefab);
                bulletObject.transform.SetPositionAndRotation(_fireOrigin.position, _fireOrigin.rotation);
                
                Bullet bullet = bulletObject.GetComponent<Bullet>();
                bullet.Initialize(_bulletSpeed, _damage);

                _lastAttackTime = _timeProvider.Time;
            }
            catch (OperationCanceledException) {
                
            }
            finally {
                _attackCancel?.Dispose();
                _attackCancel = null;
            }
        }

        private async UniTaskVoid LoadAssetsAsync() {
            _bulletPrefab = await _assetProvider.LoadAsset<GameObject>(_bulletAsset);
            if (_bulletPrefab == null) throw new ArgumentNullException();
            _assetProvider.Release(_bulletAsset);
        }

#if UNITY_EDITOR
        private Color _detectColor;
        
        private void OnDrawGizmos() {
            if(_fireOrigin == null) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_fireOrigin.position, _fireOrigin.position + _fireOrigin.forward * _fireRange);

            Gizmos.color = Color.white;
        }
#endif
    }
}