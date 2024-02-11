namespace CodeBase.ProjectContext.Services {
    public interface ITimeProvider {
        float TimeScale { get; }
        float Time { get; }
        float DelaTime { get; }
        float UnscaledTime { get; }
        float UnscaledDelaTime { get; }

        void SpeedUp(float scale);
        void SlowDown(float scale);
    }
}