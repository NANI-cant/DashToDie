using System;
using System.Collections.Generic;
using System.Threading;
using CodeBase.ProjectContext.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.PlayerStats.Impl {
    public class PlayerStatsContainer: IInitializable, IPlayerStatsContainer {
        private readonly IAssetProvider _assetProvider;
        private readonly IAssetReferenceContainer _assetReferenceContainer;
        private readonly CancellationToken _cancelToken;
        private readonly List<IStat> _stats = new();

        public PlayerStatsContainer(
            IAssetProvider assetProvider,
            IAssetReferenceContainer assetReferenceContainer,
            CancellationToken cancelToken
        ) {
            _assetProvider = assetProvider;
            _assetReferenceContainer = assetReferenceContainer;
            _cancelToken = cancelToken;
        }

        public void Initialize() {
            InitAsync().Forget();
        }

        public async UniTask SetupPlayer(GameObject player) {
            while (_stats.Count == 0) await UniTask.Yield();

            foreach (var stat in _stats) {
                stat.Apply(player);
            }
        }

        private async UniTaskVoid InitAsync() {
            var stats = await _assetProvider.LoadAssets<IStat>(_assetReferenceContainer.PlayerStats);
            _assetProvider.Release(_assetReferenceContainer.PlayerStats);
            if (_cancelToken.IsCancellationRequested) return;
            
            if (stats.Length == 0) throw new ArgumentNullException();
            _stats.AddRange(stats);
        }
    }
}