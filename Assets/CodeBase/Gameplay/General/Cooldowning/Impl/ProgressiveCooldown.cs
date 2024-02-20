using System;
using CodeBase.Gameplay.ProgressiveValues;

namespace CodeBase.Gameplay.General.Cooldowning.Impl {
    public class ProgressiveCooldown : ICooldown { 
        private ProgressiveFloat _time;

        public float WindUpTime => _time.Value;
        public bool IsTimesUp => Reminded == 0;
        public float Reminded { get; private set; }

        public ProgressiveFloat Time => _time;

        public event Action TimesUp;

        public ProgressiveCooldown(float time) {
            _time = new ProgressiveFloat(time);
        }

        public void WindUp() {
            Reminded = WindUpTime;
        }

        public void Tick(float deltaTime) {
            Reminded -= deltaTime;
            Reminded = Math.Clamp(Reminded, 0, WindUpTime);
            
            if(Reminded == 0) TimesUp?.Invoke();
        }
    }
}