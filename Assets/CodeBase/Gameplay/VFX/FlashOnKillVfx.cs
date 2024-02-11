using System;
using CodeBase.Gameplay.Enemies.Signals;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.VFX {
    public class FlashOnKillVfx: MonoBehaviour {
        [SerializeField] private Camera _camera;
        [SerializeField] private Color _color;
        [SerializeField][Min(0)] private float _duration;
        
        private SignalBus _signalBus;
        private Tween _tween;
        private Color _defaultColor;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void Start() {
            _defaultColor = _camera.backgroundColor;
        }

        private void OnEnable() {
            _signalBus.Subscribe<EnemyDiedSignal>(Tween);
        }

        private void OnDisable() {
            _signalBus.Unsubscribe<EnemyDiedSignal>(Tween);
            _tween?.Complete();
            _tween?.Kill();
            _tween = null;
        }

        private void Tween() {
            _tween?.Complete();
            _tween?.Kill();

            Sequence tweenSequence = DOTween.Sequence();
            tweenSequence
                .Append(_camera.DOColor(_color, 0))
                .Append(_camera.DOColor(_defaultColor, _duration).SetEase(Ease.OutCirc));
            _tween = tweenSequence.Play();
        }
    }
}