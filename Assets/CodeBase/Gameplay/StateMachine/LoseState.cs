using CodeBase.Configs;
using CodeBase.Metaplay;
using CodeBase.ProjectContext.GameStateMachine;
using CodeBase.ProjectContext.Services;
using CodeBase.Utils.StateMachine;
using Cysharp.Threading.Tasks;

namespace CodeBase.Gameplay.StateMachine {
    public class LoseState: AState {
        private readonly GameStateMachine _gameStateMachine;
        private readonly IAssetReferenceContainer _assetReferenceContainer;
        private readonly IStatPointsBank _statPointsBank;
        private readonly IAssetProvider _assetProvider;
        private readonly ILevelProgress _levelProgress;

        public LoseState(
            GameStateMachine gameStateMachine,
            IAssetReferenceContainer assetReferenceContainer,
            IStatPointsBank statPointsBank,
            IAssetProvider assetProvider,
            ILevelProgress levelProgress
        ) {
            _gameStateMachine = gameStateMachine;
            _assetReferenceContainer = assetReferenceContainer;
            _statPointsBank = statPointsBank;
            _assetProvider = assetProvider;
            _levelProgress = levelProgress;
        }

        public override void Enter(object payload = null) {
            ToMainMenu().Forget();
        }

        private async UniTaskVoid ToMainMenu() {
            var progressConfig = await _assetProvider.LoadAsset<IProgressConfig>(_assetReferenceContainer.ProgressConfig);
            _assetProvider.Release(_assetReferenceContainer.PlayerConfig);

            _statPointsBank.Earn(progressConfig.GetSkillPointsForRun(_levelProgress.CurrentLevel, _statPointsBank.All));
            
            await UniTask.Delay(2 * 1000);
            _gameStateMachine.TranslateTo<SceneLoadState>(_assetReferenceContainer.MainMenuScene);
        }
    }
}