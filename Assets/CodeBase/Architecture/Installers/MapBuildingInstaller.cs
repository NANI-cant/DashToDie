using CodeBase.Map;
using CodeBase.Map.Building;
using UnityEngine;
using Zenject;

namespace CodeBase.Architecture.Installers {
    public class MapBuildingInstaller: MonoInstaller {
        [SerializeField] private Transform _geometryRoot;
        
        public override void InstallBindings() {
            Container.BindService<MapBuilder>();
            Container.BindService<PlayerPlacer>();
            Container.BindService<EnemiesPlacer>();
            Container.BindService<BoostersPlacer>();
            Container.BindService<MapPlacer>();
            Container.BindService<Map.Map>();

            Container
                .BindInstance(_geometryRoot)
                .AsSingle()
                .NonLazy();
        }
    }
}