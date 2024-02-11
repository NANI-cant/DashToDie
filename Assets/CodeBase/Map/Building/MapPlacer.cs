using System;
using UnityEngine;

namespace CodeBase.Map.Building {
    public class MapPlacer {
        public bool CheckCanPlace(GameObject gameObject, Vector3Int position, Map map, ObstacleRotation rotation = ObstacleRotation.Angle0) {
            IMapObject mapObject = gameObject.GetComponent<IMapObject>();
            Scheme safeAreaScheme = mapObject.SafeArea;
            Quaternion quaternionRotation = Quaternion.AngleAxis((int)rotation, Vector3.up);
            
            safeAreaScheme.Position = position;
            safeAreaScheme.Rotation = quaternionRotation;
            
            return map.CanFit(safeAreaScheme.Cells);
        }

        public void Place(GameObject gameObject, Vector3Int position, Map map, ObstacleRotation rotation = ObstacleRotation.Angle0) {
            if(gameObject.IsPrefab())
                throw new ArgumentOutOfRangeException($"{gameObject.name} must't be a prefab");
            if (!CheckCanPlace(gameObject, position, map, rotation))
                throw new ArgumentOutOfRangeException($"Map can't place {gameObject.name} in {position} with {rotation}");
            
            IMapObject mapObject = gameObject.GetComponent<IMapObject>();
            Quaternion quaternionRotation = Quaternion.AngleAxis((int)rotation, Vector3.up);

            gameObject.transform.position = position;
            gameObject.transform.rotation = quaternionRotation;
            
            Scheme boundsScheme = mapObject.TakenPlace;
            boundsScheme.Position = position;
            boundsScheme.Rotation = quaternionRotation;
            
            map.TakeCells(gameObject, boundsScheme.Cells);
        }

        public enum ObstacleRotation {
            Angle0 = 0,
            Angle90 = 90,
            Angle180 = 180,
            Angle270 = 270,
        }
    }
}