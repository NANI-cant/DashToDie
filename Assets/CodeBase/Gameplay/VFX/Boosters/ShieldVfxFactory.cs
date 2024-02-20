using System.Threading;
using CodeBase.Gameplay.General.Impl;
using CodeBase.ProjectContext.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using Zenject;

namespace CodeBase.Gameplay.VFX.Boosters {
    public class ShieldVfxFactory : MonoBehaviour {
        [SerializeField] private AssetReference _vfxAsset;

        private CancellationToken _destroyToken;
        private IAssetProvider _assetProvider;
        private IInstantiateService _instantiateService;
        private IDestroyProvider _destroyProvider;

        private ObjectPool<GameObject> _pool;
        private GameObject _vfxPrefab;
        private Transform _container;

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

        public void Create(GameObject holder, ShieldModifier shield) {
            CreateAsync(holder, shield).Forget();
        }

        private async UniTask CreateAsync(GameObject holder, ShieldModifier shield) {
            while (_pool == null) {
                await UniTask.Yield();
                if(_destroyToken.IsCancellationRequested) return;
            }

            var vfxObject = _pool.Get();
            vfxObject.GetComponent<ShieldVfx>().Init(holder, shield, _pool);
        }

        private async UniTask InitAsync() {
            _vfxPrefab = await _assetProvider.LoadAsset<GameObject>(_vfxAsset);
            _assetProvider.Release(_vfxAsset);
            if(_destroyToken.IsCancellationRequested) return;

            _container = new GameObject("ShieldVfxContainer").transform;
            _pool = new ObjectPool<GameObject>(
                () => {
                    var vfxObject = _instantiateService.Instantiate(_vfxPrefab, _container);
                    if (vfxObject.TryGetComponent<ParticleSystem>(out _)) {
                        var returnToPool = vfxObject.AddComponent<ReturnToPoolAfterStop>();
                        returnToPool.Pool = _pool;
                    }
                    return vfxObject;
                },
                go => go.Activate(),
                go => go.Deactivate(),
                go => _destroyProvider.Destroy(go)
            );
        }
    }
}