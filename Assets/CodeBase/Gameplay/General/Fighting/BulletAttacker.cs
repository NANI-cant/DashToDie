using System;
using System.Threading;
using System.Threading.Tasks;
using CodeBase.Gameplay.General.Cooldowning.Impl;
using CodeBase.Gameplay.General.Fighting.Ammunition;
using CodeBase.ProjectContext.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using Zenject;

namespace CodeBase.Gameplay.General.Fighting {
    public abstract class BulletAttacker: MonoBehaviour, IAttacker {
        [SerializeField][Min(0)] private float _fireSpeed;
        [SerializeField][Min(0)] private float _reloadTime;
        [SerializeField][Min(1)] private int _maxAmmo;
        [SerializeField][Min(0)] private float _bulletSpeed;
        [SerializeField][Min(0)] private int _damage;
        [SerializeField] private AssetReference _bulletAsset;
        [SerializeField] private Transform _fireOrigin;
        [SerializeField] private Animator _animator;
        [SerializeField] private UnityEvent _onFire;

        private static readonly int AttackKey = Animator.StringToHash("Attack");
        private static readonly int ReloadKey = Animator.StringToHash("Reload");

        private ITimeProvider _timeProvider;
        private IInstantiateService _instantiateService;
        private IAssetProvider _assetProvider;
        private CancellationTokenSource _fireProcess;
        private CancellationTokenSource _reloadProcess;
        private CancellationToken _destroyToken;
        private GameObject _bulletPrefab;
        private Cooldown _fireCooldown;
        
        public IAmmo Ammo { get; private set; }
        public bool IsCharging => _fireProcess != null;
        public bool IsReloading => _reloadProcess != null;

        [Inject]
        public void Construct(
            ITimeProvider timeProvider,
            IInstantiateService instantiateService,
            IAssetProvider assetProvider
        ) {
            _timeProvider = timeProvider;
            _instantiateService = instantiateService;
            _assetProvider = assetProvider;

            Ammo = Init(_maxAmmo);
            if (Ammo == null) throw new ArgumentNullException();
            
            _fireCooldown = new Cooldown(1 / _fireSpeed);
        }

        protected abstract IAmmo Init(int ammoCount);

        private void Start() {
            _destroyToken = this.GetCancellationTokenOnDestroy();
            LoadAssetsAsync().Forget();
        }

        private void Update() => _fireCooldown.Tick(_timeProvider.DelaTime);
        private void OnDestroy() => Cancel();

        public void Attack() {
            if(!_fireCooldown.IsTimesUp || IsCharging || Ammo.Current == 0) return;

            _fireProcess = new CancellationTokenSource();
            ExecuteFireProcess(_fireProcess.Token).Forget();
        }

        public void Reload() {
            _reloadProcess = new CancellationTokenSource();
            ExecuteReloadProcess(_reloadProcess.Token).Forget();
        }

        public void Cancel() {
            _fireProcess?.Cancel();
            _fireProcess?.Dispose();
            _fireProcess = null;
            
            _reloadProcess?.Cancel();
            _reloadProcess?.Dispose();
            _reloadProcess = null;
        }

        private async UniTaskVoid ExecuteFireProcess(CancellationToken cancelToken) {
            try { await WaitBulletAsset(cancelToken); }
            catch (TaskCanceledException) { return; }
            
            if(_animator != null) _animator.SetTrigger(AttackKey);
            _onFire.Invoke();
            GameObject bulletObject = _instantiateService.Instantiate(_bulletPrefab);
            bulletObject.transform.SetPositionAndRotation(_fireOrigin.position, _fireOrigin.rotation);

            Bullet bullet = bulletObject.GetComponent<Bullet>();
            bullet.Initialize(_bulletSpeed, _damage);

            Ammo.Current--;
            _fireCooldown = new Cooldown(1 / _fireSpeed);
            _fireCooldown.WindUp();

            _fireProcess?.Dispose();
            _fireProcess = null;
        }

        private async UniTask ExecuteReloadProcess(CancellationToken reloadProcessToken) {
            try {
                if(_animator != null) _animator.SetTrigger(ReloadKey);
                await UniTask.Delay((int) (_reloadTime * 1000), cancellationToken: reloadProcessToken);
                if(_destroyToken.IsCancellationRequested) return;
                
                Ammo.Current = Ammo.Max;
            
                _reloadProcess?.Dispose();
                _reloadProcess = null;
            }
            catch (TaskCanceledException) { }
        }

        private async UniTask WaitBulletAsset(CancellationToken cancelToken) {
            while (_bulletPrefab == null) {
                await UniTask.Yield();
                if (_destroyToken.IsCancellationRequested || cancelToken.IsCancellationRequested) throw new TaskCanceledException();
            }
        }

        private async UniTaskVoid LoadAssetsAsync() {
            _bulletPrefab = await _assetProvider.LoadAsset<GameObject>(_bulletAsset);
            if (_bulletPrefab == null) throw new ArgumentNullException();
            _assetProvider.Release(_bulletAsset);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            if(_fireOrigin == null) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_fireOrigin.position, _fireOrigin.position + _fireOrigin.forward * 5);

            Gizmos.color = Color.white;
        }
#endif
    }
}