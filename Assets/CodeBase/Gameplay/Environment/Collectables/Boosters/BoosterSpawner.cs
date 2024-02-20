using System;
using System.Threading;
using CodeBase.ProjectContext.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using Zenject;

namespace CodeBase.Gameplay.Environment.Collectables.Boosters {
    [RequireComponent(typeof(Collider))]
    public class BoosterSpawner: MonoBehaviour, ICollectable {
        [SerializeField] private AssetReference[] _boostersReferences;
        [SerializeField] private Transform _boosterPlace;

        [SerializeField][Min(0)] private float _rotationSpeed;
        [SerializeField] private AnimationCurve _waveEffect;
        [SerializeField][Min(0)] private float _waveEffectSpeed;

        [SerializeField] private UnityEvent _onReloaded;
        [SerializeField] private UnityEvent _onCollected;
        
        private IRandomService _randomService;
        private IAssetProvider _assetProvider;
        private IInstantiateService _instantiateService;
        private IDestroyProvider _destroyProvider;
        private CancellationToken _cancelToken;
        private ITimeProvider _time;

        private Vector3 _boosterStartPosition;
        private float _waveProgress = 0;
        
        private IBooster _booster;
        private GameObject _boosterObject;

        [Inject]
        public void Construct(
            IInstantiateService instantiateService,
            IAssetProvider assetProvider,
            IRandomService randomService,
            IDestroyProvider destroyProvider,
            ITimeProvider timeProvider,
            CancellationToken cancelToken
        ) {
            _randomService = randomService;
            _assetProvider = assetProvider;
            _instantiateService = instantiateService;
            _destroyProvider = destroyProvider;
            _cancelToken = cancelToken;
            _time = timeProvider;
        }

        private void Start() {
            GetComponent<Collider>().isTrigger = true;
            _boosterStartPosition = _boosterPlace.position;
            Reload();
        }

        private void Update() {
            _boosterPlace.transform.position = _boosterStartPosition + Vector3.up * _waveEffect.Evaluate(_waveProgress);
            _waveProgress += _time.DelaTime * _waveEffectSpeed;
            if (_waveProgress > 1) _waveProgress = 0;

            _boosterPlace.Rotate(new Vector3(0, _rotationSpeed * _time.DelaTime), Space.Self);
        }

        public void Reload() {
            ReloadAsync().Forget();
        }

        public void Collect(GameObject collector) {
            if(_booster == null) return;
            
            _booster.Apply(collector);
            _destroyProvider.Destroy(_boosterObject);
            _booster = null;
            _onCollected?.Invoke();
        }

        private async UniTask ReloadAsync() {
            AssetReference pickedBooster = _randomService.GetRandom(_boostersReferences);
            GameObject boosterPrefab = await _assetProvider.LoadAsset<GameObject>(pickedBooster);
            if(_cancelToken.IsCancellationRequested) return;
            
            if(_boosterObject != null) _destroyProvider.Destroy(_boosterObject);
            _boosterObject = _instantiateService.Instantiate(boosterPrefab, transform);
            _booster = _boosterObject.GetComponent<IBooster>();

            _boosterObject.transform.position = _boosterPlace.position;
            _boosterObject.transform.parent = _boosterPlace;
            
            _onReloaded?.Invoke();
        }
    }
}