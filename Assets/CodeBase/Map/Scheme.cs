using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Map {
    [Serializable]
    public struct Scheme {
        [SerializeField] public Vector3Int Position;
        [SerializeField] public Quaternion Rotation;
        [SerializeField] private List<Vector3Int> _cells;

        public Vector3Int[] Cells {
            get {
                List<Vector3Int> globalCells = new();

                if (_cells == null) return globalCells.ToArray();
                foreach (var cell in _cells) {
                    globalCells.Add((Rotation * cell).Round());
                }

                for (int i = 0; i < globalCells.Count; i++) {
                    globalCells[i] += Position;
                }

                return globalCells.ToArray();
            }
        }

        public Scheme(Vector3Int position, IEnumerable<Vector3Int> globalCells) {
            _cells = new List<Vector3Int>();
            Position = position;
            Rotation = Quaternion.identity;
            foreach (var cell in globalCells) {
                _cells.Add(cell - position);
            }
        }

        public void Add(Vector3Int cell) {
            _cells ??= new List<Vector3Int>();
            if (_cells.Contains(cell - Position)) return;
            
            _cells.Add(cell - Position);
        }

        public void Remove(Vector3Int cell) {
            _cells?.Remove(cell - Position);
        }
    }
}