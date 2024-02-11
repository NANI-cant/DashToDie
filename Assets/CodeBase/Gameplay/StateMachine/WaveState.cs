using System.Threading;
using CodeBase.Configs;
using CodeBase.Gameplay.Environment.Collectables.Boosters;
using CodeBase.Gameplay.General;
using CodeBase.Gameplay.Player;
using CodeBase.Gameplay.Player.Signals;
using CodeBase.Gameplay.Progress;
using CodeBase.Gameplay.Waves;
using CodeBase.Map;
using CodeBase.Map.Building;
using CodeBase.ProjectContext.Services;
using CodeBase.Utils.StateMachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.StateMachine {
    public class WaveState: AState {
        private readonly EnemiesPlacer _enemiesPlacer;
        private readonly WaveGenerator _waveGenerator;
        private readonly WaveTracker _waveTracker;
        private readonly ILevelProgress _levelProgress;
        private readonly IAssetProvider _assetProvider;
        private readonly IAssetReferenceContainer _assetReferenceContainer;
        private readonly SignalBus _signalBus;
        private readonly CancellationToken _cancelToken;

        private GameObject[] _enemiesOnWave;

        public WaveState(
            EnemiesPlacer enemiesPlacer,
            WaveGenerator waveGenerator,
            WaveTracker waveTracker,
            ILevelProgress levelProgress,
            IAssetProvider assetProvider,
            IAssetReferenceContainer assetReferenceContainer,
            SignalBus signalBus,
            CancellationToken cancelToken
        ) {
            _enemiesPlacer = enemiesPlacer;
            _waveGenerator = waveGenerator;
            _waveTracker = waveTracker;
            _levelProgress = levelProgress;
            _assetProvider = assetProvider;
            _assetReferenceContainer = assetReferenceContainer;
            _signalBus = signalBus;
            _cancelToken = cancelToken;
        }
        
        public override void Enter(object payload = null) {
            _signalBus.Subscribe<PlayerDiedSignal>(Lose);
            
            SetupWave().Forget();
            foreach (var spawner in Object.FindObjectsOfType<BoosterSpawner>()) {
                spawner.Reload();
            }
            _waveTracker.Cleared += ToNextWave;
        }

        public override void Exit() {
            _waveTracker.Cleared -= ToNextWave;
            _waveTracker.Clear();
            _waveTracker.UnTrackAll();
            _signalBus.Unsubscribe<PlayerDiedSignal>(Lose);
        }

        private async UniTask SetupWave() {
            await UniTask.Delay(1 * 1000);
            if(_cancelToken.IsCancellationRequested) return;
            
            IMapConfig mapConfig = await _assetProvider.LoadAsset<IMapConfig>(_assetReferenceContainer.MapConfig);
            _assetProvider.Release(_assetReferenceContainer.MapConfig);

            int enemiesToSpawnCount = (int) ProgressRule.Linear(mapConfig.EnemiesOnWave, mapConfig.EnemiesOnWaveMultiplier, _levelProgress.CurrentLevel);
            _enemiesOnWave = await _waveGenerator.Generate(enemiesToSpawnCount);
            if(_cancelToken.IsCancellationRequested) return;

            _waveTracker.Track(_enemiesOnWave.GetComponentFromAll<IHealth>());

            _enemiesPlacer.Place(_enemiesOnWave);
        }

        private void ToNextWave() {
            _levelProgress.PassWave();
            if (_levelProgress.WavesReminded > 0) 
                ParentMachine.TranslateTo<WaveState>();
            else 
                ParentMachine.TranslateTo<RestState>();
        }

        private void Lose(PlayerDiedSignal signal) {
            ParentMachine.TranslateTo<LoseState>();
        }
    }
}