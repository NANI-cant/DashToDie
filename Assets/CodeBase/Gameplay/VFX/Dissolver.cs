using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace CodeBase.Gameplay.VFX {
    public class Dissolver: MonoBehaviour {
        [SerializeField] private Renderer[] _renderers;
        [SerializeField] private Material _dissolveMaterial;
        [SerializeField] [Min(0)] private float _duration;

        private const string DissolveOffsetProperty = "_DissolveOffest";
        private readonly Dictionary<Renderer, Material[]> _savedMaterials = new();

        private Sequence _dissolveTween;

        public float Duration => _duration;

        private void Start() {
            foreach (var rend in _renderers) {
                _savedMaterials[rend] = rend.materials;
            }
        }

        private void OnDisable() {
            _dissolveTween?.Complete();
            _dissolveTween?.Kill();
        }

        public void Dissolve() {
            _dissolveTween?.Complete();
            _dissolveTween?.Kill();
            
            _dissolveTween = DOTween.Sequence();
            foreach (var rend in _renderers) {
                rend.material = _dissolveMaterial;
                _dissolveTween
                    .Insert(0, rend.material.DOVector(new Vector4(0, 0, 0), DissolveOffsetProperty, Duration).SetEase(Ease.InCirc));
            }
        }

        public void ReturnToDefault() {
            foreach (var rend in _renderers) {
                rend.materials = _savedMaterials[rend];
            }
        }
    }
}