using System;
using System.Threading;
using CodeBase.Configs;
using CodeBase.Gameplay.General.Brains.Impl;
using CodeBase.Gameplay.PlayerStats;
using CodeBase.ProjectContext.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Player {
    public class PlayerFactory: IPlayerFactory, IInitializable {
        private readonly IAssetReferenceContainer _assetReferenceContainer;
        private readonly IAssetProvider _assetProvider;
        private readonly IPlayerStatsContainer _statsContainer;
        private readonly IInstantiateService _instantiateService;
        private readonly CancellationToken _cancellationToken;

        private GameObject _prefab;
        private IPlayerConfig _config;

        public PlayerFactory(
            IAssetReferenceContainer assetReferenceContainer,
            IAssetProvider assetProvider,
            IPlayerStatsContainer statsContainer,
            IInstantiateService instantiateService,
            CancellationToken cancellationToken
        ) {
            _assetReferenceContainer = assetReferenceContainer;
            _assetProvider = assetProvider;
            _statsContainer = statsContainer;
            _instantiateService = instantiateService;
            _cancellationToken = cancellationToken;
        }
        
        public async UniTask<GameObject> Create() {
            while (_prefab == null || _config == null) {
                await UniTask.Yield();
                if (_cancellationToken.IsCancellationRequested) return null;
            }

            var playerObject = _instantiateService.Instantiate(_prefab);
            playerObject.GetComponent<Slasher>().Initialize(_config.StaminaPrice, _config.Recovery);

            await _statsContainer.SetupPlayer(playerObject);
            
            return playerObject;
        }

        public void Initialize() {
            InitAsync().Forget();
        }

        private async UniTaskVoid InitAsync() {
            (_prefab, _config) = await (
                _assetProvider.LoadAsset<GameObject>(_assetReferenceContainer.Player),
                _assetProvider.LoadAsset<IPlayerConfig>(_assetReferenceContainer.PlayerConfig)
            );
            _assetProvider.Release(_assetReferenceContainer.Player);
            _assetProvider.Release(_assetReferenceContainer.PlayerConfig);
            if(_cancellationToken.IsCancellationRequested) return;
            

            if (_prefab == null) throw new ArgumentNullException();
            if (_config == null) throw new ArgumentNullException();
        }
    }
}