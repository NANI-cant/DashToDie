using CodeBase.Gameplay.Player.Signals;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace CodeBase.Gameplay.VFX {
    public class HurtVignette: MonoBehaviour {
        [SerializeField] private Volume _volume;
        [SerializeField][Min(0)] private float _duration;

        private SignalBus _signalBus;
        private Tween _weightTween;

        [Inject]
        private void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void OnEnable() => _signalBus.Subscribe<PlayerHurtedSignal>(RecolorVignette);
        private void OnDisable() {
            _signalBus.Unsubscribe<PlayerHurtedSignal>(RecolorVignette);
            _weightTween?.Complete();
            _weightTween?.Kill();
        }

        private void Start() {
            _volume.weight = 0;
        }

        private void RecolorVignette() {
            _weightTween?.Complete();
            _weightTween?.Kill();
            
            _volume.weight = 1;
            _weightTween = DOTween.To(
                ()=>_volume.weight,
                value => _volume.weight = value,
                0,
                _duration
            );
        }
    }
}