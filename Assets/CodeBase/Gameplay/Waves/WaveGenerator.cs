using System;
using System.Collections.Generic;
using System.Threading;
using CodeBase.ProjectContext.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Waves {
    public class WaveGenerator: IInitializable {
        private readonly IAssetProvider _assetProvider;
        private readonly IAssetReferenceContainer _assetReferenceContainer;
        private readonly IRandomService _randomService;
        private readonly IInstantiateService _instantiateService;
        private readonly CancellationToken _cancelToken;
        
        private GameObject[] _enemyPrefabs;

        public WaveGenerator(
            IAssetProvider assetProvider,
            IAssetReferenceContainer assetReferenceContainer,
            IRandomService randomService,
            IInstantiateService instantiateService,
            CancellationToken cancelToken
        ) {
            _assetProvider = assetProvider;
            _assetReferenceContainer = assetReferenceContainer;
            _randomService = randomService;
            _instantiateService = instantiateService;
            _cancelToken = cancelToken;
        }

        public void Initialize() {
            InitializeAsync().Forget();
        }

        public async UniTask<GameObject[]> Generate(int count) {
            if (_enemyPrefabs.Length == 0) await UniTask.Yield();
            if (_cancelToken.IsCancellationRequested) return null;

            List<GameObject> enemies = new List<GameObject>();

            for (int i = 0; i < count; i++) {
                GameObject pickedPrefab = _enemyPrefabs[_randomService.Range(0, _enemyPrefabs.Length)];
                var enemy = _instantiateService.Instantiate(pickedPrefab);
                enemies.Add(enemy);
            }

            return enemies.ToArray();
        }

        private async UniTaskVoid InitializeAsync() {
            _enemyPrefabs = await _assetProvider.LoadAssets<GameObject>(_assetReferenceContainer.Enemies);
            _assetProvider.Release(_assetReferenceContainer.Enemies);
            if(_cancelToken.IsCancellationRequested) return;
            
            if (_enemyPrefabs.Length == 0) throw new ArgumentNullException("No enemies Prefabs");
        }
    }
}