using System.Threading;
using CodeBase.ProjectContext.Services;
using CodeBase.Utils.StateMachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CodeBase.ProjectContext.GameStateMachine {
    public class SceneLoadState : AState {
        private readonly ISceneLoadService _sceneLoadService;
        private readonly IInstantiateService _instantiateService;
        private readonly IAssetReferenceContainer _assetReferenceContainer;
        private readonly IAssetProvider _assetProvider;
        private readonly CancellationToken _cancellationToken;

        private GameObject _loadingScreen;

        public SceneLoadState(
            ISceneLoadService sceneLoadService,
            IInstantiateService instantiateService,
            IAssetReferenceContainer assetReferenceContainer,
            IAssetProvider assetProvider,
            CancellationToken cancellationToken
        ) {
            _sceneLoadService = sceneLoadService;
            _instantiateService = instantiateService;
            _assetReferenceContainer = assetReferenceContainer;
            _assetProvider = assetProvider;
            _cancellationToken = cancellationToken;
        }
        
        public override void Enter(object payload = null) {
            AssetReference sceneToLoad = payload as AssetReference;
            LoadScene(sceneToLoad).Forget();
        }

        private async UniTaskVoid LoadScene(AssetReference sceneToLoad) {
            if (_loadingScreen == null) {
                var loadingScreenPrefab = await _assetProvider.LoadAsset<GameObject>(_assetReferenceContainer.LoadingScreen);
                _assetProvider.Release(_assetReferenceContainer.LoadingScreen);
                if(_cancellationToken.IsCancellationRequested) return;
                
                _loadingScreen = _instantiateService.Instantiate(loadingScreenPrefab);    
            }
            
            _loadingScreen.Activate();
            await _sceneLoadService.LoadSceneAsync(sceneToLoad);
            if(_cancellationToken.IsCancellationRequested) return;
            _loadingScreen.Deactivate();
            
            ParentMachine.TranslateTo<GameLoopState>();
        }
    }
}