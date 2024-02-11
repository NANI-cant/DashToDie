using System;
using System.Threading;
using CodeBase.ProjectContext.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CodeBase.Map.Building {
    public class BoostersPlacer: IInitializable {
        private readonly IAssetReferenceContainer _assetReferenceContainer;
        private readonly IAssetProvider _assetProvider;
        private readonly IInstantiateService _instantiateService;
        private readonly IRandomService _randomService;
        private readonly MapPlacer _mapPlacer;
        private readonly Map _map;
        private readonly CancellationToken _cancellationToken;
        
        private GameObject _boosterSpawnerPrefab;

        public BoostersPlacer(
            IAssetReferenceContainer assetReferenceContainer,
            IAssetProvider assetProvider,
            IInstantiateService instantiateService,
            IRandomService randomService,
            MapPlacer mapPlacer,
            Map map,
            CancellationToken cancellationToken
        ) {
            _assetReferenceContainer = assetReferenceContainer;
            _assetProvider = assetProvider;
            _instantiateService = instantiateService;
            _randomService = randomService;
            _mapPlacer = mapPlacer;
            _map = map;
            _cancellationToken = cancellationToken;
        }

        public void Initialize() => InitAsync().Forget();

        public async UniTask<GameObject[]> Place(int count) {
            while (_boosterSpawnerPrefab == null) {
                await UniTask.Yield();
                if (_cancellationToken.IsCancellationRequested) return null;
            }

            GameObject[] spawnPoints = new GameObject[count];
            for (int i = 0; i < count; i++) {
                Vector3Int pickedPosition = _map.AvailableCells[_randomService.Range(0, _map.AvailableCells.Count)];
                
                var boosterSpawnPointObject = _instantiateService.Instantiate(_boosterSpawnerPrefab);
                _mapPlacer.Place(boosterSpawnPointObject,pickedPosition,_map);
                spawnPoints[i] = boosterSpawnPointObject;
            }

            return spawnPoints;
        }

        private async UniTask InitAsync() {
            _boosterSpawnerPrefab = await _assetProvider.LoadAsset<GameObject>(_assetReferenceContainer.BoosterSpawner);
            _assetProvider.Release(_assetReferenceContainer.BoosterSpawner);
            if (_boosterSpawnerPrefab == null) throw new ArgumentNullException();
            
            if (_cancellationToken.IsCancellationRequested) return;
        }
    }
}
