using System;

namespace CodeBase.Gameplay.General.Cooldowning.Impl {
    public class Cooldown : ICooldown {
        public float WindUpTime { get; }
        public float Reminded { get; private set; }
        public bool IsTimesUp => Reminded == 0;

        public event Action TimesUp;

        public Cooldown(float time) {
            WindUpTime = time;
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