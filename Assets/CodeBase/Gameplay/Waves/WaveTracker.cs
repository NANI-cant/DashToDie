using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Gameplay.General;

namespace CodeBase.Gameplay.Waves {
    public class WaveTracker {
        private readonly Dictionary<IHealth, HealthTracker> _trackers = new();
        
        public int Reminded { get; private set; }
        
        public event Action Cleared;
        
        public void Track(params IHealth[] healthTargets) {
            foreach (var healthTarget in healthTargets) {
                HealthTracker tracker = new HealthTracker(healthTarget);
                tracker.ChangedDetected += TryProcessWave;
                
                _trackers.Add(healthTarget, tracker);
                
                if (healthTarget.HealthPoints > 0) Reminded++;
            }
        }
        
        public void Untrack(params IHealth[] healthTargets) {
            foreach (var healthTarget in healthTargets) {
                var tracker = _trackers[healthTarget];
                tracker.ChangedDetected -= TryProcessWave;
                _trackers.Remove(healthTarget);
            }
        }

        public void UnTrackAll() => Untrack(_trackers.Keys.ToArray());

        public void Clear() {
            foreach (var health in _trackers.Keys) {
                health.DecreaseHealth(health.HealthPoints);
            }
        }

        private void TryProcessWave(HealthTracker tracker, int was, int now) {
            if (was > 0 && now == 0) Reminded--;
            if (was == 0 && now > 0) Reminded++;
            
            if(Reminded <= 0) Cleared?.Invoke();
        }

        private class HealthTracker {
            private int _lastPoints;
            
            public IHealth Health { get; }

            public event Action<HealthTracker, int, int> ChangedDetected;

            public HealthTracker(IHealth health) {
                Health = health;
                _lastPoints = Health.HealthPoints;
                Health.Changed += OnChanged;
            }

            ~HealthTracker() {
                Health.Changed -= OnChanged;
            }

            private void OnChanged() {
                ChangedDetected?.Invoke(this, _lastPoints, Health.HealthPoints);
                _lastPoints = Health.HealthPoints;
            }
        }
    }
}