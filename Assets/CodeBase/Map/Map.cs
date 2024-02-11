using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Map {
    public class Map {
        private readonly List<Vector3Int> _availableCells = new();

        public IReadOnlyList<Vector3Int> AvailableCells => _availableCells;
        public Vector3Int Size { get; set; }
        public Vector3Int Extends { get; set; }
        
        private Dictionary<Vector3Int, GameObject> TakenCells { get; } = new();
        private Dictionary<GameObject, List<Vector3Int>> PlacedObjects { get; } = new();

        public void Initialize(int xLength, int zLength) {
            if (zLength % 2 == 1) zLength -= 1;
            if (xLength % 2 == 1) xLength -= 1;
            
            Size = new Vector3Int(xLength, 0, zLength);
            Extends = new Vector3Int(xLength / 2, 0, zLength / 2);
            
            for (int z = -Extends.z; z <= Extends.z; z++) {
                for (int x = -Extends.x; x <= Extends.z; x++) {
                    _availableCells.Add(new Vector3Int(x, 0, z));
                }
            }
        }

        public bool CanFit(params Vector3Int[] scheme) {
            foreach (var cell in scheme) {
                if (TakenCells.ContainsKey(cell)) return false;
            }
            
            return true;
        }

        public void TakeCells(GameObject gameObject, params Vector3Int[] cells) {
            if (!CanFit()) throw new ArgumentOutOfRangeException($"Can't fit object");
            
            foreach (var cell in cells) {
                _availableCells.Remove(cell);
                TakenCells[cell] = gameObject;
            }

            PlacedObjects[gameObject] = new List<Vector3Int>(cells);
        }

        public void FreeCells(params GameObject[] gameObjects) {
            foreach (var gameObject in gameObjects) {
                if (!PlacedObjects.ContainsKey(gameObject)) continue;
                
                foreach (var takenCell in PlacedObjects[gameObject]) {
                    if (!_availableCells.Contains(takenCell)) _availableCells.Add(takenCell);
                    if (TakenCells.ContainsKey(takenCell)) TakenCells.Remove(takenCell);            
                }
            }
        }
    }
}