using CodeBase.Configs;
using CodeBase.Gameplay.Progress;
using Cysharp.Threading.Tasks;
using Zenject;

namespace CodeBase.ProjectContext.Services.Impl {
    public class LevelProgress : ILevelProgress, IInitializable {
        private readonly IAssetProvider _assetProvider;
        private readonly IAssetReferenceContainer _assetReferenceContainer;
        
        private IMapConfig _config;
        
        public int CurrentLevel { get; private set; }
        public int WavesReminded { get; private set; }

        public LevelProgress(
            IAssetProvider assetProvider,
            IAssetReferenceContainer assetReferenceContainer,
            int startLevel = 0
        ) {
            _assetProvider = assetProvider;
            _assetReferenceContainer = assetReferenceContainer;
            CurrentLevel = startLevel;
        }

        public void PassLevel() {
            CurrentLevel++;
            WavesReminded = (int) ProgressRule.Linear(_config.Waves, _config.WavesMultiplier, CurrentLevel);
        }

        public void PassWave() {
            WavesReminded--;
        }

        public void Initialize() {
            InitAsync().Forget();
        }

        private async UniTaskVoid InitAsync() {
            _config = await _assetProvider.LoadAsset<IMapConfig>(_assetReferenceContainer.MapConfig);
            WavesReminded = (int) ProgressRule.Linear(_config.Waves, _config.WavesMultiplier, CurrentLevel);
        }
    }
}