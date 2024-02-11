using CodeBase.ProjectContext.Services;
using CodeBase.Utils.StateMachine;
using Cysharp.Threading.Tasks;

namespace CodeBase.ProjectContext.GameStateMachine {
    public class BootState : AState {
        private readonly IAssetReferenceContainer _assetReferenceContainer;
        private readonly IAssetProvider _assetProvider;

        public BootState(
            IAssetReferenceContainer assetReferenceContainer,
            IAssetProvider assetProvider
        ) {
            _assetReferenceContainer = assetReferenceContainer;
            _assetProvider = assetProvider;
        }

        public override void Enter(object payload = null) {
            BootServices().Forget();
        }

        private async UniTaskVoid BootServices() {
            await _assetProvider.Initialize();
            ParentMachine.TranslateTo<SceneLoadState>(_assetReferenceContainer.MainMenuScene);
        }
    }
}