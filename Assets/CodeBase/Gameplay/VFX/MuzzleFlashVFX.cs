using UnityEngine;
using DG.Tweening;

namespace CodeBase.Gameplay.VFX {
    public class MuzzleFlashVFX: MonoBehaviour {
        [SerializeField] [Min(0)] private float _flashDuration;
        
        private Transform _transform;
        private Tween _flashTween;

        private void Awake() => _transform = transform;
        private void OnDestroy() => _flashTween?.Kill();

        private void Start() { 
            _transform.localScale = Vector3.zero;
            gameObject.Deactivate();
        }

        public void Flash() {
            _flashTween?.Complete();
            _flashTween?.Kill();
            
            gameObject.Activate();
            _transform.localScale = Vector3.one;

            _flashTween = transform
                .DOScale(Vector3.zero, _flashDuration)
                .SetEase(Ease.InCirc);
            _flashTween
                .onComplete += gameObject.Deactivate;
        }
    }
}