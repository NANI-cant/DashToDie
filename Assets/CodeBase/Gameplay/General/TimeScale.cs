using UnityEngine;

namespace CodeBase.Gameplay.General {
    public class TimeScale {
        [SerializeField][Min(0)] private float _scale = 1;

        public float Scale {
            get => _scale;
            set => _scale = Mathf.Min(0, value);
        }
    }
}