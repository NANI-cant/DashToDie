using System.Threading;
using CodeBase.ProjectContext.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace CodeBase.Gameplay.Environment.Collectables.Boosters {
    [RequireComponent(typeof(Collider))]
    public class BoosterSpawner: MonoBehaviour, ICollectable {
        [SerializeField] private AssetReference[] _boostersReferences;
        [SerializeField] private Transform _boosterPlace;
        
        private IRandomService _randomService;
        private IAssetProvider _assetProvider;
        private IInstantiateService _instantiateService;
        private IDestroyProvider _destroyProvider;
        private CancellationToken _cancelToken;

        private IBooster _booster;
        private GameObject _boosterObject;

        [Inject]
        public void Construct(
            IInstantiateService instantiateService,
            IAssetProvider assetProvider,
            IRandomService randomService,
            IDestroyProvider destroyProvider,
            CancellationToken cancelToken
        ) {
            _randomService = randomService;
            _assetProvider = assetProvider;
            _instantiateService = instantiateService;
            _destroyProvider = destroyProvider;
            _cancelToken = cancelToken;
        }
        
        private void Start() {
            GetComponent<Collider>().isTrigger = true;
            Reload();
        }

        public void Reload() {
            ReloadAsync().Forget();
        }

        public void Collect(GameObject collector) {
            if(_booster == null) return;
            
            _booster.Apply(collector);
            _destroyProvider.Destroy(_boosterObject);
            _booster = null;
        }

        private async UniTask ReloadAsync() {
            AssetReference pickedBooster = _randomService.GetRandom(_boostersReferences);
            GameObject boosterPrefab = await _assetProvider.LoadAsset<GameObject>(pickedBooster);
            if(_cancelToken.IsCancellationRequested) return;
            
            if(_boosterObject != null) _destroyProvider.Destroy(_boosterObject);
            _boosterObject = _instantiateService.Instantiate(boosterPrefab, transform);
            _booster = _boosterObject.GetComponent<IBooster>();

            _boosterObject.transform.position = _boosterPlace.position;
        }
    }
}