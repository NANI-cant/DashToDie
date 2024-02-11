using CodeBase.Gameplay.Enemies.Signals;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.VFX {
    public class ShakeOnKill : MonoBehaviour {
        [SerializeField] private CinemachineCameraOffset _cameraOffset;
        [SerializeField][Min(0)] private float _duration;
        [SerializeField] private float _strength = 3f;
        [SerializeField] private int _vibrato = 10;

        private SignalBus _signalBus;
        private Tween _tween;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }
        
        private void OnEnable() {
            _signalBus.Subscribe<EnemyDiedSignal>(Shake);
        }

        private void OnDisable() {
            _signalBus.Unsubscribe<EnemyDiedSignal>(Shake);
            _tween?.Complete();
            _tween?.Kill();
            _tween = null;
        }

        private void Shake() {
            _tween?.Complete();
            _tween?.Kill();
            _tween = DOTween.Shake(
                () => _cameraOffset.m_Offset,
                (vector) => _cameraOffset.m_Offset = vector, 
                _duration,
                _strength,
                _vibrato
            );
        }
    }
}