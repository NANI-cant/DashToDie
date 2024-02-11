using CodeBase.ProjectContext.Services;
using UnityEngine;

namespace CodeBase.Map.Building {
    public class PlayerPlacer {
        private readonly IRandomService _randomService;
        private readonly MapPlacer _mapPlacer;
        private readonly Map _map;

        public PlayerPlacer(
            Map map,
            IRandomService randomService, 
            MapPlacer mapPlacer
        ) {
            _map = map;
            _randomService = randomService;
            _mapPlacer = mapPlacer;
        }

        public void Place(GameObject playerObject) {
            Vector3Int pickedPosition = _map.AvailableCells[_randomService.Range(0, _map.AvailableCells.Count)];

            _mapPlacer.Place(playerObject, pickedPosition, _map);
        }
    }
}