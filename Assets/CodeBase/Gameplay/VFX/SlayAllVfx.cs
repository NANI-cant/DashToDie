using CodeBase.Gameplay.General;
using CodeBase.ProjectContext.Services;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.VFX {
    public class SlayAllVfx: MonoBehaviour, ICancelable {
        [SerializeField] private float _fadeOutDuration = 0.25f;
        [SerializeField] private float _crackSpeed = 0.25f;
        [SerializeField] private Renderer _renderer;
        
        private IDestroyProvider _destroyProvider;
        private Tween _tween;

        [Inject]
        public void Construct(IDestroyProvider destroyProvider) {
            _destroyProvider = destroyProvider;
        }

        private void Awake() {
            transform.localScale = Vector3.zero;
        }

        public void Run(float delay, float radius) {
            _tween = transform
                .DOScale(Vector3.one * radius, delay)
                .SetEase(Ease.Linear);
            
            _tween.onComplete += () => {
                _renderer.material.DOColor(Color.white, 0);
                _renderer.material
                    .DOFloat(0, "_Alpha", _fadeOutDuration)
                    .SetEase(Ease.Linear);
                transform
                    .DOScale(Vector3.one * (radius + _crackSpeed * _fadeOutDuration) , _fadeOutDuration)
                    .SetEase(Ease.Linear);
                _destroyProvider.Destroy(gameObject, 0.25f);
            };
        }

        private void OnDestroy() {
            _tween.Complete();
            _tween.Kill();
        }

        public void Cancel() {
            _tween.Complete();
        }
    }
}