using System;

namespace CodeBase.ProjectContext.Services {
    public interface ILevelProgress {
        int CurrentLevel { get; }
        int WavesReminded { get; }

        void PassLevel();
        void PassWave();
    }
}