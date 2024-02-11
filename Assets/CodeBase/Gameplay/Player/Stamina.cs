using System;
using CodeBase.Gameplay.ProgressiveValues;
using UnityEngine;

namespace CodeBase.Gameplay.Player {
    public class Stamina: MonoBehaviour {
        [SerializeField] private ProgressiveFloat _capacity;
        private float _value;

        public float Value => _value;
        public ProgressiveFloat Capacity => _capacity;

        public Action Changed;

        private void Start() {
            _value = Capacity.Value;
        }

        public void Spend(float value) {
            if (value < 0) throw new ArgumentOutOfRangeException();
            if (Value < value) throw new ArgumentOutOfRangeException();

            _value -= value;
            Changed?.Invoke();
        }

        public void Gain(float value) {
            if (value < 0) throw new ArgumentOutOfRangeException();

            _value += value;
            _value = Mathf.Clamp(Value, 0, Capacity.Value);
            Changed?.Invoke();
        }
    }
}