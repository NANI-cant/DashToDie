using System;
using UnityEngine;

namespace CodeBase.Gameplay.ProgressiveValues {
    [Serializable]
    public class ProgressiveFloat {
        [SerializeField][Min(0)] private float _baseValue;
        [SerializeField] private float _multiplier;

        public float Value => Mathf.Max(0, _baseValue + _multiplier * _baseValue);

        public Action Modified;

        public ProgressiveFloat(float baseValue) {
            if (baseValue < 0) throw new ArgumentOutOfRangeException();
            _baseValue = baseValue;
            _multiplier = 0;
        }

        public void Increase(float multiplier) {
            _multiplier += multiplier;
            Modified?.Invoke();
        }
        
        public void Decrease(float multiplier) {
            _multiplier -= multiplier;
            Modified?.Invoke();
        }
    }
}