using System.Threading;
using CodeBase.ProjectContext.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using Zenject;

namespace CodeBase.Gameplay.VFX {
    public class HitVfxFactory: MonoBehaviour {
        [SerializeField] private AssetReference _hitAsset;

        private CancellationToken _destroyToken;
        private IAssetProvider _assetProvider;
        private IInstantiateService _instantiateService;
        private IDestroyProvider _destroyProvider;

        private ObjectPool<GameObject> _hitPool;
        private GameObject _hitPrefab;
        private Transform _vfxContainer;

        [Inject]
        private void Construct(
            IInstantiateService instantiateService,
            IAssetProvider assetProvider,
            IDestroyProvider destroyProvider
        ) {
            _assetProvider = assetProvider;
            _instantiateService = instantiateService;
            _destroyProvider = destroyProvider;
        }

        private void Awake() {
            _destroyToken = this.GetCancellationTokenOnDestroy();
            InitAsync().Forget();
        }

        public void Create(Vector3 position) {
            CreateAsync(position).Forget();
        }

        private async UniTask CreateAsync(Vector3 position) {
            while (_hitPool == null) {
                await UniTask.Yield();
                if(_destroyToken.IsCancellationRequested) return;
            }

            var hitObject = _hitPool.Get();
            hitObject.transform.position = position;
        }

        private async UniTask InitAsync() {
            _hitPrefab = await _assetProvider.LoadAsset<GameObject>(_hitAsset);
            _assetProvider.Release(_hitAsset);
            if(_destroyToken.IsCancellationRequested) return;

            _vfxContainer = new GameObject("HitVfxContainer").transform;
            _hitPool = new ObjectPool<GameObject>(
                () => {
                    var hitObject = _instantiateService.Instantiate(_hitPrefab, _vfxContainer);
                    if (hitObject.TryGetComponent<ParticleSystem>(out _)) {
                        var returnToPool = hitObject.AddComponent<ReturnToPoolAfterStop>();
                        returnToPool.Pool = _hitPool;
                    }
                    return hitObject;
                },
                go => go.Activate(),
                go => go.Deactivate(),
                go => _destroyProvider.Destroy(go)
            );
        }
    }
}