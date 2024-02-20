using System.Threading;
using CodeBase.Gameplay.Player;
using CodeBase.ProjectContext.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace CodeBase.Gameplay.Environment.Collectables.Boosters {
    public class SodaBooster : MonoBehaviour, IBooster {
        [SerializeField] [Min(0)] private float _staminaPerSecondMultiplier = 0.5f;
        [SerializeField] [Min(0)] private int _duration;
        [SerializeField] private AssetReference _vfxAsset;

        private CancellationToken _cancelToken;
        private IAssetProvider _assetProvider;
        private IInstantiateService _instantiateService;
        private IDestroyProvider _destroyProvider;
        
        private GameObject _vfxObject;

        [Inject]
        public void Construct(
            CancellationToken cancelToken,
            IAssetProvider assetProvider,
            IInstantiateService instantiateService,
            IDestroyProvider destroyProvider
        ) {
            _cancelToken = cancelToken;
            _assetProvider = assetProvider;
            _instantiateService = instantiateService;
            _destroyProvider = destroyProvider;
        }

        public void Apply(GameObject target) {
            var staminaAutoRegen = target.GetComponent<StaminaAutoRegen>();
            staminaAutoRegen.StaminaPerSecond.Increase(_staminaPerSecondMultiplier);
            CreateVfxAsync(target.transform, _cancelToken).Forget();
            
            CancelAfterDelay(staminaAutoRegen).Forget();
        }

        private async UniTask CreateVfxAsync(Transform holder, CancellationToken cancelToken) {
            var vfxPrefab = await _assetProvider.LoadAsset<GameObject>(_vfxAsset);
            _assetProvider.Release(_vfxAsset);
            if(cancelToken.IsCancellationRequested) return;

            _vfxObject = _instantiateService.Instantiate(vfxPrefab, holder);
        }

        private async UniTask CancelAfterDelay(StaminaAutoRegen staminaAutoRegen) {
            await UniTask.Delay(_duration * 1000);
            if(_cancelToken.IsCancellationRequested) return;
            
            staminaAutoRegen.StaminaPerSecond.Decrease(_staminaPerSecondMultiplier);
            _destroyProvider.Destroy(_vfxObject);
        }
    }
}