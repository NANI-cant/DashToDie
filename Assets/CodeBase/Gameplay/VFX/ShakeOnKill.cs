using CodeBase.Gameplay.Enemies.Signals;
using Zenject;

namespace CodeBase.Gameplay.VFX {
    public class ShakeOnKill : CameraShake {
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }
        
        private void OnEnable() {
            _signalBus.Subscribe<EnemyDiedSignal>(Shake);
        }

        protected override void OnDisable() {
            base.OnDisable();
            _signalBus.Unsubscribe<EnemyDiedSignal>(Shake);
        }
    }
}