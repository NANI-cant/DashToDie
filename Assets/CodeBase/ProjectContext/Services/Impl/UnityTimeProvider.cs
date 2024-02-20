using System;
using CodeBase.ProjectContext.Signals;
using Zenject;

namespace CodeBase.ProjectContext.Services.Impl {
    public class UnityTimeProvider : ITimeProvider, IFixedTimeProvider {
        private readonly SignalBus _signalBus;
        
        public float TimeScale => UnityEngine.Time.timeScale;

        public float Time => UnityEngine.Time.time;
        public float DelaTime => UnityEngine.Time.deltaTime;
        public float UnscaledTime => UnityEngine.Time.unscaledTime;
        public float UnscaledDelaTime => UnityEngine.Time.unscaledDeltaTime;

        public float FixedTime => UnityEngine.Time.fixedTime;
        public float FixedDelaTime => UnityEngine.Time.fixedDeltaTime;
        public float FixedUnscaledTime => UnityEngine.Time.fixedUnscaledTime;
        public float FixedUnscaledDelaTime => UnityEngine.Time.fixedUnscaledDeltaTime;

        public UnityTimeProvider(SignalBus signalBus) {
            _signalBus = signalBus;
        }
        
        public void SpeedUp(float scale) {
            if (scale < 0) throw new ArgumentOutOfRangeException();
            UnityEngine.Time.fixedDeltaTime *= scale;
            UnityEngine.Time.timeScale *= scale;
            
            _signalBus.Fire(new TimeSpeedUpSignal(TimeScale));
        }

        public void SlowDown(float scale) {
            if (scale < 0) throw new ArgumentOutOfRangeException();
            UnityEngine.Time.fixedDeltaTime /= scale;
            UnityEngine.Time.timeScale /= scale;
            
            _signalBus.Fire(new TimeSlowDownSignal(TimeScale));
        }
    }
}