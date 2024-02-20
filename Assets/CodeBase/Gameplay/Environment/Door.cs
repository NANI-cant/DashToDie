using DG.Tweening;
using UnityEngine;

namespace CodeBase.Gameplay.Environment {
    public class Door: MonoBehaviour {
        [SerializeField][Min(0)] private float _openDuration = 0.1f;
        [SerializeField][Range(-1,1)] private int _openSide = 1;

        private Transform _transform;
        private Tween _doorTween;

        private void Awake() {
            _transform = transform;
        }

        public void Open() {
            _openSide = (int) Mathf.Sign(_openSide);
            
            _doorTween?.Kill();
            _doorTween = _transform
                .DOLocalRotate(new Vector3(0, _openSide * 90, 0), _openDuration)
                .SetEase(Ease.OutCirc);
        }

        public void Close() {
            _doorTween?.Kill();
            _doorTween = _transform
                .DOLocalRotate(Vector3.zero, _openDuration)
                .SetEase(Ease.InCirc);
        }
    }
}