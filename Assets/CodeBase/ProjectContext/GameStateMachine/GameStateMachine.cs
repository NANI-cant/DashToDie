using CodeBase.Utils.StateMachine;
using Zenject;

namespace CodeBase.ProjectContext.GameStateMachine {
    public class GameStateMachine: AStateMachine, IInitializable {
        public GameStateMachine(params AState[] states) : base(states) {
            
        }

        public void Initialize() {
            TranslateTo<BootState>();
        }
    }
}