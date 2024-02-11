using System;
using System.Threading;
using Cinemachine;
using CodeBase.Configs;
using CodeBase.Gameplay.Player;
using CodeBase.Gameplay.Progress;
using CodeBase.Map;
using CodeBase.Map.Building;
using CodeBase.ProjectContext.Services;
using CodeBase.UI;
using CodeBase.UI.Screens;
using CodeBase.Utils.StateMachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.StateMachine {
    public class StartState: AState {
        private readonly MapBuilder _mapBuilder;
        private readonly PlayerPlacer _playerPlacer;
        private readonly DiContainer _diContainer;
        private readonly BoostersPlacer _boostersPlacer;
        private readonly IUiService _uiService;
        private readonly IAssetProvider _assetProvider;
        private readonly IAssetReferenceContainer _assetReferenceContainer;
        private readonly ILevelProgress _levelProgress;
        private readonly CancellationToken _cancelToken;
        private readonly IPlayerFactory _playerFactory;
        private readonly CinemachineVirtualCamera _followPlayerVCam;
        private readonly Map.Map _map;
        
        private IMapConfig _mapConfig;

        public StartState(
            MapBuilder mapBuilder,
            PlayerPlacer playerPlacer,
            DiContainer diContainer,
            BoostersPlacer boostersPlacer,
            Map.Map map,
            IUiService uiService,
            IAssetProvider assetProvider,
            IAssetReferenceContainer assetReferenceContainer,
            ILevelProgress levelProgress,
            CancellationToken cancelToken,
            IPlayerFactory playerFactory,
            CinemachineVirtualCamera followPlayerVCam
        ) {
            _mapBuilder = mapBuilder;
            _playerPlacer = playerPlacer;
            _diContainer = diContainer;
            _boostersPlacer = boostersPlacer;
            _map = map;
            _uiService = uiService;
            _assetProvider = assetProvider;
            _assetReferenceContainer = assetReferenceContainer;
            _levelProgress = levelProgress;
            _cancelToken = cancelToken;
            _playerFactory = playerFactory;
            _followPlayerVCam = followPlayerVCam;
        }
        
        public override void Enter(object payload = null) => EnterAsync().Forget();

        private async UniTaskVoid EnterAsync() {
            if (!await TrySetupMap()) return;
            if (!await TrySetupPlayer()) return;
            await InitializeUI();
            
            ParentMachine.TranslateTo<WaveState>();
        }

        private async UniTask<bool> TrySetupMap() {
            _mapConfig = await _assetProvider.LoadAsset<IMapConfig>(_assetReferenceContainer.MapConfig);
            _assetProvider.Release(_assetReferenceContainer.MapConfig);
            if (_mapConfig == null) throw new ArgumentNullException();
            if (_cancelToken.IsCancellationRequested) return false;

            _map.Initialize(
                (int) ProgressRule.Linear(_mapConfig.Width, _mapConfig.WidthMultiplier, _levelProgress.CurrentLevel),
                (int) ProgressRule.Linear(_mapConfig.Height, _mapConfig.HeightMultiplier, _levelProgress.CurrentLevel)
            );
            await _mapBuilder.Build(_map);
            if (_cancelToken.IsCancellationRequested) return false;

            await _boostersPlacer.Place((int) ProgressRule.Linear(_mapConfig.Boosters, _mapConfig.BoostersMultiplier, _levelProgress.CurrentLevel));
            if (_cancelToken.IsCancellationRequested) return false;
            
            return true;
        }

        private async UniTask<bool> TrySetupPlayer() {
            var playerObject = await _playerFactory.Create();
            if (_cancelToken.IsCancellationRequested) return false;
            
            _playerPlacer.Place(playerObject);
            _diContainer
                .Bind<GameObject>()
                .FromInstance(playerObject)
                .AsSingle()
                .NonLazy();

            _followPlayerVCam.Follow = playerObject.transform;
            _followPlayerVCam.LookAt = playerObject.transform;

            return true;
        }

        private async UniTask InitializeUI() {
            await _uiService.Initialize();
            if(_cancelToken.IsCancellationRequested) return;
            
            await _uiService.Open<Hud>();
        }
    }
}