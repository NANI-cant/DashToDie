using System;

namespace CodeBase.Gameplay.General.Cooldowning {
    public interface ICooldown {
        float WindUpTime { get; }
        float Reminded { get; }

        event Action TimesUp;

        void WindUp();
        void Tick(float deltaTime);
    }
}