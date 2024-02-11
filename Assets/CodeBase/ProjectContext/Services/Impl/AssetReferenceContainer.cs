using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CodeBase.ProjectContext.Services.Impl {
    [CreateAssetMenu(fileName = nameof(AssetReferenceContainer), menuName = nameof(AssetReferenceContainer))]
    public class AssetReferenceContainer : ScriptableObject, IAssetReferenceContainer {
        [SerializeField] private AssetReference _player;
        
        [Header("Configs")]
        [SerializeField] private AssetReference _playerConfig;
        [SerializeField] private AssetReference _mapConfig;
        [SerializeField] private AssetReference _progressConfig;
        
        [Header("Stats")]
        [SerializeField] private AssetReference[] _playerStats;
        
        [Header("Map Objects")]
        [SerializeField] private AssetReference _navMeshSurface;
        [SerializeField] private AssetReference _border;
        [SerializeField] private AssetReference _floor;
        [SerializeField] private AssetReference[] _walls;
        [SerializeField] private AssetReference _exit;
        
        [Header("Collectables")]
        [SerializeField] private AssetReference _boosterSpawner;
        
        [Header("Scenes")]
        [SerializeField] private AssetReference _bootScene;
        [SerializeField] private AssetReference _gameplayScene;
        [SerializeField] private AssetReference _mainMenuScene;
        
        [Header("Enemies")]
        [SerializeField] private AssetReference[] _enemies;

        [Header("UI")]
        [SerializeField] private AssetReference _loadingScreen;
        [SerializeField] private AssetReference[] _screens;

        public AssetReference Border => _border;
        public AssetReference Floor => _floor;
        public AssetReference MapConfig => _mapConfig;
        public AssetReference[] Walls => _walls;
        public AssetReference BootScene => _bootScene;
        public AssetReference GameplayScene => _gameplayScene;
        public AssetReference MainMenuScene => _mainMenuScene;
        public AssetReference Player => _player;
        public AssetReference[] Enemies => _enemies;
        public AssetReference PlayerConfig => _playerConfig;
        public AssetReference NavMeshSurface => _navMeshSurface;
        public AssetReference[] Screens => _screens;
        public AssetReference LoadingScreen => _loadingScreen;
        public AssetReference[] PlayerStats => _playerStats;
        public AssetReference BoosterSpawner => _boosterSpawner;

        public AssetReference ProgressConfig => _progressConfig;

        public AssetReference Exit => _exit;
    }
}