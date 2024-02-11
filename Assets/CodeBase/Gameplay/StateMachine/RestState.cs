using System.Threading;
using CodeBase.Map;
using CodeBase.Map.Building;
using CodeBase.ProjectContext.GameStateMachine;
using CodeBase.ProjectContext.Services;
using CodeBase.Utils.StateMachine;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Gameplay.StateMachine {
    public class RestState: AState {
        private readonly ILevelProgress _levelProgress;
        private readonly IAssetReferenceContainer _assetReferenceContainer;
        private readonly IAssetProvider _assetProvider;
        private readonly Map.Map _map;
        private readonly MapPlacer _mapPlacer;
        private readonly IInstantiateService _instantiateService;
        private readonly GameStateMachine _gameStateMachine;
        private readonly CancellationToken _cancelToken;
        private Exit _exit;

        public RestState(
            ILevelProgress levelProgress,
            IAssetReferenceContainer assetReferenceContainer,
            IAssetProvider assetProvider,
            Map.Map map,
            MapPlacer mapPlacer,
            IInstantiateService instantiateService,
            GameStateMachine gameStateMachine,
            CancellationToken cancelToken
        ) {
            _levelProgress = levelProgress;
            _assetReferenceContainer = assetReferenceContainer;
            _assetProvider = assetProvider;
            _map = map;
            _mapPlacer = mapPlacer;
            _instantiateService = instantiateService;
            _gameStateMachine = gameStateMachine;
            _cancelToken = cancelToken;
        }
        
        public override void Enter(object payload = null) {
            _levelProgress.PassLevel();

            CreateExit().Forget();
        }

        public override void Exit() {
            if (_exit != null) _exit.PlayerExited -= NextStage;
        }

        private async UniTaskVoid CreateExit() {
            var exitPrefab = await _assetProvider.LoadAsset<GameObject>(_assetReferenceContainer.Exit);
            _assetProvider.Release(_assetReferenceContainer.Exit);
            if(_cancelToken.IsCancellationRequested) return;

            var exitObject = _instantiateService.Instantiate(exitPrefab);
            _mapPlacer.Place(exitObject, _map.AvailableCells[0], _map);

            _exit = exitObject.GetComponent<Exit>();
            _exit.PlayerExited += NextStage;
        }

        private void NextStage() {
            _gameStateMachine.TranslateTo<SceneLoadState>(_assetReferenceContainer.GameplayScene);
        }
    }
}