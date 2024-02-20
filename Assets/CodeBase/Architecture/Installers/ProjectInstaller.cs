using System.Threading;
using CodeBase.Gameplay.PlayerStats.Impl;
using CodeBase.ProjectContext.GameStateMachine;
using CodeBase.ProjectContext.Services.Impl;
using CodeBase.ProjectContext.Signals;
using CodeBase.UI;
using CodeBase.UI.Impl;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CodeBase.Architecture.Installers {
    public class ProjectInstaller: MonoInstaller {
        [SerializeField] [Min(0)] private int _randomSeed = 0;
        [SerializeField] private AssetReferenceContainer _assetReferenceContainer;
        
        [Header("Debug")]
        [SerializeField] [Min(0)] private int _startLevel = 0;

        public override void InstallBindings() {
            Container.BindService<UnityTimeProvider>();
            Container.BindService<UnityDestroyProvider>();
            Container.BindService<ZenjectInstantiateService>();
            Container.BindService<AddressablesAssetProvider>();
            Container.BindService<RandomService>();
            Container.BindService<AddressablesSceneLoadService>();
            Container.BindService<DefaultInputService>();
            Container.BindService<LevelProgress>();
            Container.BindService<PlayerStatsContainer>();
            
            SignalBusInstaller.Install(Container);

            Container
                .Bind<CancellationToken>()
                .FromInstance(this.GetCancellationTokenOnDestroy())
                .AsSingle()
                .NonLazy();

            Container
                .BindInterfacesAndSelfTo<AssetReferenceContainer>()
                .FromInstance(_assetReferenceContainer)
                .AsSingle()
                .NonLazy();

            Container
                .BindInstance(_randomSeed)
                .WhenInjectedInto<RandomService>()
                .NonLazy();
            
            Container.BindService<GameStateMachine>();
            Container.BindState<BootState, GameStateMachine>();
            Container.BindState<SceneLoadState, GameStateMachine>();
            Container.BindState<GameLoopState, GameStateMachine>();

            DeclareSignals();
            DebugBind();
        }

        private void DeclareSignals() {
            Container.DeclareSignal<TimeSpeedUpSignal>().OptionalSubscriber();
            Container.DeclareSignal<TimeSlowDownSignal>().OptionalSubscriber();
        }

        private void DebugBind() {
            Container.BindInstance(_startLevel).WhenInjectedInto<LevelProgress>();
        }
    }
}