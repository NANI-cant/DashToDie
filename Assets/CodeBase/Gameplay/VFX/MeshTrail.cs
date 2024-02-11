using System.Collections.Generic;
using CodeBase.Utils.ObjectPooling;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.VFX {
    public class MeshTrail: MonoBehaviour {
        [SerializeField] [Min(0f)] private float _lifeTime = 1;
        [SerializeField] [Min(0f)] private float _spacing = 0.1f;
        [SerializeField] [GradientUsage(true)] private Gradient _headToTailColor;
        [SerializeField] private Material _material;
        [SerializeField] private SkinnedMeshRenderer[] _meshRenderers;

        private IObjectPoolFactory _objectPoolFactory;
        
        private Transform _transform;
        private Vector3 _lastPosition;
        private ObjectPool _trailPartPool;

        private readonly List<GameObject> _trailParticles = new();

        [Inject]
        public void Construct(IObjectPoolFactory objectPoolFactory) => _objectPoolFactory = objectPoolFactory;
        
        private void Awake() => _transform = transform;
        
        private void Start() {
            _lastPosition = _transform.position;
            PrepareObjectPool();
        }

        private void LateUpdate() {
            if (enabled) {
                float distance = Vector3.Distance(_transform.position, _lastPosition);
                if(Mathf.Approximately(distance,0)) return;
                Vector3 direction = _lastPosition.DirectionTo(_transform.position);
                
                int partsCount = (int) (distance / _spacing);
                if (partsCount == 0) partsCount++;
                for (int i = 0; i < partsCount; i++) {
                    Vector3 partPosition = _lastPosition + direction * i * _spacing;
                    Quaternion partRotation = _transform.rotation;

                    var trailPart = _trailPartPool.Get();
                    InitializePart(trailPart, partPosition, partRotation);
                }
            }
            
            PaintTrail();

            _lastPosition = _transform.position;
        }

        private void PaintTrail() {
            if(_trailParticles.Count == 0) return;
            
            for (int i = 0; i < _trailParticles.Count; i++) {
                var renderers = _trailParticles[_trailParticles.Count - 1 - i].GetComponentsInChildren<Renderer>();
                foreach (var rend in renderers) {
                    float progress = Mathf.InverseLerp(0, _trailParticles.Count - 1, i);
                    rend.material.SetColor("_Color", _headToTailColor.Evaluate(progress));    
                }
            }
        }

        private void PrepareObjectPool() {
            GameObject trailPartTemplate = new GameObject("TrailPart");
            foreach (var meshRenderer in _meshRenderers) {
                GameObject meshPart = new GameObject(meshRenderer.gameObject.name);
                meshPart.transform.SetParent(trailPartTemplate.transform);

                var partRenderer = meshPart.AddComponent<MeshRenderer>();
                meshPart.AddComponent<MeshFilter>();

                partRenderer.material = _material;
            }

            GameObject trailContainer = new GameObject("TrailContainer");
            _trailPartPool = _objectPoolFactory.Create(50, trailPartTemplate, false, trailContainer.transform);
        }

        private void InitializePart(GameObject part, Vector3 position, Quaternion rotation) {
            part.transform.SetPositionAndRotation(position, rotation);
            var partFilters = part.GetComponentsInChildren<MeshFilter>();
            
            for (int i = 0; i < _meshRenderers.Length; i++) {
                var bakedMesh = new Mesh();
                _meshRenderers[i].BakeMesh(bakedMesh);
                partFilters[i].mesh = bakedMesh;
            }
            
            _trailParticles.Add(part);
            DestroyPart(part, _lifeTime).Forget();
        }

        private async UniTaskVoid DestroyPart(GameObject part, float delay) {
            await UniTask.WaitForSeconds(delay, false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            _trailParticles.Remove(part);
            _trailPartPool.Return(part);
        }
    }
}
