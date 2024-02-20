using CodeBase.Gameplay.Player.Signals;
using Zenject;

namespace CodeBase.Gameplay.VFX {
    public class ShakeOnHurt : CameraShake {
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }
        
        private void OnEnable() {
            _signalBus.Subscribe<PlayerHurtedSignal>(Shake);
        }

        protected override void OnDisable() {
            base.OnDisable();
            _signalBus.Unsubscribe<PlayerHurtedSignal>(Shake);
        }
    }
}