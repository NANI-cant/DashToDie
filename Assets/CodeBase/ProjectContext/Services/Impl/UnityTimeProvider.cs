using System;

namespace CodeBase.ProjectContext.Services.Impl {
    public class UnityTimeProvider : ITimeProvider, IFixedTimeProvider {
        public float TimeScale => UnityEngine.Time.timeScale;

        public float Time => UnityEngine.Time.time;
        public float DelaTime => UnityEngine.Time.deltaTime;
        public float UnscaledTime => UnityEngine.Time.unscaledTime;
        public float UnscaledDelaTime => UnityEngine.Time.unscaledDeltaTime;

        public float FixedTime => UnityEngine.Time.fixedTime;
        public float FixedDelaTime => UnityEngine.Time.fixedDeltaTime;
        public float FixedUnscaledTime => UnityEngine.Time.fixedUnscaledTime;
        public float FixedUnscaledDelaTime => UnityEngine.Time.fixedUnscaledDeltaTime;
        
        public void SpeedUp(float scale) {
            if (scale < 0) throw new ArgumentOutOfRangeException();
            UnityEngine.Time.fixedDeltaTime *= scale;
            UnityEngine.Time.timeScale *= scale;
        }

        public void SlowDown(float scale) {
            if (scale < 0) throw new ArgumentOutOfRangeException();
            UnityEngine.Time.fixedDeltaTime /= scale;
            UnityEngine.Time.timeScale /= scale;
        }
    }
}