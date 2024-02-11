using System;

namespace CodeBase.Configs {
    public interface IMapConfig {
        int Height { get; }
        int Width { get; }
        int Boosters { get; }
        int EnemiesOnWave { get; }
        int Waves { get; }
        
        float HeightMultiplier { get; }
        float WidthMultiplier { get; }
        float BoostersMultiplier { get; }
        float WavesMultiplier { get; }
        float EnemiesOnWaveMultiplier { get; }
    }
}