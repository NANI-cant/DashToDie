using DG.Tweening;
using UnityEngine;

namespace CodeBase.Gameplay.VFX {
    public abstract class CameraShake : MonoBehaviour {
        [SerializeField] private CinemachineCameraOffset _cameraOffset;
        [SerializeField][Min(0)] private float _duration;
        [SerializeField][Min(0)] private float _strength = 3f;
        [SerializeField][Min(0)] private int _vibrato = 10;
        
        private Tween _tween;
        
        public void Shake() {
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

        protected virtual void OnDisable() {
            _tween?.Complete();
            _tween?.Kill();
        }
    }
}