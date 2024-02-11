using System;
using System.Threading;
using CodeBase.Configs;
using CodeBase.Gameplay.Enemies.Signals;
using CodeBase.ProjectContext.Services;
using Cysharp.Threading.Tasks;
using Zenject;

namespace CodeBase.Metaplay {
    public class MoneyForKillService: IInitializable, IDisposable {
        private readonly IMoneyBank _moneyBank;
        private readonly SignalBus _signalBus;
        private readonly ILevelProgress _levelProgress;
        private readonly IAssetProvider _assetProvider;
        private readonly IAssetReferenceContainer _assetReferenceContainer;
        private readonly CancellationToken _cancelToken;
        
        private IProgressConfig _progressConfig;

        public MoneyForKillService(
            IMoneyBank moneyBank,
            SignalBus signalBus,
            ILevelProgress levelProgress,
            IAssetProvider assetProvider,
            IAssetReferenceContainer assetReferenceContainer,
            CancellationToken cancelToken
        ) {
            _moneyBank = moneyBank;
            _signalBus = signalBus;
            _levelProgress = levelProgress;
            _assetProvider = assetProvider;
            _assetReferenceContainer = assetReferenceContainer;
            _cancelToken = cancelToken;
        }

        public void Initialize() {
            _signalBus.Subscribe<EnemyDiedSignal>(GiveMoney);
            InitAsync().Forget();
        }

        private async UniTask InitAsync() {
            _progressConfig = await _assetProvider.LoadAsset<IProgressConfig>(_assetReferenceContainer.ProgressConfig);
            _assetProvider.Release(_assetReferenceContainer.ProgressConfig);
            if(_cancelToken.IsCancellationRequested) return;

            if (_progressConfig == null) throw new ArgumentNullException();
        }

        public void Dispose() => _signalBus.Unsubscribe<EnemyDiedSignal>(GiveMoney);

        private void GiveMoney(EnemyDiedSignal signal) => GiveAsync().Forget();

        private async UniTask GiveAsync() {
            while (_progressConfig == null) {
                await UniTask.Yield();
                if(_cancelToken.IsCancellationRequested) return;
            }

            _moneyBank.Earn(_progressConfig.GetMoneyForKill(_levelProgress.CurrentLevel));
        }
    }
}