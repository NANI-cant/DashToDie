using UnityEngine;

namespace CodeBase.Configs.Impl {
    [CreateAssetMenu(fileName = "MapConfig", menuName = "Configs/Map")]
    public class MapConfig : ScriptableObject, IMapConfig {
        [Header("Base")]
        [SerializeField][Min(2)] private int _height = 2;
        [SerializeField][Min(2)] private int _width = 2;
        [SerializeField][Min(1)] private int _waves;
        [SerializeField][Min(1)] private int _enemiesOnWaveOnWave;
        [SerializeField][Min(1)] private int _boosters = 1;
        
        [Header("Multipliers")]
        [SerializeField][Min(0)] private float _heightMultiplier = 1;
        [SerializeField][Min(0)] private float _widthMultiplier = 1;
        [SerializeField][Min(0)] private float _wavesMultiplier = 1;
        [SerializeField][Min(0)] private float _enemiesOnWaveMultiplier = 1;
        [SerializeField][Min(0)] private float _boostersMultiplier;


        public int Height => _height;
        public int Width => _width;
        public int Boosters => _boosters;
        public int EnemiesOnWave => _enemiesOnWaveOnWave;
        public int Waves => _waves;

        public float HeightMultiplier => _heightMultiplier;
        public float WidthMultiplier => _widthMultiplier;
        public float BoostersMultiplier => _boostersMultiplier;
        public float WavesMultiplier => _wavesMultiplier;
        public float EnemiesOnWaveMultiplier => _enemiesOnWaveMultiplier;

#if UNITY_EDITOR
        private void OnValidate() {
            if (_height % 2 == 1) _height -= 1;
            if (_width % 2 == 1) _width -= 1;
        }
#endif
    }
}