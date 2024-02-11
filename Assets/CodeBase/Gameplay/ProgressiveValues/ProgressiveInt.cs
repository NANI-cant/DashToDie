using System;
using UnityEngine;

namespace CodeBase.Gameplay.ProgressiveValues {
    [Serializable]
    public class ProgressiveInt {
        [SerializeField][Min(0)] private int _baseValue;
        [SerializeField] private float _multiplier;

        public int Value => (int) Mathf.Max(0, _baseValue + _multiplier * _baseValue);
        
        public ProgressiveInt(int baseValue) {
            if (baseValue < 0) throw new ArgumentOutOfRangeException();
            _baseValue = baseValue;
            _multiplier = 0;
        }

        public void Increase(float multiplier) {
            _multiplier += multiplier;
        }
        
        public void Decrease(float multiplier) {
            _multiplier -= multiplier;
        }
    }
}