using UnityEngine.AddressableAssets;

namespace CodeBase.ProjectContext.Services {
    public interface IAssetReferenceContainer {
        AssetReference Border { get; }
        AssetReference Floor { get; }
        AssetReference MapConfig { get; }
        AssetReference[] Walls { get; }
        AssetReference BootScene { get; }
        AssetReference GameplayScene { get; }
        AssetReference MainMenuScene { get; }
        AssetReference Player { get; }
        AssetReference[] Enemies { get; }
        AssetReference PlayerConfig { get; }
        AssetReference NavMeshSurface { get; }
        AssetReference[] Screens { get; }
        AssetReference LoadingScreen { get; }
        AssetReference[] PlayerStats { get; }
        AssetReference BoosterSpawner { get; }
        AssetReference ProgressConfig { get; }
        AssetReference Exit { get; }
    }
}