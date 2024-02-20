namespace CodeBase.ProjectContext.Signals {
    public class TimeSpeedUpSignal {
        public float TimeScale { get; }

        public TimeSpeedUpSignal(float timeScale) {
            TimeScale = timeScale;
        }
    }
}