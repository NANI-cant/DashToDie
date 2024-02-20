using System;

namespace CodeBase.Gameplay.General {
    public interface IHealth {
        public int MaxHealth { get; }
        public int HealthPoints { get; }

        event Action Changed;
        event Action Increased;
        event Action Decreased;
        
        public void IncreaseHealth(int increasing);
        public void DecreaseHealth(int decreasing);
    }
}