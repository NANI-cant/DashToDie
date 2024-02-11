using CodeBase.Utils.StateMachine;
using Zenject;

namespace CodeBase.Gameplay.StateMachine {
    public class GameLoopStateMachine: AStateMachine, IInitializable {
        public GameLoopStateMachine(params AState[] states) : base(states) {
            
        }

        public void Initialize() {
            TranslateTo<StartState>();
        }
    }
}