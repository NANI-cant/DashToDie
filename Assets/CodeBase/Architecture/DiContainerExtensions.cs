using CodeBase.Utils.StateMachine;
using Zenject;

namespace CodeBase.Architecture {
    public static class DiContainerExtensions {
        public static void BindService<TService>(this DiContainer container) {
            container
                .BindInterfacesAndSelfTo<TService>()
                .AsSingle()
                .NonLazy();
        }

        public static void BindState<TState, TStateMachine>(this DiContainer container) 
            where TState : AState 
            where TStateMachine: AStateMachine {
            container
                .Bind<AState>()
                .To<TState>()
                .WhenInjectedInto<TStateMachine>()
                .NonLazy();
        }
    }
}