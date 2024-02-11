using System;
using System.Collections.Generic;

namespace CodeBase.Utils.StateMachine {
    public abstract class AStateMachine {
        private readonly Dictionary<Type, AState> _states = new();
        private AState _activeState;

        protected AStateMachine(AState[] states) {
            _states = new();
            foreach (var state in states) {
                _states.Add(state.GetType(), state);
                state.ParentMachine = this;
            }
        }

        public void TranslateTo<TState>(object payload = null) where TState: AState {
            _activeState?.Exit();
            _activeState = _states[typeof(TState)];
            _activeState?.Enter(payload);
        }
    }
}