using UnityEngine;

namespace CodeBase.Map.Impl {
    public class MapObject: MonoBehaviour, IMapObject {
        [SerializeField] private Scheme _takenPlace;
        [SerializeField] private Scheme _safeArea;

        public Scheme TakenPlace => _takenPlace;
        public Scheme SafeArea => _safeArea;
        
#if UNITY_EDITOR
        [ContextMenu("Calculate Safe Area")]
        public void CalculateSafeArea() {
            _safeArea = new Scheme();
            foreach (var cell in _takenPlace.Cells) {
                for (int i = -1; i <= 1; i++) {
                    for (int j = -1; j <= 1; j++) {
                        _safeArea.Add(cell + new Vector3Int(i, 0, j));
                    }    
                }
            }
        }
        
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            foreach (var cell in _takenPlace.Cells) {
                Gizmos.DrawWireCube(cell + Vector3.up * 0.25f, new Vector3(1, 0.5f, 1));
            }
            
            Gizmos.color = Color.green;
            foreach (var cell in _safeArea.Cells) {
                Gizmos.DrawWireCube(cell + Vector3.up * 0.25f, new Vector3(1, 0.5f, 1));
            }
        }  
#endif
    }
}