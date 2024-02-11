using System;
using System.Threading;
using CodeBase.ProjectContext.Services;
using Cysharp.Threading.Tasks;
using Unity.AI.Navigation;
using UnityEngine;
using IInitializable = Zenject.IInitializable;

namespace CodeBase.Map.Building {
    public class MapBuilder: IInitializable {
        private readonly IAssetProvider _assetProvider;
        private readonly IAssetReferenceContainer _referenceContainer;
        private readonly IInstantiateService _instantiateService;
        private readonly IRandomService _randomService;
        private readonly CancellationToken _cancellationToken;
        private readonly Transform _geometryRoot;
        private readonly MapPlacer _mapPlacer;

        private GameObject _borderPrefab;
        private GameObject _floorPrefab;
        private GameObject[] _wallPrefabs;
        private UniTask<(GameObject, GameObject, GameObject, GameObject[])> _assetsLoading;
        private GameObject _navMeshSurfacePrefab;

        public MapBuilder(
            IAssetProvider assetProvider,
            IAssetReferenceContainer referenceContainer,
            Transform geometryRoot,
            IInstantiateService instantiateService,
            IRandomService randomService,
            CancellationToken cancellationToken,
            MapPlacer mapPlacer
        ) {
            _assetProvider = assetProvider;
            _referenceContainer = referenceContainer;
            _geometryRoot = geometryRoot;
            _instantiateService = instantiateService;
            _randomService = randomService;
            _cancellationToken = cancellationToken;
            _mapPlacer = mapPlacer;
        }

        public void Initialize() {
            _assetsLoading = UniTask.WhenAll(
                _assetProvider.LoadAsset<GameObject>(_referenceContainer.NavMeshSurface),
                _assetProvider.LoadAsset<GameObject>(_referenceContainer.Border),
                _assetProvider.LoadAsset<GameObject>(_referenceContainer.Floor),
                _assetProvider.LoadAssets<GameObject>(_referenceContainer.Walls)
            );
        }

        public async UniTask<GameObject> Build(Map map) {
            (_navMeshSurfacePrefab, _borderPrefab, _floorPrefab, _wallPrefabs) = await _assetsLoading;

            Transform mapContainer = new GameObject("Map").transform;
            mapContainer.parent = _geometryRoot;

            if (_cancellationToken.IsCancellationRequested) {
                ReleaseAssets();
                _cancellationToken.ThrowIfCancellationRequested();
            }
            
            BuildFloor(mapContainer, map);
            BuildBorders(mapContainer, map);
            BuildWalls(mapContainer, map);
            
            var navMeshSurface = _instantiateService.Instantiate(_navMeshSurfacePrefab);
            navMeshSurface.GetComponent<NavMeshSurface>().BuildNavMesh();
            
            ReleaseAssets();
            
            return mapContainer.gameObject;
        }

        private void BuildWalls(Transform mapTransform, Map map) {
            var wallsContainer = new GameObject("Walls").transform;
            wallsContainer.parent = mapTransform;
            Vector3Int startPosition = new Vector3Int(-map.Extends.x, 0, -map.Extends.z);
            int[] angles = {0, 90, 180, 270};

            for (int i = 0; i < map.Size.z; i++) {
                for (int j = 0; j < map.Size.x; j++) {
                    Vector3Int currentPosition = startPosition + new Vector3Int(j, 0, i);
                    
                    GameObject pickedWall = _wallPrefabs[_randomService.Range(0, _wallPrefabs.Length)];
                    Array values = Enum.GetValues(typeof(MapPlacer.ObstacleRotation));
                    MapPlacer.ObstacleRotation pickedAngle = (MapPlacer.ObstacleRotation)values.GetValue(_randomService.Range(0, angles.Length));

                    if (!_mapPlacer.CheckCanPlace(pickedWall, currentPosition, map, pickedAngle)) continue;
                    
                    GameObject builtWall = _instantiateService.Instantiate(pickedWall, wallsContainer);
                    _mapPlacer.Place(builtWall, currentPosition, map, pickedAngle);
                }
            }
        }

        private void BuildFloor(Transform mapTransform, Map map) {
            var floor = _instantiateService.Instantiate(_floorPrefab, mapTransform);
            floor.transform.localScale = new Vector3(map.Size.x, 1, map.Size.z);
        }

        private void BuildBorders(Transform mapTransform, Map map) {
            var bordersContainer = new GameObject("Borders").transform;
            bordersContainer.parent = mapTransform;
            
            var topBorder = _instantiateService.Instantiate(_borderPrefab, bordersContainer);
            topBorder.transform.position = new Vector3(0, 0, map.Extends.z);
            topBorder.transform.localScale = new Vector3(map.Size.x+1, 1, 1);
            
            var bottomBorder = _instantiateService.Instantiate(_borderPrefab, bordersContainer);
            bottomBorder.transform.position = new Vector3(0, 0, -map.Extends.z);
            bottomBorder.transform.localScale = new Vector3(map.Size.x+1, 1, 1);
            
            var leftBorder = _instantiateService.Instantiate(_borderPrefab, bordersContainer);
            leftBorder.transform.position = new Vector3(-map.Extends.x, 0, 0);
            leftBorder.transform.localScale = new Vector3(1, 1, map.Size.z+1);

            var rightBorder = _instantiateService.Instantiate(_borderPrefab, bordersContainer);
            rightBorder.transform.position = new Vector3(map.Extends.x, 0, 0);
            rightBorder.transform.localScale = new Vector3(1, 1, map.Size.z+1);

            for (int x = -map.Extends.x; x <= map.Extends.x; x++) 
                map.TakeCells(topBorder, new Vector3Int(x, 0, map.Extends.z));
            
            for (int x = -map.Extends.x; x <= map.Extends.x; x++) 
                map.TakeCells(bottomBorder, new Vector3Int(x, 0, -map.Extends.z));
            
            for (int z = -map.Extends.z+1; z < map.Extends.z; z++) 
                map.TakeCells(leftBorder, new Vector3Int(-map.Extends.x, 0, z));
            
            for (int z = -map.Extends.z+1; z < map.Extends.z; z++) 
                map.TakeCells(rightBorder, new Vector3Int(map.Extends.x, 0, z));
        }

        private void ReleaseAssets() {
            _assetProvider.Release(_referenceContainer.NavMeshSurface);
            _assetProvider.Release(_referenceContainer.Border);
            _assetProvider.Release(_referenceContainer.Floor);
            _assetProvider.Release(_referenceContainer.MapConfig);
            _assetProvider.Release(_referenceContainer.Walls);
        }
    }
}