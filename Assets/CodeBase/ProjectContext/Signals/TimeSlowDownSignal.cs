namespace CodeBase.ProjectContext.Signals {
    public class TimeSlowDownSignal {
        public float TimeScale { get; }

        public TimeSlowDownSignal(float timeScale) {
            TimeScale = timeScale;
        }
    }
}