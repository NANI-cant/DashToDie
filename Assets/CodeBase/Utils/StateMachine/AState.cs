using Cysharp.Threading.Tasks;

namespace CodeBase.Utils.StateMachine {
    public class AState {
        public AStateMachine ParentMachine { get; set; }

        public virtual void Exit() {
            
        }

        public virtual void Enter(object payload = null) {
            
        }
    }
}