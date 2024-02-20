using CodeBase.ProjectContext.Signals;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace CodeBase.Gameplay.VFX {
    public class SlowMotionVignette: MonoBehaviour {
        [SerializeField] private Volume _volume;
        [SerializeField][Min(0)] private float _duration;

        private SignalBus _signalBus;
        private Tween _weightTween;

        [Inject]
        private void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void OnEnable() {
            _signalBus.Subscribe<TimeSpeedUpSignal>(ReactToSignal);
            _signalBus.Subscribe<TimeSlowDownSignal>(ReactToSignal);
        }

        private void OnDisable() {
            _signalBus.Unsubscribe<TimeSpeedUpSignal>(ReactToSignal);
            _signalBus.Unsubscribe<TimeSlowDownSignal>(ReactToSignal);
            _weightTween?.Complete();
            _weightTween?.Kill();
        }

        private void Start() {
            _volume.weight = 0;
        }

        private void ReactToSignal(TimeSpeedUpSignal signal) => RecolorVignette(signal.TimeScale);
        private void ReactToSignal(TimeSlowDownSignal signal) => RecolorVignette(signal.TimeScale);

        private void RecolorVignette(float timeScale) {
            _weightTween?.Complete();
            _weightTween?.Kill();

            float weight = timeScale >= 1 ? 0 : 1;

            _weightTween = DOTween.To(
                () => _volume.weight,
                value => _volume.weight = value,
                weight,
                _duration
            );
            _weightTween.timeScale /= timeScale;
        }
    }
}