using System.Threading;
using Cinemachine;
using CodeBase.Gameplay.Enemies.Signals;
using CodeBase.Gameplay.Player;
using CodeBase.Gameplay.Player.Signals;
using CodeBase.Gameplay.Skills.Factory.Impl;
using CodeBase.Gameplay.StateMachine;
using CodeBase.Gameplay.Waves;
using CodeBase.Metaplay;
using CodeBase.ProjectContext.Services.Impl;
using CodeBase.UI.Impl;
using CodeBase.Utils.ObjectPooling.Impl;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CodeBase.Architecture.Installers {
    public class GameplayInstaller : MonoInstaller {
        [SerializeField] private Canvas _uiRoot;
        [SerializeField] private CinemachineVirtualCamera _followVCam;
        
        public override void InstallBindings() {
            Container.BindService<ZenjectInstantiateService>();
            Container.BindService<ObjectPoolFactory>();
            Container.BindService<UiService>();
            Container.BindService<SkillFactory>();
            Container.BindService<WaveGenerator>();
            Container.BindService<WaveTracker>();
            Container.BindService<PlayerFactory>();
            
            Container.BindService<MoneyForKillService>();

            Container
                .Bind<CancellationToken>()
                .FromInstance(this.GetCancellationTokenOnDestroy())
                .AsSingle()
                .NonLazy();

            Container
                .BindInstance(_uiRoot)
                .AsSingle()
                .NonLazy();

            Container
                .BindInstance(_followVCam)
                .AsSingle()
                .NonLazy();

            Container.BindService<GameLoopStateMachine>();
            Container.BindState<StartState, GameLoopStateMachine>();
            Container.BindState<WaveState, GameLoopStateMachine>();
            Container.BindState<RestState, GameLoopStateMachine>();
            Container.BindState<LoseState, GameLoopStateMachine>();
            
            DeclareSignals();
        }

        private void DeclareSignals() {
            Container.DeclareSignal<EnemyDiedSignal>()
                .OptionalSubscriber();
            Container.DeclareSignal<PlayerDiedSignal>()
                .OptionalSubscriber();
            Container.DeclareSignal<PlayerHurtedSignal>()
                .OptionalSubscriber();
        }
    }
}