namespace CodeBase.ProjectContext.Services {
    public interface IFixedTimeProvider {
        float FixedTime { get; }
        float FixedDelaTime { get; }
        float FixedUnscaledTime { get; }
        float FixedUnscaledDelaTime { get; }
    }
}